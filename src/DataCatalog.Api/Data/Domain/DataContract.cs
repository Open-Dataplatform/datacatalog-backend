using System;

namespace DataCatalog.Api.Data.Domain
{
    public class DataContract
    {
        public Guid Id { get; set; }
        public Guid DatasetId { get; set; }
        public Guid DataSourceId { get; set; }
        public Dataset Dataset { get; set; }
        public DataSource DataSource { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
