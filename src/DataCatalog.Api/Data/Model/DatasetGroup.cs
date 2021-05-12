using DataCatalog.Api.Data.Common;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Model
{
    public class DatasetGroup : MemberEntity
    {        
        public string Name { get; set; }
        public string Description { get; set; }

        public Member Member { get; set; }

        public List<DatasetGroupDataset> DatasetGroupDatasets { get; set; } = new List<DatasetGroupDataset>();
    }
}