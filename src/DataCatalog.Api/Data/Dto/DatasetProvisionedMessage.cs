namespace DataCatalog.Api.Data.Dto
{
    public class DatasetProvisionedMessage : MessageBusReceivedMessage
    {
        public string DatasetId { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
    }
}
