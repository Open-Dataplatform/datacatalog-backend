using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;
using DataCatalog.Common.Extensions;
using DataCatalog.Common.Rebus.Extensions;
using DataCatalog.Common.Utils;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.Storage;
using DataCatalog.DatasetResourceManagement.MessageHandlers;
using DataCatalog.DatasetResourceManagement.Services.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.Services.ActiveDirectory.Middleware;
using DataCatalog.DatasetResourceManagement.Services.Storage;
using Energinet.DataPlatform.DataSetResourceManagement.Infrastructure.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Polly.Retry;
using Rebus.ServiceProvider;

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
            var config = new InfrastructureConfiguration();
            _configuration.Bind(config);
            services.AddTransient<IStorageService, StorageService>();

            var storageAccountName = $"dpcontentstorage{EnvironmentUtil.GetCurrentEnvironment()}";
            var serviceEndpoint = new Uri($"https://{storageAccountName}.blob.core.windows.net");
            services.AddSingleton(x => new DataLakeServiceClient(serviceEndpoint, new DefaultAzureCredential()));

            var connectionString = _configuration.GetValidatedStringValue("ConnectionStrings:DataCatalog");
            services.AddRebusWithSubscription<DatasetCreatedHandler>(_configuration, connectionString);

            IConfidentialClientApplication confidentialGroupClient = ConfidentialClientApplicationBuilder
                .Create(config.ClientId)
                .WithClientSecret(config.ClientSecret)
                .WithTenantId(config.TenantId)
                .Build();

            services.AddSingleton<IGraphServiceClient>(
                new GraphServiceClient(new ClientCredentialProvider(confidentialGroupClient)));

            services.AddTransient<IActiveDirectoryGroupService, AzureActiveDirectoryGroupService>();
            services.AddTransient<IAccessControlListService, AccessControlListService>();
            services.AddSingleton(confidentialGroupClient);
            services.AddTransient<ITokenProvider, TokenProvider>();
            services.AddTransient<GroupAuthenticationMiddleware>();
            services.AddTransient<IActiveDirectoryGroupProvider, AzureActiveDirectoryGroupProvider>();
            services.AddTransient<IActiveDirectoryRootGroupProvider, AzureActiveDirectoryRootGroupProvider>();
            services.AddTransient<ILeaseClientProvider, DataLakeLeaseClientProvider>();

            services.AddHttpClient<IActiveDirectoryGroupService, AzureActiveDirectoryGroupService>(httpConfig =>
                    httpConfig.BaseAddress = config.AadProvisionerBaseUrl)
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}