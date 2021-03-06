using System;
using System.Runtime.CompilerServices;
using Azure.Identity;
using Azure.Storage.Files.DataLake;
using DataCatalog.Api.Data;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Api.Extensions;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.MessageHandlers;
using DataCatalog.Api.Repositories;
using DataCatalog.Api.Services;
using DataCatalog.Api.Services.AD;
using DataCatalog.Api.Services.Egress;
using DataCatalog.Api.Services.Local;
using DataCatalog.Api.Services.Storage;
using DataCatalog.Api.Utils;
using DataCatalog.Common.Data;
using DataCatalog.Common.Extensions;
using DataCatalog.Common.Implementations;
using DataCatalog.Common.Interfaces;
using DataCatalog.Common.Middleware;
using DataCatalog.Common.Rebus;
using DataCatalog.Common.Rebus.Extensions;
using DataCatalog.Common.Utils;
using DataCatalog.Data;
using DataCatalog.DatasetResourceManagement.Messages;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Prometheus;
using Serilog;
using Serilog.Context;

[assembly: ApiConventionType(typeof(ApiConventions))]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("DataCatalog.Api.UnitTests")]
[assembly: InternalsVisibleTo("DataCatalog.Api.IntegrationTests")]
namespace DataCatalog.Api
{
    public class Startup
    {
        private const string DataCatalogAllowSpecificOrigins = "_dataCatalogAllowSpecificOrigins";
        private const string DataCatalogAllowAll = "_dataCatalogAllowAll";
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (!EnvironmentUtil.IsProduction())
                services.AddSwagger();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            AddServicesAndDbContext(services);

