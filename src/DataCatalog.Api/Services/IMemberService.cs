using System;
using System.Threading.Tasks;

namespace DataCatalog.Api.Services
{
    public interface IMemberService
    {
        Task<Data.Domain.Member> GetOrCreateAsync(string externalId, Guid identityProviderId);
    }
}
