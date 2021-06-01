using DataCatalog.Common.Data;

namespace DataCatalog.Api.Data.Dto
{
    public class DatasetLocationRequest
    {
        public string Name { get; set; }
        public NullableGuidId Hierarchy { get; set; }
    }

    public class DatasetLocationResponse
    {
        public string Location { get; set; }
    }
}