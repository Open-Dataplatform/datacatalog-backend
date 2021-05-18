using DataCatalog.Api.Data.Common;
using DataCatalog.Api.Enums;
using System;

namespace DataCatalog.Api.Data.Model
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