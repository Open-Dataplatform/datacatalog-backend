using System;
using DataCatalog.Api.Enums;

namespace DataCatalog.Api.Data.Domain
{
    public class DataSource
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactInfo { get; set; }
        public SourceType SourceType { get; set; }
        public Guid Id { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
