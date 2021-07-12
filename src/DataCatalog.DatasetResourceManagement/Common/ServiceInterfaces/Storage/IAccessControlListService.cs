using System.Threading.Tasks;
using DataCatalog.DatasetResourceManagement.Commands.AccessControlList;

namespace DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.Storage
{
    public interface IAccessControlListService
    {
        Task RemoveGroupFromAccessControlListAsync(string storageContainer, string path, string groupId, string leaseId);
        Task SetGroupsInAccessControlListAsync(CreateGroupsInAccessControlList createGroupsInAccessControlList, string leaseId = null);
        Task<bool> IsGroupInAccessControlListAsync(string groupId, string storageContainer, string directory);
    }
}