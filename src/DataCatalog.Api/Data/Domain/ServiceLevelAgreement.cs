using System;

namespace DataCatalog.Api.Data.Domain
{
    public class ServiceLevelAgreement
    {
        public Guid Id { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
    }
}