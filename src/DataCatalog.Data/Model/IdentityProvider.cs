using System;

namespace DataCatalog.Data.Model
{
    public class IdentityProvider
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TenantId { get; set; }
    }
}
