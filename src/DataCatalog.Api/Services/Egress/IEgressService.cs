using System;
using System.Threading.Tasks;
using DataCatalog.Common.Utils;

namespace DataCatalog.Api.Services.Egress
{
    public interface IEgressService
    {
        public Task<Either<object, Exception>> FetchData(Guid datasetId, string fromDate, string toDate, string authorizationHeader);
    }
}