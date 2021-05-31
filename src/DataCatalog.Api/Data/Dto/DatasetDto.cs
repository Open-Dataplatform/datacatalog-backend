using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using DataCatalog.Common.Interfaces;
using System;

namespace DataCatalog.Api.Data.Dto
{
    public class DatasetCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SlaDescription { get; set; }
        public string SlaLink { get; set; }
        public string Owner { get; set; }
        public DatasetStatus Status { get; set; }
        public Confidentiality Confidentiality { get; set; }
        public RefinementLevel RefinementLevel { get; set; }
        public string Location { get; set; }

        public GuidId Contact { get; set; }
        public GuidId Hierarchy { get; set; }
        public GuidId[] Categories { get; set; }
        public GuidId[] DataSources { get; set; }

        public DurationUpsertRequest Frequency { get; set; }
        public DurationUpsertRequest Resolution { get; set; }
        public SourceTransformationUpsertRequest SourceTransformation { get; set; }
        public DataFieldUpsertRequest[] DataFields { get; set; }
    }

    public class DatasetUpdateRequest : DatasetCreateRequest, IUpdateRequest
    {
        public Guid Id { get; set; }
    }

    public class DatasetResponse : ReplicantEntity
    {
        public Guid? MemberId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string SlaDescription { get; set; }
        public string SlaLink { get; set; }
        public string Owner { get; set; }
        public DatasetStatus Status { get; set; }
        public Confidentiality Confidentiality { get; set; }
        public RefinementLevel RefinementLevel { get; set; }
        public string Location { get; set; }

        public MemberGroupResponse Contact { get; set; }
        public DurationResponse Frequency { get; set; }
        public DurationResponse Resolution { get; set; }
        public TransformationResponse SourceTransformation { get; set; }
        public HierarchyResponse Hierarchy { get; set; }

        public DataFieldResponse[] DataFields { get; set; }        
        public CategoryResponse[] Categories { get; set; }                
        public DatasetChangeLogResponse[] DatasetChangeLogs { get; set; }
        public DataSourceResponse[] DataSources { get; set; }

        public ProvisionDatasetStatusEnum ProvisionStatus { get; set; }
    }
}