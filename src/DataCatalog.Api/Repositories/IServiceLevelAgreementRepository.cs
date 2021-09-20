using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Data.Model;

namespace DataCatalog.Api.Repositories
{
    public interface IServiceLevelAgreementRepository
    {
        Task<IEnumerable<ServiceLevelAgreement>> ListAsync();
    }
}