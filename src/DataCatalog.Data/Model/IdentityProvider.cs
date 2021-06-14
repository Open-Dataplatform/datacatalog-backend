using System;
using DataCatalog.Common.Data;

namespace DataCatalog.Data.Model
{
    public class IdentityProvider : GuidId
    {
        public string Name { get; set; }
        public string TenantId { get; set; }
    }
}
