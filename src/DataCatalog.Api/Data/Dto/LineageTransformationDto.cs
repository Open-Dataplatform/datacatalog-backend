using DataCatalog.Common.Data;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Dto
{
    public class LineageTransformationResponse : Entity
    {
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public List<LineageDatasetResponse> Datasets { get; set; } = new List<LineageDatasetResponse>();
    }
}