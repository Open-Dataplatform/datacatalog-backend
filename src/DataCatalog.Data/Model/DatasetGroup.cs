using DataCatalog.Common.Data;
using System.Collections.Generic;

namespace DataCatalog.Data.Model
{
    public class DatasetGroup : MemberEntity
    {        
        public string Name { get; set; }
        public string Description { get; set; }

        public Member Member { get; set; }

        public List<DatasetGroupDataset> DatasetGroupDatasets { get; set; } = new List<DatasetGroupDataset>();
    }
}