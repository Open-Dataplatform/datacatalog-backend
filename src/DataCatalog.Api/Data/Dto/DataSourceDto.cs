using DataCatalog.Api.Data.Common;
using DataCatalog.Api.Enums;
using DataCatalog.Api.Interfaces;
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

    public class DataSourceResponse : ReplicantEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactInfo { get; set; }
        public SourceType SourceType { get; set; }
    }
}