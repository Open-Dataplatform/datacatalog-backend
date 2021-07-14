using System.Threading.Tasks;
using DataCatalog.DatasetResourceManagement.Commands.Group;
using DataCatalog.DatasetResourceManagement.Responses.Group;

namespace DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory
{
    public interface IActiveDirectoryGroupService
    {
        Task<CreateGroupResponse> CreateGroupAsync(CreateGroup createGroup);
        Task<GetGroupResponse> GetGroupAsync(string displayName);
        Task AddGroupMember(string groupId, string memberId);
    }
}