using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Common.Data;

namespace DataCatalog.Data.Model
{
    public class DataAvailabilityInfo : Entity
    {
        public DateTime FirstAvailableData { get; set; }
        public DateTime LatestAvailableData { get; set; }

        public Guid DatasetId { get; set; }
        public Dataset Dataset { get; set; }
    }
}