using DataCatalog.Api.Data.Common;
using DataCatalog.Api.Enums;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Model
{
    public class DataSource: ReplicantEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactInfo { get; set; }
        public SourceType SourceType { get; set; }

        public List<DataContract> DataContracts { get; set; } = new List<DataContract>();
    }
}