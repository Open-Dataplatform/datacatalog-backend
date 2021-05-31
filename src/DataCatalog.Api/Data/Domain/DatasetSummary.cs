using System;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Data.Domain
{
    public class DatasetSummary
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DatasetStatus Status { get; set; }
        public Confidentiality Confidentiality { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public Category[] Categories { get; set; }

    }
}
