using System;
using DataCatalog.Api.Data;
using DataCatalog.Api.Data.Common;
using DataCatalog.Api.Extensions;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Repositories;
using DataCatalog.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Serilog;
using System.Runtime.CompilerServices;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Files.DataLake;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Implementations;
using DataCatalog.Api.Interfaces;
using DataCatalog.Api.MessageBus;
using DataCatalog.Api.Services.AD;
using DataCatalog.Api.Services.Storage;
using DataCatalog.Api.Utils;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
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

            AddServicesAndDbContext(services);

            var azureAdConfiguration = Configuration.GetSection("AzureAd").Get<AzureAd>();
            ValidateAzureAdConfiguration(azureAdConfiguration);
            // Groups and roles
            services.AddSingleton(azureAdConfiguration);

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Bearer";
            }).AddJwtBearer(options => Configuration.Bind("AzureAd", options));

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
            services.AddScoped<IUnitIOfWork, UnitOfWork>();
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
            
            // Graph client registration
            var groupManagementClientId = Configuration.GetValidatedStringValue("GroupManagementClientId");
            Log.Information("GroupManagementClientId = {GroupManagementClientId}", groupManagementClientId);
            var groupManagementClientSecret = Configuration.GetValidatedStringValue("GroupManagementClientSecret");
            
            var confidentialGroupClient = ConfidentialClientApplicationBuilder
                .Create(groupManagementClientId)
                .WithClientSecret(groupManagementClientSecret)
                .WithTenantId(azureAdConfiguration.TenantId)
                .Build();
            
            services.AddSingleton<IGraphServiceClient>(
                new GraphServiceClient(new ClientCredentialProvider(confidentialGroupClient)));
            
            services.AddTransient<IActiveDirectoryGroupService, AzureActiveDirectoryGroupService>();

            var dataCatalogBlobStorageUrl = Configuration.GetValidatedStringValue("DataCatalogBlobStorageUrl");
            Log.Information("DataCatalogBlobStorageUrl = {DataCatalogBlobStorageUrl}", dataCatalogBlobStorageUrl);
            var serviceEndpoint = new Uri(dataCatalogBlobStorageUrl);
            services.AddSingleton(x => new DataLakeServiceClient(serviceEndpoint, new DefaultAzureCredential()));
            services.AddTransient<IStorageService, AzureStorageService>();

            services.AddHealthChecks();
        }

        private void ValidateAzureAdConfiguration(AzureAd azureAdConfiguration)
        {
            if (azureAdConfiguration == null)
            {
                throw new ArgumentException("'AzureAd' must have a value");
            }

            azureAdConfiguration.Audience.ValidateConfiguration("AzureAd:Audience");
            azureAdConfiguration.Authority.ValidateConfiguration("AzureAd:Authority");
            azureAdConfiguration.TenantId.ValidateConfiguration("AzureAd:TenantId");
            if (azureAdConfiguration.Roles == null)
            {
                throw new ArgumentException("'AzureAd:Roles' must have a value");
            }

            azureAdConfiguration.Roles.Admin.ValidateConfiguration("AzureAd:Roles:Admin");
            azureAdConfiguration.Roles.User.ValidateConfiguration("AzureAd:Roles:User");
            azureAdConfiguration.Roles.DataSteward.ValidateConfiguration("AzureAd:Roles:DataSteward");
            Log.Information("Logging Configuration - start AzureAd");
            Log.Information("AzureAd:Audience = {Audience}", azureAdConfiguration.Audience);
            Log.Information("AzureAd:Authority = {Authority}", azureAdConfiguration.Authority);
            Log.Information("AzureAd:TenantId = {TenantId}", azureAdConfiguration.TenantId);
            Log.Information("AzureAd:Roles:Admin = {AdminRole}", azureAdConfiguration.Roles.Admin);
            Log.Information("AzureAd:Roles:User = {UserRole}", azureAdConfiguration.Roles.User);
            Log.Information("AzureAd:Roles:DataSteward = {DataStewardRole}", azureAdConfiguration.Roles.DataSteward);
            Log.Information("Logging Configuration - end AzureAd");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, DataCatalogContext db)
        {
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
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
            services.AddTransient(typeof(IMessageBusSender<>), typeof(MessageBusSender<>));

            // Hosted services
            services.AddHostedService<MessageBusReceiver<DatasetProvisioned, IDatasetService>>();

            // Db Context
            var conn = Configuration.GetConnectionString("DataCatalog");
            conn.ValidateConfiguration("ConnectionStrings:DataCatalog");
            conn = string.Format(conn, EnvironmentUtil.GetCurrentEnvironment().ToLower(), Configuration.GetValidatedStringValue("SqlPassword"));
            services.AddDbContext<DataCatalogContext>(o => o.UseSqlServer(conn));

            //HttpContext
            services.AddHttpContextAccessor();

            services.AddScoped<Current>();
        }
    }
}
