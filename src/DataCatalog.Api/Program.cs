using System;
using DataCatalog.Api.Extensions;
using DataCatalog.Api.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

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
            
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironment()
                .CreateLogger();

            try
            {
                Log.Information("Configuring the DataCatalog Api using the environment {Environment}", environmentName);
                var host = CreateHost(args, configuration);
                Log.Information("Completed configuration of the DataCatalog Api");
                Log.Information("Starting up the DataCatalog Api");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        
        private static IHost CreateHost(string[] args, IConfiguration configuration)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.FromLogContext()
                        .Enrich.WithEnvironment();
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    if (!context.HostingEnvironment.IsDevelopment() && !EnvironmentUtil.IsLocal())
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
