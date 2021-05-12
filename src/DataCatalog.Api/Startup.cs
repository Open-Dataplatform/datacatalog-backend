using System;
using AutoMapper;
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
using Energinet.DataPlatform.Shared.Environments;
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
        readonly string egressApiUrl = "https://dpegresswebapi-appservice-{0}.azurewebsites.net";
        readonly string ingressApiUrl = "https://dpingresswebapi-appservice-{0}.azurewebsites.net";
        readonly string dataCatalogUrl = "https://dpdatacatalogwebapp-appservice-{0}.azurewebsites.net";
        readonly string dataCatalogProdUrl = "https://dataplatform.energinet.dk";
        readonly string dataCatalogAllowSpecificOrigins = "_dataCatalogAllowSpecificOrigins";
        readonly string dataCatalogAllowAll = "_dataCatalogAllowAll";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // This will add WebAppEnvironment to the DI container as a IEnvironment you can take a dependency on        
            services.AddWebAppEnvironment();

            if (!WebAppEnvironment.GetEnvironment().IsProduction())
                services.AddSwagger();

            AddServicesAndDbContext(services);

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
            services.AddSingleton<IAllUsersGroupProvider>(new AllUsersGroupProvider(Configuration.GetValue<string>("ALL_USERS_GROUP")));

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

            //CORS
            services.AddCors(options =>
            {
                options.AddPolicy(dataCatalogAllowAll,
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

                var dataCatalogFormattedUrl = string.Format(dataCatalogUrl, WebAppEnvironment.GetEnvironment().Name.ToLower());
                var ingressApiFormattedUrl = string.Format(ingressApiUrl, WebAppEnvironment.GetEnvironment().Name.ToLower());
                var egressApiFormattedUrl = string.Format(egressApiUrl, WebAppEnvironment.GetEnvironment().Name.ToLower());
                options.AddPolicy(dataCatalogAllowSpecificOrigins,
                    builder => builder.WithOrigins(dataCatalogFormattedUrl, dataCatalogProdUrl, ingressApiFormattedUrl, egressApiFormattedUrl).AllowAnyHeader().AllowAnyMethod());
            });

            // Groups and roles
            AzureAd azureAdConfiguration = Configuration.GetSection("AzureAd").Get<AzureAd>();
            services.AddSingleton(azureAdConfiguration);

            // Azure KeyVault
            var keyVaultEnvironment = WebAppEnvironment.GetEnvironment().IsDevelopment()
                ? Environments.TestingShort
                : WebAppEnvironment.GetEnvironment().Name;

            var client = new SecretClient(new Uri($"https://dpvault-{keyVaultEnvironment}.vault.azure.net/"), new DefaultAzureCredential());
            var groupManagementClientSecret = client.GetSecret(Configuration["GroupManagementClientSecretName"]);
            
            // Graph client registration
            IConfidentialClientApplication confidentialGroupClient = ConfidentialClientApplicationBuilder
                .Create(Configuration["GroupManagementClientId"])
                .WithClientSecret(groupManagementClientSecret.Value.Value)
                .WithTenantId(azureAdConfiguration.TenantId)
                .Build();

            services.AddSingleton<IGraphServiceClient>(
                new GraphServiceClient(new ClientCredentialProvider(confidentialGroupClient)));

            services.AddTransient<IActiveDirectoryGroupService, AzureActiveDirectoryGroupService>();

            var storageAccountName = $"dpcontentstorage{WebAppEnvironment.GetEnvironment().Name}";
            var serviceEndpoint = new Uri($"https://{storageAccountName}.blob.core.windows.net");
            services.AddSingleton(x => new DataLakeServiceClient(serviceEndpoint, new DefaultAzureCredential()));
            services.AddTransient<IStorageService, AzureStorageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, DataCatalogContext db, IEnvironment environment)
        {
            db.Database.Migrate();

            //One-time update of OriginEnvironment
            var datasets = db.Datasets.Where(a => a.OriginEnvironment == null).ToArray();
            foreach (var a in datasets) 
                a.OriginEnvironment = WebAppEnvironment.GetEnvironment().Name.ToLower();
            var dataContracts = db.DataContracts.Where(a => a.OriginEnvironment == null).ToArray();
            foreach (var a in dataContracts) 
                a.OriginEnvironment = WebAppEnvironment.GetEnvironment().Name.ToLower();
            var categories = db.Categories.Where(a => a.OriginEnvironment == null).ToArray();
            foreach (var a in categories) 
                a.OriginEnvironment = WebAppEnvironment.GetEnvironment().Name.ToLower();
            var dataSources = db.DataSources.Where(a => a.OriginEnvironment == null).ToArray();
            foreach (var a in dataSources) 
                a.OriginEnvironment = WebAppEnvironment.GetEnvironment().Name.ToLower();
            db.SaveChanges();

            // Use IEnvironment to check what environment the web app is running in
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Swagger setup from extensions
            if (!environment.IsProduction()) 
                app.UseCustomSwagger();

            // Exception handling and logging.
            app.UseMiddleware<ExceptionExtension>();
            app.UseHttpsRedirection();
            app.UseRouting();

            // CORS
            if (environment.IsDevelopment())
                app.UseCors(dataCatalogAllowAll);
            else 
                app.UseCors(dataCatalogAllowSpecificOrigins);

            app.UseAuthentication();

            app.UseAuthorization();

            // Push properties to the log context
            app.Use(async (context, next) =>
            {
                LogContext.PushProperty("UserName", context.User.Identity.Name);
                await next.Invoke();
            });

            app.UseSerilogRequestLogging(config => config.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms from user {UserName}");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        internal void AddServicesAndDbContext(IServiceCollection services)
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
            conn = string.Format(conn, WebAppEnvironment.GetEnvironment().Name.ToLower(), Configuration["sqlpassword"]);
            services.AddDbContext<DataCatalogContext>(o => o.UseSqlServer(conn));

            //HttpContext
            services.AddHttpContextAccessor();

            services.AddScoped<Current>();
        }
    }
}
