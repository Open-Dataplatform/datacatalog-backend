using DataCatalog.Api.Data.Model;
using System;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public interface IMemberRepository
    {
        Task AddAsync(Member member);
        Task<Member> FindByExternalIdAsync(string externalId, Guid identityProviderId);
        Task<Member> FindByIdAsync(Guid id);
    }
}
