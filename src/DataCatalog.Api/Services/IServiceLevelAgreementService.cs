using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;

namespace DataCatalog.Api.Services
{
    public interface IServiceLevelAgreementService
    {
        Task<IEnumerable<ServiceLevelAgreement>> ListAsync();
    }
}