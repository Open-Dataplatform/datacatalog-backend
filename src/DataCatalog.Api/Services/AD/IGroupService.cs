using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Services.AD
{
    public interface IGroupService
    {
        Task<IEnumerable<AccessMember>> GetGroupMembersAsync(string groupId);
        Task<AccessMember> GetAccessMemberAsync(string groupId);
        Task RemoveGroupMemberAsync(Guid datasetId, string groupId, string memberId, AccessType accessType);
        Task<AccessMember> AddGroupMemberAsync(Guid datasetId, string groupId, string memberId, AccessType accessType);
        Task<IEnumerable<AdSearchResult>> SearchAsync(string searchString);
    }
}
