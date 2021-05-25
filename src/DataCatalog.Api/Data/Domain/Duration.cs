using System;

namespace DataCatalog.Api.Data.Domain
{
    public class Duration
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
