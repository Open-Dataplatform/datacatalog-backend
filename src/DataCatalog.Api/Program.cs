using System;
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
        private static WebAppEnvironment _environment;

        public static void Main(string[] args)
        {
            // Use the GetEnvironment() method to access the current environment when IoC isn't available    
            _environment = WebAppEnvironment.GetEnvironment();

            var exceptionLogger = DataPlatformLogging.CreateLogger(_environment.Name);

            try
            {
                exceptionLogger.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
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

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseDataPlatformLogging(_environment.Name)
                .ConfigureAppConfiguration((context, config) =>
                {
                    if (!context.HostingEnvironment.IsDevelopment())
                    {
                        var azureServiceTokenProvider = new AzureServiceTokenProvider();
                        var keyVaultClient = new KeyVaultClient(
                            new KeyVaultClient.AuthenticationCallback(
                                azureServiceTokenProvider.KeyVaultTokenCallback));

                        config.AddAzureKeyVault(
                            $"https://dpvault-{_environment.Name}.vault.azure.net/",
                            keyVaultClient,
                            new DefaultKeyVaultSecretManager());
                    }
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}