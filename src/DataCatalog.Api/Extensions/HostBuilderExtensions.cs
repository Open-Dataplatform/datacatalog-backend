using System.IO;
using Microsoft.Extensions.Configuration;

namespace DataCatalog.Api.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IConfigurationBuilder BuildPlatformConfiguration(this IConfigurationBuilder builder, string environmentName, string[] commandLineArgs)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", false, true);
            builder.AddJsonFile($"appsettings.{environmentName}.json", true, true);
            builder.AddEnvironmentVariables();
            builder.AddCommandLine(commandLineArgs);
            return builder;
        }
    }
}