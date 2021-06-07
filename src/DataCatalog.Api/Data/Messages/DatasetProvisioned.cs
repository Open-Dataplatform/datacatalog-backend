using System;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Data.Messages
{
    public class DatasetProvisioned
    {
        public Guid DatasetId { get; set; }
        public ProvisionDatasetStatusEnum Status { get; set; }
        public string Error { get; set; }
    }
}
