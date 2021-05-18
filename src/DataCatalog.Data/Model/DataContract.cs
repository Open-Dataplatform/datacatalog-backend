using DataCatalog.Api.Data.Common;
using System;

namespace DataCatalog.Api.Data.Model
{
    public class DataContract: ReplicantEntity
    {
        public Guid DatasetId { get; set; }
        public Guid DataSourceId { get; set; }

        public Dataset Dataset { get; set; }
        public DataSource DataSource { get; set; }
    }
}