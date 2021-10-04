using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using DataCatalog.Common.Interfaces;
using System;

namespace DataCatalog.Api.Data.Dto
{
    public class DataSourceCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactInfo { get; set; }
        public SourceType SourceType { get; set; }
    }

    public class DataSourceUpdateRequest : DataSourceCreateRequest, IUpdateRequest
    {
        public Guid Id { get; set; }
    }

    public class DataSourceResponse : GuidId
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactInfo { get; set; }
        public SourceType SourceType { get; set; }
    }
}