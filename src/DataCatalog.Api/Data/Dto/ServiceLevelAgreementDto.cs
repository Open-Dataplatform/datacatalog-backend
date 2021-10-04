using DataCatalog.Common.Data;

namespace DataCatalog.Api.Data.Dto
{
    public class ServiceLevelAgreementResponse : GuidId
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
    }
}