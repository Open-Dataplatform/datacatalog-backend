
using DataCatalog.Data;
using DataCatalog.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public class MemberRepository : BaseRepository, IMemberRepository
    {
        public MemberRepository(DataCatalogContext context) : base(context)
        { }

        public async Task AddAsync(Member member)
        {
            await _context.Members.AddAsync(member);
        }

        public async Task<Member> FindByExternalIdAsync(string externalId, Guid identityProviderId)
        {
            return await _context.Members.SingleOrDefaultAsync(a => a.ExternalId == externalId && a.IdentityProviderId == identityProviderId);
        }

        public async Task<Member> FindByIdAsync(Guid id)
        {
            return await _context.Members.SingleOrDefaultAsync(a => a.Id == id);
        }
    }
}
