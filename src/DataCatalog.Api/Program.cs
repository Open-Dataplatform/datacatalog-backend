using System;
using DataCatalog.Api.Extensions;
using Energinet.DataPlatform.Shared.Environments;
using Energinet.DataPlatform.Shared.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;

namespace DataCatalog.Api
{
    public class Program
    {
        private static WebAppEnvironment environment;

        public static void Main(string[] args)
        {
            // Use the GetEnvironment() method to access the current environment when IoC isn't available
            environment = WebAppEnvironment.GetEnvironment();

            var configuration = new ConfigurationBuilder()
                .BuildPlatformConfiguration(environment.Name, args)
                .Build();
            var exceptionLogger =
                DataPlatformLogging.CreateLogger(environment.Name, configuration);

            try
            {
                exceptionLogger.Information("Configuring the DataCatalog Api using the environment {Environment}", environment.Name);
                var host = CreateHost(args, configuration);
                exceptionLogger.Information("Completed configuration of the DataCatalog Api");
                exceptionLogger.Information("Starting up the DataCatalog Api");
                host.Run();
            }
            catch (Exception ex)
            {
                exceptionLogger.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                exceptionLogger?.Dispose();
            }
        }

        private static IHost CreateHost(string[] args, IConfigurationRoot configuration)
        {
            return Host.CreateDefaultBuilder(args)
                .UseDataPlatformLogging(environment.Name)
                .ConfigureAppConfiguration((context, config) =>
                {
                    if (!context.HostingEnvironment.IsDevelopment())
                    {
                        var azureServiceTokenProvider = new AzureServiceTokenProvider();
                        var keyVaultClient = new KeyVaultClient(
                            new KeyVaultClient.AuthenticationCallback(
                                azureServiceTokenProvider.KeyVaultTokenCallback));

                        config.AddAzureKeyVault(
                            configuration.GetValidatedStringValue("DataCatalogKeyVaultUrl"),
                            keyVaultClient,
                            new DefaultKeyVaultSecretManager());
                    }
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .Build();
        }
    }
}
