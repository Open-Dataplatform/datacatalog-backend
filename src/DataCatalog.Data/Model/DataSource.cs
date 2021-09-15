using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using System.Collections.Generic;

namespace DataCatalog.Data.Model
{
    public class DataSource: ReplicantEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactInfo { get; set; }
        public SourceType SourceType { get; set; }
    }
}