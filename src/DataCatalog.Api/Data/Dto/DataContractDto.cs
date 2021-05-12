using DataCatalog.Api.Data.Common;
using DataCatalog.Api.Enums;
using DataCatalog.Api.Interfaces;
using System;

namespace DataCatalog.Api.Data.Dto
{
    public class DataContractCreateRequest
    {
        public Guid DatasetId { get; set; }
        public Guid DataSourceId { get; set; }
    }

    public class DataContractUpdateRequest : DataContractCreateRequest, IUpdateRequest
    {
        public Guid Id { get; set; }
    }

    public class DataContractResponse : ReplicantEntity
    {
        public Guid DatasetId { get; set; }
        public Guid DataSourceId { get; set; }
        public string DatasetContainer { get; set; }
        public string DatasetLocation { get; set; }
        public DatasetStatus DatasetStatus { get; set; }
    }
}