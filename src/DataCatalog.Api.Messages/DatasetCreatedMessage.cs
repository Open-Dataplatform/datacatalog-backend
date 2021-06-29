using System;

namespace DataCatalog.Api.Messages
{
    public class DatasetCreated
    {
        public Guid DatasetId { get; set; }
        public string DatasetName { get; set; }
        public string Container { get; set; }
        public string Hierarchy { get; set; }
        public string Owner { get; set; }
        public bool Public { get; set; }
    }
}
