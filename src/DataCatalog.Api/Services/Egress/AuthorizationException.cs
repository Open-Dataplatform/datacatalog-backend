using System;

namespace DataCatalog.Api.Services.Egress
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(string message) : base(message)
        { }
    }
}