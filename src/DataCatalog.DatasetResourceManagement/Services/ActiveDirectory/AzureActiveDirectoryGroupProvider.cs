using System;
using System.Threading.Tasks;
using DataCatalog.DatasetResourceManagement.Commands.Group;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory;

namespace DataCatalog.DatasetResourceManagement.Services.ActiveDirectory
{
    public class AzureActiveDirectoryGroupProvider : IActiveDirectoryGroupProvider
    {
        private readonly IActiveDirectoryGroupService _activeDirectoryGroupService;

        public AzureActiveDirectoryGroupProvider(IActiveDirectoryGroupService activeDirectoryGroupService)
        {
            _activeDirectoryGroupService = activeDirectoryGroupService ?? throw new ArgumentNullException(nameof(activeDirectoryGroupService));
        }

        public async Task<string> ProvideGroupAsync(string displayName, string description, string[] members = null)
        {
            var group = await _activeDirectoryGroupService.GetGroupAsync($"SEC-A-ENDK-{displayName}");

            if (group == null)
                return await CreateGroup(displayName, description, members);
            
            return group.Id;
        }

        private async Task<string> CreateGroup(string displayName, string description, string[] members = null)
        {
            var groupResponse = await _activeDirectoryGroupService.CreateGroupAsync(
                new CreateGroup
                {
                    Description = description,
                    DisplayName = displayName,
                    MailNickname = "dataplatform",
                    Members = members
                });

            if (groupResponse == null) throw new NullReferenceException($"Failed creating group with display name {displayName}");

            return groupResponse.Id;
        }
    }
}