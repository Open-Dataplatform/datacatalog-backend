using System;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Domain
{
    public class DatasetGroup
    {
        public Guid MemberId { get; set; }
        public Guid? Id { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public Member Member { get; set; }
        public List<DatasetGroupDataset> DatasetGroupDatasets { get; set; } = new List<DatasetGroupDataset>();
    }
}
