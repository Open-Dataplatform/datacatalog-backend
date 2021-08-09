using System;
using System.Runtime.CompilerServices;
using Azure.Identity;
using Azure.Storage.Files.DataLake;
using DataCatalog.Api.Extensions;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.MessageHandlers;
using DataCatalog.Api.Repositories;
using DataCatalog.Api.Services;
using DataCatalog.Api.Services.AD;
using DataCatalog.Api.Services.Local;
using DataCatalog.Api.Services.Storage;
using DataCatalog.Common.Data;
using DataCatalog.Common.Extensions;
using DataCatalog.Common.Implementations;
using DataCatalog.Common.Interfaces;
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
            services.AddControllers();

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
            services.AddScoped<IHierarchyRepository, HierarchyRepository>();
            services.AddScoped<IIdentityProviderRepository, IdentityProviderRepository>();
            services.AddScoped<IMemberGroupRepository, MemberGroupRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<ITransformationDatasetRepository, TransformationDatasetRepository>();
            services.AddScoped<ITransformationRepository, TransformationRepository>();
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
                app.UseCustomSwagger();

            // Exception handling and logging.
            app.UseMiddleware<ExceptionExtension>();
            app.UseHttpsRedirection();
            app.UseRouting();

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
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
            services.AddTransient<IHierarchyService, HierarchyService>();
            services.AddTransient<IIdentityProviderService, IdentityProviderService>();
            services.AddTransient<IMemberGroupService, MemberGroupService>();
            services.AddTransient<IMemberService, MemberService>();

            services.AddTransient<ITransformationService, TransformationService>();
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
