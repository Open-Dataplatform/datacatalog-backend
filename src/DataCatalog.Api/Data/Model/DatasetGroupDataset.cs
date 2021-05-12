using DataCatalog.Api.Data.Common;
using System;

namespace DataCatalog.Api.Data.Model
{
    public class DatasetGroupDataset : Created
    {
        public Guid DatasetGroupId { get; set; }
        public Guid DatasetId { get; set; }

        public DatasetGroup DatasetGroup { get; set; }
        public Dataset Dataset { get; set; }
    }
}