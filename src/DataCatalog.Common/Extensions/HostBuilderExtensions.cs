using System;
using System.IO;
using DataCatalog.Common.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace DataCatalog.Common.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IConfigurationBuilder BuildPlatformConfiguration(this IConfigurationBuilder builder, string environmentName, string[] commandLineArgs)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", false, true);
            builder.AddJsonFile($"appsettings.{environmentName.FirstCharToUpper()}.json", true, true);
            builder.AddJsonFile("/vault/secrets/appsettings.secrets.json", true, true); // This file is injected when running on Kubernetes
            builder.AddEnvironmentVariables();
            builder.AddCommandLine(commandLineArgs);
            return builder;
        }

        public static void CreateHostBuilderWithStartup<TStartup>(string serviceName, string[] args, Action<LoggerConfiguration> additionalSerilogConfig = default)
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
                Log.Information("Configuring the {ServiceName} using the environment {Environment}", serviceName, environmentName);
                var host = CreateHost<TStartup>(args);
                Log.Information("Completed configuration of the {ServiceName}", serviceName);
                Log.Information("Starting up the {ServiceName}", serviceName);
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
        
        private static IHost CreateHost<TStartup>(string[] args, Action<LoggerConfiguration> additionalSerilogConfig = default)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.FromLogContext()
                        .Enrich.WithEnvironment()
                        .Enrich.WithCorrelationIdHeader(CorrelationId.CorrelationIdHeaderKey);
                    additionalSerilogConfig?.Invoke(loggerConfiguration);

                })
                .ConfigureAppConfiguration((_, builder) => 
                {
                    builder.BuildPlatformConfiguration(EnvironmentUtil.GetCurrentEnvironment(), args);
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup(typeof(TStartup)); })
                .Build();
        }
    }
}