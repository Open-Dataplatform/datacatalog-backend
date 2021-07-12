using System;
using Microsoft.Extensions.Configuration;

namespace DataCatalog.Common.Extensions
{
    public static class ConfigurationValidationExtensions
    {
        public static void ValidateConfiguration(this string conf, string configurationName)
        {
            if (string.IsNullOrEmpty(conf))
            {
                throw new ArgumentException($"'{configurationName}' must have a value");
            }
        }
        
        public static string GetValidatedStringValue(this IConfiguration conf, string configurationKey)
        {
            var stringValue = conf.GetValue<string>(configurationKey);
            
            if (string.IsNullOrEmpty(stringValue))
            {
                throw new ArgumentException($"'{configurationKey}' must have a value");
            }
            return stringValue;
        }
        
        public static int GetValidatedIntValue(this IConfiguration conf, string configurationKey)
        {
            var intValue = conf.GetValue<int?>(configurationKey);
            if (!intValue.HasValue)
            {
                throw new ArgumentException($"'{configurationKey}' must have a value");
            }
            return intValue.Value;
        }
    }
}
