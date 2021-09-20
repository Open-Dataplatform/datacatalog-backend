using System;
using System.Collections.Generic;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Data.Domain
{
    public class Dataset
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string SlaDescription { get; set; }
        public string SlaLink { get; set; }
        public string Owner { get; set; }
        public DatasetStatus Status { get; set; }
        public Confidentiality Confidentiality { get; set; }
        
        public Guid ContactId { get; set; }
        public Guid SourceId { get; set; }
        
        public MemberGroup Contact { get; set; }

        public ProvisionDatasetStatusEnum ProvisionStatus { get; set; }

        public DataField[] DataFields { get; set; }
        public List<DatasetCategory> DatasetCategories { get; set; } = new List<DatasetCategory>();
        public List<TransformationDataset> TransformationDatasets { get; set; } = new List<TransformationDataset>();
        public List<DatasetChangeLog> DatasetChangeLogs { get; set; } = new List<DatasetChangeLog>();
        public List<DatasetDuration> DatasetDurations { get; set; } = new List<DatasetDuration>();
        public List<DataContract> DataContracts { get; set; } = new List<DataContract>();

        public Guid ServiceLevelAgreementId { get; set; }
        public ServiceLevelAgreement ServiceLevelAgreement { get; set; }
     }
}
