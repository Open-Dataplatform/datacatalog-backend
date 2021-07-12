using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.DatasetResourceManagement.Commands.AccessControlList;
using DataCatalog.DatasetResourceManagement.Commands.Group;
using DataCatalog.DatasetResourceManagement.Common;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.Storage;

namespace DataCatalog.DatasetResourceManagement.Services.ActiveDirectory
{
    public class AzureActiveDirectoryRootGroupProvider : IActiveDirectoryRootGroupProvider
    {
        private readonly IActiveDirectoryGroupService _activeDirectoryGroupService;
        private readonly IAccessControlListService _accessControlListService;
        private readonly IStorageService _storageService;

        public AzureActiveDirectoryRootGroupProvider(
            IActiveDirectoryGroupService activeDirectoryGroupService, 
            IAccessControlListService accessControlListService,
            IStorageService storageService)
        {
            _activeDirectoryGroupService = activeDirectoryGroupService ?? throw new ArgumentNullException(nameof(activeDirectoryGroupService));
            _accessControlListService = accessControlListService ?? throw new ArgumentNullException(nameof(accessControlListService));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task<string> ProvideGroupAsync(string displayName, string description, string leaseContainer)
        {
            var group = await _activeDirectoryGroupService.GetGroupAsync($"{GroupConstants.SecurityGroupPrefix}{displayName}");

            if (group == null)
            {
                string groupId;
                await using (await _storageService.AcquireLeaseAsync(leaseContainer))
                {
                    groupId = await CreateGroup(displayName, description);
                    
                    await UpdateGroupAcl(leaseContainer, groupId);
                }

                return groupId;
            }

            if (!await _accessControlListService.IsGroupInAccessControlListAsync(group.Id, leaseContainer, "/"))
                await UpdateGroupAcl(leaseContainer, group.Id);

            return group.Id;
        }

        private async Task UpdateGroupAcl(string container, string groupId)
        {
            await _accessControlListService.SetGroupsInAccessControlListAsync(new CreateGroupsInAccessControlList
            {
                StorageContainer = container,
                Path = "/",
                GroupEntries = new List<AccessControlListGroupEntry>
                {
                    new AccessControlListGroupEntry {Id = groupId, Permissions = "r-x"},
                    new AccessControlListGroupEntry {Id = groupId, Permissions = "r-x", IsDefault = true}
                }
            });
        }

        private async Task<string> CreateGroup(string displayName, string description)
        {
            var groupResponse = await _activeDirectoryGroupService.CreateGroupAsync(
                new CreateGroup
                {
                    Description = description,
                    DisplayName = displayName,
                    MailNickname = "dataplatform"
                });

            if (groupResponse == null) throw new NullReferenceException($"Failed creating group with displayname {displayName}");

            return groupResponse.Id;
        }
    }
}
