using DataCatalog.Common.Data;
using System;

namespace DataCatalog.Data.Model
{
    public class DatasetGroupDataset : Created
    {
        public Guid DatasetGroupId { get; set; }
        public Guid DatasetId { get; set; }

        public DatasetGroup DatasetGroup { get; set; }
        public Dataset Dataset { get; set; }
    }
}