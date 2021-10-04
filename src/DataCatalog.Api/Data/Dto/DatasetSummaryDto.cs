using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Dto
{
    public class DatasetSummaryResponse : GuidId
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DatasetStatus Status { get; set; }
        public Confidentiality Confidentiality { get; set; }
        
        public CategoryResponse[] Categories { get; set; }
    }

    public class LineageDatasetResponse : DatasetSummaryResponse
    {
        public List<LineageTransformationResponse> SourceTransformations { get; set; } = new List<LineageTransformationResponse>();
        public List<LineageTransformationResponse> SinkTransformations { get; set; } = new List<LineageTransformationResponse>();
    }

    public class DatasetPredictiveSearchResponse : GuidId
    {
        public string Name { get; set; }
        public Confidentiality Confidentiality { get; set; }

        public CategoryColourResponse[] Categories { get; set; }
    }
}