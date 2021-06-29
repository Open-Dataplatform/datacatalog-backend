using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace DataCatalog.DatasetResourceManagement.Services.ActiveDirectory.Middleware
{
    public class GroupAuthenticationMiddleware : DelegatingHandler
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly ILogger _logger;
        private const string GroupScope = "https://aadprovisioner.energinet.dk/.default";

        public GroupAuthenticationMiddleware(
            ITokenProvider tokenProvider, 
            ILogger<GroupAuthenticationMiddleware> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AuthenticationResult authenticationResult;
            try
            {
                authenticationResult = await _tokenProvider.GetTokenAsync(new[] { GroupScope }, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting a token for the aad provisioner API");
                throw;
            }
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
