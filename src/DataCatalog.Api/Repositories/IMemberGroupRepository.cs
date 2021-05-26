using DataCatalog.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public interface IMemberGroupRepository
    {
        Task<IEnumerable<MemberGroup>> ListAsync();
        Task AddAsync(MemberGroup memberGroup);
        Task<MemberGroup> FindByIdAsync(Guid id);
        Task<IEnumerable<MemberGroup>> GetMemberGroups(Guid memberId);
        Task AddMemberAsync(Guid memberGroupId, Guid memberId);
        Task RemoveMemberAsync(Guid memberGroupId, Guid memberId);
        void Update(MemberGroup memberGroup);
        void Remove(MemberGroup memberGroup);
    }
}
