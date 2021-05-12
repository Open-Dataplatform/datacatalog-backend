using DataCatalog.Api.Data.Common;
using DataCatalog.Api.Enums;
using System;

namespace DataCatalog.Api.Data.Model
{
    public class TransformationDataset : Created
    {
        public TransformationDirection TransformationDirection { get; set; }

        public Guid DatasetId { get; set; }
        public Guid TransformationId { get; set; }        

        public Dataset Dataset { get; set; }
        public Transformation Transformation { get; set; }
    }
}