using System;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Dto
{
    public class SPDatasetAccessListDto
    {
        public Guid DatasetId { get; set; }
        public IEnumerable<DataAccessEntry> ReadAccessList { get; set; }
        public IEnumerable<DataAccessEntry> WriteAccessList { get; set; }
    }
}