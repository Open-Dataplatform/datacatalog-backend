using DataCatalog.Common.Data;
using System;

namespace DataCatalog.Data.Model
{
    public class DatasetCategory : Created
    {
        public Guid DatasetId { get; set; }
        public Guid CategoryId { get; set; }

        public Dataset Dataset { get; set; }
        public Category Category { get; set; }
    }
}