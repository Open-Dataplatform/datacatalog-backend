using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory
{
    public interface ITokenProvider
    {
        Task<AuthenticationResult> GetTokenAsync(IEnumerable<string> scopes, CancellationToken cancellationToken);
    }
}