using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCatalog.Api.Data.Dto
{
    public class DataAvailabilityInfoDto
    {
        public Guid DatasetId { get; set; }

        public DateTime FirstAvailableData { get; set; }
        public DateTime LatestAvailableData { get; set; }
    }
}