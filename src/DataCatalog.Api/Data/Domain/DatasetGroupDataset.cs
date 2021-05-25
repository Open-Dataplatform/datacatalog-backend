using System;

namespace DataCatalog.Api.Data.Domain
{
    public class DatasetGroupDataset
    {
        public DateTime CreatedDate { get; set; }

        public Guid DatasetGroupId { get; set; }
        public Guid DatasetId { get; set; }

        public DatasetGroup DatasetGroup { get; set; }
        public Dataset Dataset { get; set; }
    }
}
