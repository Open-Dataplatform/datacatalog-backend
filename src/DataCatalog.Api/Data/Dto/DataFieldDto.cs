using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Data.Dto
{
    public class DataFieldUpsertRequest : NullableGuidId
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Format { get; set; }
        public string Validation { get; set; }
        public DataFieldUnit? Unit { get; set; }
        public int SortingKey { get; set; }
    }

    public class DataFieldResponse : GuidId
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Format { get; set; }
        public string Validation { get; set; }
        public string Unit { get; set; }
        public int SortingKey { get; set; }
    }
}