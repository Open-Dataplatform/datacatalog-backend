using DataCatalog.Api.Data.Common;
using DataCatalog.Api.Interfaces;
using System;

namespace DataCatalog.Api.Data.Dto
{
    public class TransformationCreateRequest
    {
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public GuidId[] SourceDatasets { get; set; }
        public GuidId[] SinkDatasets { get; set; }
    }

    public class TransformationUpdateRequest : GuidId, IUpdateRequest
    {
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public GuidId[] SourceDatasets { get; set; }
        public GuidId[] SinkDatasets { get; set; }
    }       

    public class TransformationResponse : Entity
    {
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public DatasetSummaryResponse[] SourceDatasets { get;set; }
        public DatasetSummaryResponse[] SinkDatasets { get; set; }
    }

    public class SourceTransformationUpsertRequest
    {
        public Guid? Id { get; set; }
        public string ShortDescription { get; set; } = "";
        public string Description { get; set; } = "";
        public GuidId[] SourceDatasets { get; set; }
    }
}