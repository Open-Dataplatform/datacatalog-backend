using System;

namespace DataCatalog.Api.Messages
{
    public class DatasetCreatedMessage
    {
        public Guid DatasetId { get; set; }
        public string DatasetName { get; set; }
        public string Owner { get; set; }
        public bool AddAllUsersGroup { get; set; }
    }
}
