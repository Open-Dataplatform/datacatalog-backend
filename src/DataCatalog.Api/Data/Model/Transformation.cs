using DataCatalog.Api.Data.Common;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Model
{
    public class Transformation : Entity
    {
        public string ShortDescription { get; set; } = "";
        public string Description { get; set; } = "";

        public List<TransformationDataset> TransformationDatasets { get; set; } = new List<TransformationDataset>();
    }
}