            var oAuthConfiguration = Configuration.GetSection("OAuth").Get<OAuth>();
            ValidateOAuthConfiguration(oAuthConfiguration);
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Audience = oAuthConfiguration.Audience;
                options.Authority = oAuthConfiguration.Authority;
            });

            // Controllers
            services.AddControllers(options => options.Filters.Add<CustomExceptionFilter>());

            // Repositories
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IDataContractRepository, DataContractRepository>();
            services.AddScoped<IDataFieldRepository, DataFieldRepository>();
            services.AddScoped<IDatasetCategoryRepository, DatasetCategoryRepository>();
            services.AddScoped<IDatasetChangeLogRepository, DatasetChangeLogRepository>();
            services.AddScoped<IDatasetDurationRepository, DatasetDurationRepository>();
            services.AddScoped<IDatasetGroupRepository, DatasetGroupRepository>();
            services.AddScoped<IDatasetRepository, DatasetRepository>();
            services.AddScoped<IDataSourceRepository, DataSourceRepository>();
            services.AddScoped<IDurationRepository, DurationRepository>();
            services.AddScoped<IIdentityProviderRepository, IdentityProviderRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<ITransformationDatasetRepository, TransformationDatasetRepository>();
            services.AddScoped<ITransformationRepository, TransformationRepository>();
            services.AddScoped<IServiceLevelAgreementRepository, ServiceLevelAgreementRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            var allUsersGroup = Configuration.GetValidatedStringValue("ALL_USERS_GROUP");
            services.AddSingleton<IAllUsersGroupProvider>(new AllUsersGroupProvider(allUsersGroup));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddApiVersioning(config =>
            {
                // Specify the default API Version
                config.DefaultApiVersion = new ApiVersion(1, 0);
                // If the client hasn't specified the API version in the request, use the default API version number 
                config.AssumeDefaultVersionWhenUnspecified = true;
                // Advertise the API versions supported for the particular endpoint
                config.ReportApiVersions = true;
                // DEFAULT Version reader is QueryStringApiVersionReader();  
                // clients request the specific version using the X-version header
                config.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });

            var dataCatalogUrl = Configuration.GetValidatedStringValue("AllowedCorsUrls:DataCatalogUrl");
            var dataCatalogProdUrl = Configuration.GetValidatedStringValue("AllowedCorsUrls:DataCatalogProdUrl");
            var ingressApiUrl = Configuration.GetValidatedStringValue("AllowedCorsUrls:IngressApiUrl");
            var egressApiUrl = Configuration.GetValidatedStringValue("AllowedCorsUrls:EgressApiUrl");
            Log.Information("Logging configuration - AllowedCorsUrls start");
            Log.Information("AllowedCorsUrls:DataCatalogUrl = {DataCatalogUrl}", dataCatalogUrl);
            Log.Information("AllowedCorsUrls:DataCatalogProdUrl = {DataCatalogProdUrl}", dataCatalogProdUrl);
            Log.Information("AllowedCorsUrls:IngressApiUrl = {IngressApiUrl}", ingressApiUrl);
            Log.Information("AllowedCorsUrls:EgressApiUrl = {EgressApiUrl}", egressApiUrl);
            Log.Information("Logging configuration - AllowedCorsUrls end");

            //CORS
            services.AddCors(options =>
            {
                options.AddPolicy(DataCatalogAllowAll,
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

                options.AddPolicy(DataCatalogAllowSpecificOrigins,
                    builder => builder.WithOrigins(dataCatalogUrl, dataCatalogProdUrl, ingressApiUrl, egressApiUrl).AllowAnyHeader().AllowAnyMethod());
            });

            if (EnvironmentUtil.IsDevelopment())
            {
                services.RemoveAll(typeof(IAuthorizationHandler));
                services.AddSingleton<IAuthorizationHandler, AllowAnonymousAuthorizationHandler>();
            }

            if (Configuration.GetValue<bool>("AzureAd:Enabled"))
            {
                ConfigureAzureServices(services);
            }

            Log.Information("Logging configuration - ContactInfo start");
            Log.Information("ContactInfo:Name = {Name}", Configuration.GetValidatedStringValue("ContactInfo:Name"));
            Log.Information("ContactInfo:Name = {Link}", Configuration.GetValidatedStringValue("ContactInfo:Link"));
            Log.Information("ContactInfo:Name = {Email}", Configuration.GetValidatedStringValue("ContactInfo:Email"));
            Log.Information("Logging configuration - ContactInfo end");
            services.Configure<ContactInfo>(Configuration.GetSection(nameof(ContactInfo)));

            //CorrelationId section
            services.AddSingleton<ICorrelationIdResolver, CorrelationIdResolver>();
            services.AddSingleton<ICorrelationIdProvider, WebApiCorrelationIdProvider>();
            services.AddSingleton<ICorrelationIdProvider, RebusCorrelationIdProvider>();
            
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, DataCatalogContext db)
        {
            app.UseForwardedHeaders();

            // Use IEnvironment to check what environment the web app is running in
            if (EnvironmentUtil.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Swagger setup from extensions
            if (!EnvironmentUtil.IsProduction()) 
                app.UseCustomSwagger(Configuration.GetValue("Swagger:VirtualBasePath", ""));

            // Exception handling and logging.
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseHttpMetrics();

            // CORS
            if (EnvironmentUtil.IsDevelopment())
                app.UseCors(DataCatalogAllowAll);
            else 
                app.UseCors(DataCatalogAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            // Push properties to the log context
            app.Use(async (context, next) =>
            {
                LogContext.PushProperty("UserName", context.User.Identity?.Name);
                await next.Invoke();
            });

            app.UseSerilogRequestLogging(config => config.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms from user {UserName}");

            if (EnvironmentUtil.IsDevelopment())
            {
                app.UseMiddleware<LocalCurrentUserInitializationMiddleware>();
            }
            else 
            {
                app.UseMiddleware<CurrentUserInitializationMiddleware>();
            }
            app.UseMiddleware<CorrelationIdMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
                endpoints.MapHealthChecks("/health");
            });

            app.ApplicationServices.UseRebusSubscriptions(new[]
            {
                typeof(DatasetProvisionedMessage)
            });
        }

        private void ConfigureAzureServices(IServiceCollection services)
        {
            // Graph client registration
            var groupManagementClientId = Configuration.GetValidatedStringValue("GroupManagementClientId");
            Log.Information("GroupManagementClientId = {GroupManagementClientId}", groupManagementClientId);
            var tenantId = Configuration.GetValidatedStringValue("AzureAd:TenantId");
            Log.Information("AzureAd:TenantId = {TenantId}", tenantId);
            var groupManagementClientSecret = Configuration.GetValidatedStringValue("GroupManagementClientSecret");
            
            var confidentialGroupClient = ConfidentialClientApplicationBuilder
                .Create(groupManagementClientId)
                .WithClientSecret(groupManagementClientSecret)
                .WithTenantId(tenantId)
                .Build();
            
            services.AddSingleton<IGraphServiceClient>(
                new GraphServiceClient(new ClientCredentialProvider(confidentialGroupClient)));

            services.AddTransient<IGroupService, AzureGroupService>();

            // Data lake registration
            var dataCatalogBlobStorageUrl = Configuration.GetValidatedStringValue("DataCatalogBlobStorageUrl");
            Log.Information("DataCatalogBlobStorageUrl = {DataCatalogBlobStorageUrl}", dataCatalogBlobStorageUrl);
            var dataLakeClientId = Configuration.GetValidatedStringValue("DataLakeClientId");
            Log.Information("DataLakeClientId = {DataLakeClientId}", dataLakeClientId);
            var dataLakeClientSecret = Configuration.GetValidatedStringValue("DataLakeClientSecret");

            var serviceEndpoint = new Uri(dataCatalogBlobStorageUrl);
            services.AddSingleton(x => new DataLakeServiceClient(serviceEndpoint, new ClientSecretCredential(tenantId, dataLakeClientId, dataLakeClientSecret)));
            services.AddTransient<IStorageService, AzureStorageService>();
        }

        private static void ValidateOAuthConfiguration(OAuth oAuthConfiguration)
        {
            if (oAuthConfiguration == null)
            {
                throw new ArgumentException("'OAuth' must have a value");
            }

            oAuthConfiguration.Audience.ValidateConfiguration("OAuth:Audience");
            oAuthConfiguration.Authority.ValidateConfiguration("OAuth:Authority");

            Log.Information("Logging Configuration - start OAuth");
            Log.Information("OAuth:Audience = {Audience}", oAuthConfiguration.Audience);
            Log.Information("OAuth:Authority = {Authority}", oAuthConfiguration.Authority);
            Log.Information("Logging Configuration - end OAuth");
        }

        private void AddServicesAndDbContext(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));

            // Services
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IDataContractService, DataContractService>();
            services.AddTransient<IDatasetGroupService, DatasetGroupService>();
            services.AddTransient<IDatasetService, DatasetService>();
            services.AddTransient<IDataSourceService, DataSourceService>();
            services.AddTransient<IDurationService, DurationService>();
            services.AddTransient<IIdentityProviderService, IdentityProviderService>();
            services.AddTransient<IMemberService, MemberService>();
            services.Configure<EgressOptions>(Configuration.GetSection("Egress"));
            services.AddTransient<IEgressService, EgressService>();
            services.AddHttpClient<IEgressService, EgressService>();

            services.AddTransient<ITransformationService, TransformationService>();
            services.AddTransient<IServiceLevelAgreementService, ServiceLevelAgreementService>();
                        
            services.AddScoped<IPermissionUtils, PermissionUtils>();

            // Db Context
            var conn = Configuration.GetConnectionString("DataCatalog");
            conn.ValidateConfiguration("ConnectionStrings:DataCatalog");
            services.AddDbContext<DataCatalogContext>(o => o.UseSqlServer(conn));
            
            services.AddRebusWithSubscription<DatasetProvisionedHandler>(Configuration, conn, new[]
            {
                typeof(DatasetProvisionedMessage)
            });

            if (EnvironmentUtil.IsDevelopment())
            {
                services.AddTransient<IGroupService, LocalGroupService>();
                services.AddTransient<IStorageService, LocalStorageService>();
            }

            //HttpContext
            services.AddHttpContextAccessor();

            services.AddScoped<Current>();
        }
    }
}
