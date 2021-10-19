using System;

namespace DataCatalog.Api.Services.Egress
{
    public class EgressAuthorizationException : Exception
    {
        public EgressAuthorizationException(string message) : base(message)
        { }
    }
}