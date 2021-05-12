using DataCatalog.Api.Data;
using DataCatalog.Api.Data.Common;
using DataCatalog.Api.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public class MemberGroupRepository : BaseRepository, IMemberGroupRepository
    {
        public MemberGroupRepository(DataCatalogContext context) : base(context)
        { }

        public async Task<IEnumerable<MemberGroup>> ListAsync()
        {
            return await _context.MemberGroups.Include(a => a.Datasets).ToListAsync();
        }
        public async Task AddAsync(MemberGroup memberGroup)
        {
            await _context.MemberGroups.AddAsync(memberGroup);
        }

        public async Task<MemberGroup> FindByIdAsync(Guid id)
        {
            return await _context.MemberGroups.Include(a => a.Datasets).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<MemberGroup>> GetMemberGroups(Guid memberId)
        {
            return await _context.MemberGroups
                .Include(a => a.Datasets)
                .Where(a => a.MemberGroupMembers.Any(b => b.MemberId == memberId))
                .ToListAsync();
        }

        public async Task AddMemberAsync(Guid memberGroupId, Guid memberId)
        {
            await _context.MemberGroupMembers.AddAsync(new MemberGroupMember
            {
                MemberGroupId = memberGroupId,
                MemberId = memberId,
                CreatedDate = DateTime.Now
            });
        }

        public async Task RemoveMemberAsync(Guid memberGroupId, Guid memberId)
        {
            var entry = await _context.MemberGroupMembers.SingleOrDefaultAsync(m => m.MemberGroupId == memberGroupId && m.MemberId == memberId);
            if (entry != null)
                _context.MemberGroupMembers.Remove(entry);
        }

        public void Update(MemberGroup memberGroup)
        {
            _context.MemberGroups.Update(memberGroup);
        }

        public void Remove(MemberGroup memberGroup)
        {
            _context.MemberGroups.Remove(memberGroup);
        }
    }
}
