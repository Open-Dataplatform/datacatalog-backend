using System;
using DataCatalog.Common.Messages;

namespace DataCatalog.DatasetResourceManagement.Messages
{
    public class DatasetProvisionedMessage : MessageBase
    {
        public Guid DatasetId { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
    }
}
