using System;
using System.Threading.Tasks;

namespace DataCatalog.Api.Services.Egress
{
    public interface IEgressService
    {
        public Task<object> FetchData(Guid datasetId, string fromDate, string toDate, string authorizationHeader);
    }
}