using System;

namespace DataCatalog.Common.Interfaces
{
    public interface IGuidId
    {
        public Guid Id { get; }
    }

    public interface INullableGuidId
    {
        public Guid? Id { get; }
    }
}