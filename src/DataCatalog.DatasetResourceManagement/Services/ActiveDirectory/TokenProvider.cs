using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory;
using Microsoft.Identity.Client;

namespace DataCatalog.DatasetResourceManagement.Services.ActiveDirectory
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IConfidentialClientApplication _confidentialClientApplication;

        public TokenProvider(IConfidentialClientApplication confidentialClientApplication)
        {
            _confidentialClientApplication = confidentialClientApplication ?? throw new ArgumentNullException(nameof(confidentialClientApplication));
        }

        public Task<AuthenticationResult> GetTokenAsync(IEnumerable<string> scopes, CancellationToken cancellationToken)
        {
            return _confidentialClientApplication
                .AcquireTokenForClient(scopes)
                .ExecuteAsync(cancellationToken);
        }
    }
}
