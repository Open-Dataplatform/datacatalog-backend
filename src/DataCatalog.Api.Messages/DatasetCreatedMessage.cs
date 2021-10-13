using System;
using DataCatalog.Common.Messages;

namespace DataCatalog.Api.Messages
{
    public class DatasetCreatedMessage : MessageBase
    {
        public Guid DatasetId { get; set; }
        public string DatasetName { get; set; }
        public string Owner { get; set; }
        public bool AddAllUsersGroup { get; set; }
    }
}
