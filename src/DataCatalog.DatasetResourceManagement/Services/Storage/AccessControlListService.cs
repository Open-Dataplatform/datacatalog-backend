using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using DataCatalog.DatasetResourceManagement.Commands.AccessControlList;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.Storage;

namespace DataCatalog.DatasetResourceManagement.Services.Storage
{
    public class AccessControlListService : IAccessControlListService
    {
        private readonly DataLakeServiceClient _dataLakeServiceClient;
        
        public AccessControlListService(DataLakeServiceClient dataLakeServiceClient)
        {
            _dataLakeServiceClient = dataLakeServiceClient ?? throw new ArgumentNullException(nameof(dataLakeServiceClient));
        }

        public async Task RemoveGroupFromAccessControlListAsync(string storageContainer, string path, string groupId, string leaseId)
        {
            var fileSystemClient = _dataLakeServiceClient.GetFileSystemClient(storageContainer.ToLower());
            var directoryClient = fileSystemClient.GetDirectoryClient(path);

            var accessControlList = await directoryClient.GetAccessControlAsync();

            var newAccessControlList =
                accessControlList.Value.AccessControlList.Where(x => !Equals(x.EntityId, groupId));

            await directoryClient.SetAccessControlListAsync(newAccessControlList.ToList(),conditions:new DataLakeRequestConditions {LeaseId = leaseId });
        }

        public Task SetGroupsInAccessControlListAsync(CreateGroupsInAccessControlList createGroupsInAccessControlList, string leaseId = null)
        {
            var fileSystemClient = _dataLakeServiceClient.GetFileSystemClient(createGroupsInAccessControlList.StorageContainer.ToLower());
            var directoryClient = fileSystemClient.GetDirectoryClient(createGroupsInAccessControlList.Path);

            var entries = createGroupsInAccessControlList.GroupEntries.Select(x => new PathAccessControlItem
            {
                AccessControlType = AccessControlType.Group, 
                EntityId = x.Id, 
                DefaultScope = x.IsDefault,
                Permissions = PathAccessControlExtensions.ParseSymbolicRolePermissions(x.Permissions)
            }).ToList();
            
            entries.Add(new PathAccessControlItem
            {
                AccessControlType = AccessControlType.Mask,
                DefaultScope = true,
                Permissions = PathAccessControlExtensions.ParseSymbolicRolePermissions("rwx")
            });

            entries.Add(new PathAccessControlItem
            {
                AccessControlType = AccessControlType.Mask,
                Permissions = PathAccessControlExtensions.ParseSymbolicRolePermissions("rwx")
            });

            return leaseId != null
                ? directoryClient.SetAccessControlListAsync(entries, null, null,
                    new DataLakeRequestConditions {LeaseId = leaseId})
                : directoryClient.SetAccessControlListAsync(entries);
        }

        private async Task<IEnumerable<string>> GetGroupsInAclAsync(string storageContainer, string directory)
        {
            var fileSystemClient = _dataLakeServiceClient.GetFileSystemClient(storageContainer.ToLower());
            var directoryClient = fileSystemClient.GetDirectoryClient(directory);

            var acl = await directoryClient.GetAccessControlAsync();

            return acl.Value.AccessControlList
                .Where(x => x.AccessControlType == AccessControlType.Group && x.EntityId != null)
                .Select(x => x.EntityId);
        }

        public async Task<bool> IsGroupInAccessControlListAsync(string groupId, string storageContainer, string directory)
        {
            var groupsInAcl = await GetGroupsInAclAsync(storageContainer, directory);
            
            return groupsInAcl.Contains(groupId);
        }
    }
}
