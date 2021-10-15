using System;

namespace DataCatalog.Api.Services.Egress
{
    /// <summary>
    /// Indicates that the egress system has not been configured for the given guid
    /// </summary>
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message): base(message)
        {
            
        }
        
    }
}