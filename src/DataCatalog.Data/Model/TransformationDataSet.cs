using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using System;

namespace DataCatalog.Data.Model
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