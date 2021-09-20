using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Common.Data;
using DataCatalog.Data;
using DataCatalog.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace DataCatalog.Api.Repositories
{
    public class ServiceLevelAgreementRepository : BaseRepository, IServiceLevelAgreementRepository
    {
        public ServiceLevelAgreementRepository(DataCatalogContext context, Current current) : base(context, current)
        {
        }

        public async Task<IEnumerable<ServiceLevelAgreement>> ListAsync()
        {
            return await _context.ServiceLevelAgreement.ToListAsync();
        }
    }
}