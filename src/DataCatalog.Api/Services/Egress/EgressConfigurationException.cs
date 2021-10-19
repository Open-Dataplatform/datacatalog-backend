using System;

namespace DataCatalog.Api.Services.Egress
{
    /// <summary>
    /// Indicates that the egress system has not been configured for the given guid
    /// </summary>
    public class EgressConfigurationException : Exception
    {
        public EgressConfigurationException(string message): base(message)
        {
            
        }
        
    }
}