using DataCatalog.Common.Data;
using DataCatalog.Common.Interfaces;
using System;

namespace DataCatalog.Api.Data.Dto
{
    public class DatasetGroupCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public GuidId[] Datasets { get; set; }
    }

    public class DatasetGroupUpdateRequest : GuidId, IUpdateRequest
    {
        public Guid MemberId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public GuidId[] Datasets { get; set; }
    }

    public class DatasetGroupResponse : Entity
    {
        public Guid? MemberId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DatasetSummaryResponse[] Datasets { get; set; }
    }
}