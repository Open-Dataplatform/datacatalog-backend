using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using System;

namespace DataCatalog.Data.Model
{
    public class DataField : Entity
    {        
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Format { get; set; }
        public string Validation { get; set; }
        public string Unit { get; set; }
        public int SortingKey { get; set; }
        
        public Guid DatasetId { get; set; }
        public Dataset Dataset { get; set; }
    }
}