using System;
using DataCatalog.Api.Enums;

namespace DataCatalog.Api.Data.Domain
{
    public class DatasetProvisioned
    {
        public Guid DatasetId { get; set; }
        public ProvisionDatasetStatusEnum Status { get; set; }
        public string Error { get; set; }
    }
}
