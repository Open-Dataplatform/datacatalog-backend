using System;
using DataCatalog.Api.Enums;

namespace DataCatalog.Api.Data.Domain
{
    public class DatasetDuration
    {
        public DurationType DurationType { get; set; }
        public Guid DatasetId { get; set; }
        public Guid DurationId { get; set; }

        public Dataset Dataset { get; set; }
        public Duration Duration { get; set; }
    }
}
