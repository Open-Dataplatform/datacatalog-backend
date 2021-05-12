using System;

namespace DataCatalog.Api.Interfaces
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