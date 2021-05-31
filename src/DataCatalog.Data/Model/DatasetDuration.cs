using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using System;

namespace DataCatalog.Data.Model
{
    public class DatasetDuration : Created
    {
        public DurationType DurationType { get; set; }
        public Guid DatasetId { get; set; }
        public Guid DurationId { get; set; }

        public Dataset Dataset { get; set; }
        public Duration Duration { get; set; }
    }
}