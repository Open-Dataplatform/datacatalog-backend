using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.DatasetResourceManagement.Commands.AccessControlList;
using DataCatalog.DatasetResourceManagement.Commands.Group;
using DataCatalog.DatasetResourceManagement.Common;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.Storage;
using Microsoft.Extensions.Logging;

namespace DataCatalog.DatasetResourceManagement.Services.ActiveDirectory
{
    public class AzureActiveDirectoryRootGroupProvider : IActiveDirectoryRootGroupProvider
    {
        private readonly ILogger<AzureActiveDirectoryRootGroupProvider> _logger;
        private readonly IActiveDirectoryGroupService _activeDirectoryGroupService;
        private readonly IAccessControlListService _accessControlListService;
        private readonly IStorageService _storageService;

        public AzureActiveDirectoryRootGroupProvider(
            ILogger<AzureActiveDirectoryRootGroupProvider> logger,
            IActiveDirectoryGroupService activeDirectoryGroupService, 
            IAccessControlListService accessControlListService,
            IStorageService storageService)
        {
            _logger = logger;
            _activeDirectoryGroupService = activeDirectoryGroupService;
            _accessControlListService = accessControlListService;
            _storageService = storageService;
        }

        public async Task<string> ProvideGroupAsync(string displayName, string description, string leaseContainer)
        {
            var group = await _activeDirectoryGroupService.GetGroupAsync($"{Constants.SecurityGroupPrefix}{displayName}");

            if (group == null)
            {
                string groupId;
                await using (await _storageService.AcquireLeaseAsync(leaseContainer))
                {
                    groupId = await CreateGroup(displayName, description);
                    _logger.LogInformation("Updating group Access Control List for groupId {GroupId} for container {Container}", groupId, leaseContainer);
                    await UpdateGroupAcl(leaseContainer, groupId);
                }

                return groupId;
            }

            if (!await _accessControlListService.IsGroupInAccessControlListAsync(group.Id, leaseContainer, "/"))
            {
                _logger.LogInformation("Updating group Access Control List for groupId {GroupId} for container {Container}", group.Id, leaseContainer);
                await UpdateGroupAcl(leaseContainer, group.Id);
            }

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
                    new() {Id = groupId, Permissions = "r-x"},
                    new() {Id = groupId, Permissions = "r-x", IsDefault = true}
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

            if (groupResponse == null)
            {
                _logger.LogError("Could not create group");
                throw new NullReferenceException($"Failed creating group with displayname {displayName}");
            }

            _logger.LogInformation("Created group with name {GroupName} which got an Id of {GroupId}", displayName, groupResponse.Id);
            return groupResponse.Id;
        }
    }
}
