using System;
using Azure.Identity;
using Azure.Storage.Files.DataLake;
using DataCatalog.Api.Messages;
using DataCatalog.Common.Extensions;
using DataCatalog.Common.Interfaces;
using DataCatalog.Common.Rebus.Extensions;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.Storage;
using DataCatalog.DatasetResourceManagement.MessageHandlers;
using DataCatalog.DatasetResourceManagement.Services.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.Services.ActiveDirectory.Middleware;
using DataCatalog.DatasetResourceManagement.Services.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Serilog;

namespace DataCatalog.DatasetResourceManagement
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Graph client registration
            var groupManagementClientId = _configuration.GetValidatedStringValue("GroupManagementClientId");
            Log.Information("GroupManagementClientId = {GroupManagementClientId}", groupManagementClientId);
            var tenantId = _configuration.GetValidatedStringValue("AzureAd:TenantId");
            Log.Information("AzureAd:TenantId = {TenantId}", tenantId);
            var groupManagementClientSecret = _configuration.GetValidatedStringValue("GroupManagementClientSecret");
            
            var confidentialGroupClient = ConfidentialClientApplicationBuilder
                .Create(groupManagementClientId)
                .WithClientSecret(groupManagementClientSecret)
                .WithTenantId(tenantId)
                .Build();

            services.AddSingleton<IGraphServiceClient>(
                new GraphServiceClient(new ClientCredentialProvider(confidentialGroupClient)));
            
            services.AddTransient<IStorageService, StorageService>();

            // Data lake registration
            var dataCatalogBlobStorageUrl = _configuration.GetValidatedStringValue("DataCatalogBlobStorageUrl");
            Log.Information("DataCatalogBlobStorageUrl = {DataCatalogBlobStorageUrl}", dataCatalogBlobStorageUrl);
            var dataLakeClientId = _configuration.GetValidatedStringValue("DataLakeClientId");
            Log.Information("DataLakeClientId = {DataLakeClientId}", dataLakeClientId);
            var dataLakeClientSecret = _configuration.GetValidatedStringValue("DataLakeClientSecret");

            var serviceEndpoint = new Uri(dataCatalogBlobStorageUrl);
            services.AddSingleton(x => new DataLakeServiceClient(serviceEndpoint, 
                new ClientSecretCredential(tenantId, dataLakeClientId, dataLakeClientSecret))
            );

            var connectionString = _configuration.GetValidatedStringValue("ConnectionStrings:DataCatalog");
            services.AddRebusWithSubscription<DatasetCreatedHandler>(_configuration, connectionString);

            services.AddTransient<IActiveDirectoryGroupService, AzureActiveDirectoryGroupService>();
            services.AddTransient<IAccessControlListService, AccessControlListService>();
            services.AddSingleton(confidentialGroupClient);
            services.AddTransient<ITokenProvider, TokenProvider>();
            services.AddTransient<GroupAuthenticationMiddleware>();
            services.AddTransient<IActiveDirectoryGroupProvider, AzureActiveDirectoryGroupProvider>();
            services.AddTransient<IActiveDirectoryRootGroupProvider, AzureActiveDirectoryRootGroupProvider>();
            services.AddTransient<ILeaseClientProvider, DataLakeLeaseClientProvider>();
            var allUsersGroup = _configuration.GetValidatedStringValue("ALL_USERS_GROUP");
            services.AddSingleton<IAllUsersGroupProvider>(new AllUsersGroupProvider(allUsersGroup));

            var aadProvisionerBaseUrl = new Uri(_configuration.GetValidatedStringValue("AadProvisionerBaseUrl"));
            services.AddHttpClient<IActiveDirectoryGroupService, AzureActiveDirectoryGroupService>(httpConfig =>
                    httpConfig.BaseAddress = aadProvisionerBaseUrl)
                .AddHttpMessageHandler<GroupAuthenticationMiddleware>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.ApplicationServices.UseRebusSubscriptions(new[]
            {
                typeof(DatasetCreatedMessage)
            });
        }
    }
}