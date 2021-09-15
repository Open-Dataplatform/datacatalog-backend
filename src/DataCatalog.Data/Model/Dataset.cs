using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using System;
using System.Collections.Generic;

namespace DataCatalog.Data.Model
{
    public class Dataset : ReplicantEntity
    {
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

        public List<DataField> DataFields { get; set; } = new List<DataField>();
        public List<DatasetCategory> DatasetCategories { get; set; } = new List<DatasetCategory>();
        public List<TransformationDataset> TransformationDatasets { get; set; } = new List<TransformationDataset>();
        public List<DatasetChangeLog> DatasetChangeLogs { get; set; } = new List<DatasetChangeLog>();
        public List<DatasetDuration> DatasetDurations { get; set; } = new List<DatasetDuration>();
        public List<DataContract> DataContracts { get; set; } = new List<DataContract>();

        public ProvisionDatasetStatusEnum? ProvisionStatus { get; set; }
        public bool IsDeleted { get; set; }

        public Guid? ServiceLevelAgreementId { get; set; }
        public ServiceLevelAgreement ServiceLevelAgreement { get; set; }
    }
}