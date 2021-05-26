using System;
using Serilog;
using Serilog.Configuration;

namespace DataCatalog.Common.Extensions
{
    public static class LoggerConfigurationExtensions
    {
        /// <summary>
        /// Adds the environment string ASPNETCORE_ENVIRONMENT from the process environment as a property on log messages
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static LoggerConfiguration WithEnvironment(this LoggerEnrichmentConfiguration configuration)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return configuration.When(
                _ =>
                    !string.IsNullOrEmpty(environment),
                conf => conf.WithProperty("Environment", environment));
        }
    }
}