using System;

namespace DataCatalog.Api.Services.Egress
{
    public class GenericEgressException : Exception
    {
        public GenericEgressException(string message): base(message)
        {
            
        }
    }
}