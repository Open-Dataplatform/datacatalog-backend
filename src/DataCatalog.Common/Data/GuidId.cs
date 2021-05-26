using DataCatalog.Common.Interfaces;
using System;

namespace DataCatalog.Common.Data
{
    public class GuidId : IGuidId
    {
        public Guid Id { get; set; }
    }

    public class NullableGuidId
    {
        public Guid? Id { get; set; }
    }
} 