using System;

namespace DataCatalog.Api.Data.Domain
{
    public class Category
    {
        public string Name { get; set; }
        public string Colour { get; set; }
        public Uri ImageUri { get; set; }
        public Guid Id { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
