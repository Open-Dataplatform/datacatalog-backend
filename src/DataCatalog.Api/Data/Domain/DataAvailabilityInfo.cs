using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCatalog.Api.Data.Domain
{
    public class DataAvailabilityInfo
    {
        public Guid DatasetId { get; set; }

        public DateTime FirstAvailableData { get; set; }
        public DateTime LatestAvailableData { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}