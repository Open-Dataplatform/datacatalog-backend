using System;

namespace DataCatalog.Common.Data
{
    public abstract class Entity : Created
    {
        public Guid Id { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}