using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DataCatalog.DatasetResourceManagement.Commands.Group;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.Responses.Group;
using DataCatalog.DatasetResourceManagement.Services.ActiveDirectory.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace DataCatalog.DatasetResourceManagement.Services.ActiveDirectory
{
    public class AzureActiveDirectoryGroupService : IActiveDirectoryGroupService
    {
        private readonly HttpClient _httpClient;
        private readonly IGraphServiceClient _graphServiceClient;
        private readonly ILogger<AzureActiveDirectoryGroupService> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public AzureActiveDirectoryGroupService(
            HttpClient httpClient,
            ILogger<AzureActiveDirectoryGroupService> logger, 
            IGraphServiceClient graphServiceClient)
        {
            _httpClient = httpClient;
            _logger = logger;
            _graphServiceClient = graphServiceClient;
            _retryPolicy = Policy
                .Handle<ServiceException>()
                .RetryAsync(100, (e, r) => Task.Delay(500));
        }

        public async Task<CreateGroupResponse> CreateGroupAsync(CreateGroup createGroup)
        {
            var httpRequest = new CreateGroupRequestDto
            {
                DisplayName = createGroup.DisplayName,
                Description = createGroup.Description,
                MailNickname = createGroup.MailNickname,
                Owners = createGroup.Owners,
                Members = createGroup.Members
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "v1/Groups")
            {
                Content = new StringContent(JsonConvert.SerializeObject(httpRequest))
            };

            try
            {
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error creating group {GroupName} with error message {ErrorMessage}", createGroup.DisplayName, content);
                    return null;
                }

                var createGroupResponse =
                    JsonConvert.DeserializeObject<CreateGroupResponseDto>(await response.Content.ReadAsStringAsync());

                // Ensure group exists before returning
                try
                {
                    await _retryPolicy.ExecuteAsync(async () => await
                        _graphServiceClient.Groups[createGroupResponse.GroupId].Request().GetAsync());
                }
                catch (ServiceException e)
                {
                    _logger.LogError(e, "ServiceException caught when creating AD group {GroupName}", createGroup.DisplayName);
                    throw;
                }

                return new CreateGroupResponse {Id = createGroupResponse.GroupId};
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error sending request for group {GroupName}", createGroup.DisplayName);
                throw;
            }
        }

        public async Task AddGroupMember(string groupId, string memberId)
        {
            var members = await _graphServiceClient.Groups[groupId].Members.Request().GetAsync();
            
            if (members.Any(x => Equals(x.Id, memberId))) return;
            
            await _graphServiceClient.Groups[groupId].Members.References.Request()
                .AddAsync(new DirectoryObject {Id = memberId});
        }

        public async Task<GetGroupResponse> GetGroupAsync(string displayName)
        {
            var groups = await _graphServiceClient.Groups.Request().Filter($"displayName eq '{displayName}'").GetAsync();
            
            var group = groups?.FirstOrDefault();

            if (group == null) return null;

            return new GetGroupResponse {DisplayName = group.DisplayName, Id = group.Id};
        }
    }
}
