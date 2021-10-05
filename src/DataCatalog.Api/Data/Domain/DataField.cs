using System;

namespace DataCatalog.Api.Data.Domain
{
    public class DataField
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public string Format { get; set; }
        public string Validation { get; set; }
        public int SortingKey { get; set; }
        public Dataset Dataset { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
    }
}
