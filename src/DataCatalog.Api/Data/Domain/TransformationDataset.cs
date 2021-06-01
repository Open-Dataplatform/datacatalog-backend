using System;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Data.Domain
{
    public class TransformationDataset
    {
        public TransformationDirection TransformationDirection { get; set; }

        public Guid DatasetId { get; set; }
        public Guid TransformationId { get; set; }

        public Dataset Dataset { get; set; }
        public Transformation Transformation { get; set; }
    }
}
