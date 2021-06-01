using DataCatalog.Common.Data;

namespace DataCatalog.Api.Data.Dto
{
    public class DataFieldUpsertRequest : NullableGuidId
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Format { get; set; }
        public string Validation { get; set; }
    }

    public class DataFieldResponse : Entity
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Format { get; set; }
        public string Validation { get; set; }
    }
}