using System.Collections.Generic;

namespace DataCatalog.Api.Data.Dto
{
    public class DatasetAccessListResponse
    {
        public IEnumerable<DataAccessEntry> ReadAccessList { get; set; }
        public IEnumerable<DataAccessEntry> WriteAccessList { get; set; }
    }
}
