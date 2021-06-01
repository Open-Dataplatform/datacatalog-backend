using System;

namespace DataCatalog.Common.Utils
{
    public static class EnvironmentUtil
    {
        private const string EnvironmentVariableKey = "ASPNETCORE_ENVIRONMENT";
        
        public static string GetCurrentEnvironment()
        {
            return Environment.GetEnvironmentVariable(EnvironmentVariableKey);
        }

        /// <summary>
        /// Returns whether or not the environment is a Local environment (e.g. without security)
        /// </summary>
        /// <returns><c>True</c>, to indicate the environment is a Local environment, otherwise <c>false</c>.</returns>
        public static bool IsLocal() => IsEnvironment("Local");
        
        /// <summary>
        /// Returns whether or not the environment is a development environment.
        /// </summary>
        /// <returns><c>True</c>, to indicate the environment is a development environment, otherwise <c>false</c>.</returns>
        public static bool IsDevelopment() => !IsTesting() && !IsPreProduction() && !IsProduction();

        /// <summary>
        /// Returns whether or not the environment is a testing environment.
        /// </summary>
        /// <returns><c>True</c>, to indicate the environment is a testing environment, otherwise <c>false</c>.</returns>
        public static bool IsTesting() => IsEnvironment("Testing") || IsEnvironment("Test");

        /// <summary>
        /// Returns whether or not the environment is a pre-production environment.
        /// </summary>
        /// <returns><c>True</c>, to indicate the environment is a pre-production environment, otherwise <c>false</c>.</returns>
        public static bool IsPreProduction() => IsEnvironment("PreProduction") || IsEnvironment("PreProd");

        /// <summary>
        /// Returns whether or not the environment is a production environment.
        /// </summary>
        /// <returns><c>True</c>, to indicate the environment is a production environment, otherwise <c>false</c>.</returns>
        public static bool IsProduction() => IsEnvironment("Production") || IsEnvironment("Prod");
        
        /// <summary>
        /// Returns whether or not the current environment name matches the specified environment name.
        /// </summary>
        /// <param name="environmentName">The environment name to compare to the current environment name.</param>
        /// <returns><c>True</c>, if the current environment name matches the specified environment name, otherwise <c>false</c>.</returns>
        private static bool IsEnvironment(string environmentName) => string.Equals(Environment.GetEnvironmentVariable(EnvironmentVariableKey), environmentName, StringComparison.OrdinalIgnoreCase);
    }
}