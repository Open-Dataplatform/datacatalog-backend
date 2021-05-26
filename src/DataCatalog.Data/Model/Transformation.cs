using DataCatalog.Common.Data;
using System.Collections.Generic;

namespace DataCatalog.Data.Model
{
    public class Transformation : Entity
    {
        public string ShortDescription { get; set; } = "";
        public string Description { get; set; } = "";

        public List<TransformationDataset> TransformationDatasets { get; set; } = new List<TransformationDataset>();
    }
}