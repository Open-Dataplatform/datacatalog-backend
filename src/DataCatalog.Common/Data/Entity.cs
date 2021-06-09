using System;
using DataCatalog.Common.Interfaces;

namespace DataCatalog.Common.Data
{
    public abstract class Entity : Created, IGuidId
    {
        public Guid Id { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}