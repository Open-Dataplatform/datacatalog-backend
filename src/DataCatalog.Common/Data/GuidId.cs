using DataCatalog.Api.Interfaces;
using System;

namespace DataCatalog.Api.Data.Common
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