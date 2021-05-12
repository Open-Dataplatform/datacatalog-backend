using System;
using DataCatalog.Api.Extensions;
using DataCatalog.Api.Utils;
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
        public static void Main(string[] args)
        {
            var environmentName = EnvironmentUtil.GetCurrentEnvironment();

            var configuration = new ConfigurationBuilder()
                .BuildPlatformConfiguration(environmentName, args)
                .Build();
            var exceptionLogger =
                DataPlatformLogging.CreateLogger(environmentName, configuration);

            try
            {
                exceptionLogger.Information("Configuring the DataCatalog Api using the environment {Environment}", environmentName);
                var host = Host.CreateDefaultBuilder(args)
                    .UseDataPlatformLogging(environmentName)
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
    }
}
