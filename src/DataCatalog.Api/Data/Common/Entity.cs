using System;

namespace DataCatalog.Api.Data.Common
{
    public abstract class Entity : Created
    {
        public Guid Id { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}