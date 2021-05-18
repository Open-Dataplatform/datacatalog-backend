using System;

namespace DataCatalog.Api.Data.Domain
{
    public class DatasetCategory
    {
        public Guid DatasetId { get; set; }
        public Guid CategoryId { get; set; }

        public Dataset Dataset { get; set; }
        public Category Category { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
