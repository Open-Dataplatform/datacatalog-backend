using DataCatalog.Api.Data.Dto;

namespace DataCatalog.Api.Data.Domain
{
    public class DatasetCreated : MessageBusPublishMessage
    {
        public string DatasetId { get; set; }
        public string DatasetName { get; set; }
        public string Container { get; set; }
        public string Hierarchy { get; set; }
        public string Owner { get; set; }
        public bool Public { get; set; }
    }
}
