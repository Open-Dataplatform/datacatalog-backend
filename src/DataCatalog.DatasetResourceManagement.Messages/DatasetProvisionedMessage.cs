using System;

namespace DataCatalog.DatasetResourceManagement.Messages
{
    public class DatasetProvisionedMessage
    {
        public Guid DatasetId { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
    }
}
