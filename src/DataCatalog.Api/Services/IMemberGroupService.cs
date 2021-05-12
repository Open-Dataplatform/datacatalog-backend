using DataCatalog.Api.Data.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Services
{
    public interface IMemberGroupService
    {
        Task<IEnumerable<MemberGroup>> ListAsync();
        Task<MemberGroup> FindByIdAsync(Guid id);
        Task<IEnumerable<MemberGroup>> GetMemberGroups(Guid memberId);
        Task SaveAsync(MemberGroup memberGroup);
        Task UpdateAsync(MemberGroup memberGroup);
        Task AddMemberAsync(Guid memberGroupId, Guid memberId);
        Task RemoveMemberAsync(Guid memberGroupId, Guid memberId);
        Task DeleteAsync(Guid id);
    }
}
