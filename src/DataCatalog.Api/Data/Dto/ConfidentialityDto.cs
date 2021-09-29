using DataCatalog.Common.Data;

namespace DataCatalog.Api.Data.Dto
{
    public class ConfidentialityResponse : GuidId
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}