using System;
using DataCatalog.Common.Data;

namespace DataCatalog.Api.Data.Dto
{
    public class DurationCreateRequest
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class DurationUpdateRequest : DurationCreateRequest
    {
        public Guid Id { get; set; }
    }

    public class DurationResponse: GuidId
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class DurationUpsertRequest : NullableGuidId
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}