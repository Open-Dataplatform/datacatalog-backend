using System.IO;
using Microsoft.Extensions.Configuration;

namespace DataCatalog.Common.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IConfigurationBuilder BuildPlatformConfiguration(this IConfigurationBuilder builder, string environmentName, string[] commandLineArgs)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", false, true);
            builder.AddJsonFile($"appsettings.{environmentName.FirstCharToUpper()}.json", true, true);
            builder.AddJsonFile($"/vault/secrets/appsettings.secrets.json", false, true); // This file is injected when running on Kubernetes
            builder.AddEnvironmentVariables();
            builder.AddCommandLine(commandLineArgs);
            return builder;
        }
    }
}