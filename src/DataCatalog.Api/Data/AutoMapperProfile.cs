using AutoMapper;
using DataCatalog.Data.Model;
using DataCatalog.Api.Data.Dto;
using System.Linq;
using DataCatalog.Common.Enums;
using DataCatalog.Common.Extensions;
using DataCatalog.Common.Data;
using System;

namespace DataCatalog.Api.Data
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            //Category
            CreateMap<Domain.AdSearchResult, AdSearchResultResponse>();
            CreateMap<Domain.AccessMember, DataAccessEntry>()
                .ForMember(a => a.MemberType, b => b.MapFrom(c => c.Type.EnumNameToDescription()));
            CreateMap<Category, CategoryResponse>();
            CreateMap<Domain.Category, CategoryResponse>();
            CreateMap<Category, CategoryColourResponse>();
            CreateMap<Category, Domain.Category>();
            CreateMap<CategoryResponse, GuidId>();
            CreateMap<CategoryCreateRequest, Domain.Category>();
            CreateMap<CategoryUpdateRequest, Domain.Category>();

            //Confidentiality
            CreateMap<Confidentiality, ConfidentialityResponse>();
            CreateMap<Confidentiality, GuidId>();

            //DataContract
            CreateMap<DataContract, DataContractResponse>()
                .ForMember(a => a.DatasetLocation, b => b.MapFrom(c => c.Dataset.Location))
                .ForMember(a => a.DatasetContainer, b => b.MapFrom(c => c.Dataset.RefinementLevel.EnumNameToDescription().ToLower()))
                .ForMember(a => a.DatasetStatus, b => b.MapFrom(c => c.Dataset.Status));
            CreateMap<DataContractCreateRequest, DataContract>();
            CreateMap<Domain.DataContract, DataContractResponse>();
            CreateMap<DataContract, Domain.DataContract>();
            CreateMap<Domain.DataContract, DataContract>();

            //DataField
            CreateMap<DataField, DataFieldResponse>();
            CreateMap<DataFieldUpsertRequest, DataField>().ForMember(a => a.Name, b => b.MapFrom(c => c.Name.FormatName()));
            CreateMap<DataFieldUpsertRequest, Domain.DataField>().ForMember(a => a.Name, b => b.MapFrom(c => c.Name.FormatName()));
            CreateMap<DataFieldResponse, DataFieldUpsertRequest>();
            CreateMap<DataField, Domain.DataField>();

            //Dataset
            CreateMap<Dataset, GuidId>();
            CreateMap<DatasetSummaryResponse, GuidId>();

            CreateMap<DatasetCategory, Domain.DatasetCategory>();
            CreateMap<Domain.DatasetCategory, DatasetCategory>();

            CreateMap<Dataset, DatasetSummaryResponse>()
                .ForMember(a => a.Categories, b => b.MapFrom(c => c.DatasetCategories.Select(d => d.Category)));

            CreateMap<Dataset, LineageDatasetResponse>()
                .ForMember(a => a.Categories, b => b.MapFrom(c => c.DatasetCategories.Select(d => d.Category)));

            CreateMap<Dataset, DatasetPredictiveSearchResponse>()
                .ForMember(a => a.Categories, b => b.MapFrom(c => c.DatasetCategories.Select(d => d.Category)));

            CreateMap<Dataset, DatasetResponse>()
                .ForMember(a => a.Categories, b => b.MapFrom(c => c.DatasetCategories.Select(d => d.Category)))
                .ForMember(a => a.Frequency, b => b.MapFrom(c =>
                    c.DatasetDurations.Where(d => d.DurationType == DurationType.Frequency).Select(d => d.Duration).FirstOrDefault()))
                .ForMember(a => a.Resolution, b => b.MapFrom(c =>
                    c.DatasetDurations.Where(d => d.DurationType == DurationType.Resolution).Select(d => d.Duration).FirstOrDefault()))
                .ForMember(a => a.SourceTransformation, b =>
                    b.MapFrom(c => c.TransformationDatasets.Where(c => c.TransformationDirection == TransformationDirection.Sink).Select(c => c.Transformation).FirstOrDefault()))
                .ForMember(a => a.DataSources, b => b.MapFrom(c => c.DataContracts.Select(d => d.DataSource)));

            CreateMap<Dataset, Domain.Dataset>();

            CreateMap<Domain.Dataset, DatasetSummaryResponse>()
                .ForMember(a => a.Categories, b => b.MapFrom(c => c.DatasetCategories.Select(e => e.Category)));
            CreateMap<Dataset, Domain.LineageDataset>()
                .ForMember(a => a.Categories, b => b.MapFrom(c => c.DatasetCategories.Select(e => e.Category)));

            CreateMap<DatasetCreateRequest, Dataset>()
                .ForMember(a => a.Name, b => b.MapFrom(c => c.Name.FormatName()))
                .ForMember(a => a.DatasetCategories, b =>
                    b.MapFrom(c => c.Categories.Select(d => new DatasetCategory { CategoryId = d.Id })))
                .ForMember(a => a.Contact, b => b.Ignore()).ForMember(a => a.ContactId, b => b.MapFrom(c => c.Contact.Id));

            CreateMap<DatasetCreateRequest, Domain.Dataset>()
                .ForMember(a => a.Name, b => b.MapFrom(c => c.Name.FormatName()))
                .ForMember(a => a.DatasetCategories, b =>
                    b.MapFrom(c => c.Categories.Select(d => new DatasetCategory { CategoryId = d.Id })))
                .ForMember(a => a.Contact, b => b.Ignore()).ForMember(a => a.ContactId, b => b.MapFrom(c => c.Contact.Id));

            CreateMap<DatasetUpdateRequest, Dataset>()
                .ForMember(a => a.Name, b => b.MapFrom(c => c.Name.FormatName()))
                .ForMember(a => a.DatasetCategories, b =>
                    b.MapFrom(c => c.Categories.Select(d => new DatasetCategory { CategoryId = d.Id })))
                .ForMember(a => a.Contact, b => b.Ignore()).ForMember(a => a.ContactId, b => b.MapFrom(c => c.Contact.Id));

            CreateMap<Domain.Dataset, DatasetResponse>()
                .ForMember(a => a.Categories, b => b.MapFrom(c => c.DatasetCategories.Select(d => d.Category)))
                .ForMember(a => a.Frequency, b => b.MapFrom(c =>
                    c.DatasetDurations.Where(d => d.DurationType == DurationType.Frequency).Select(d => d.Duration).FirstOrDefault()))
                .ForMember(a => a.Resolution, b => b.MapFrom(c =>
                    c.DatasetDurations.Where(d => d.DurationType == DurationType.Resolution).Select(d => d.Duration).FirstOrDefault()))
                .ForMember(a => a.SourceTransformation, b =>
                    b.MapFrom(c => c.TransformationDatasets.Where(c => c.TransformationDirection == TransformationDirection.Sink).Select(c => c.Transformation).FirstOrDefault()))
                .ForMember(a => a.DataSources, b => b.MapFrom(c => c.DataContracts.Select(d => d.DataSource)));
            CreateMap<Domain.Dataset, Dataset>();


            CreateMap<Domain.Dataset, Domain.LineageDataset>()
                .ForMember(a => a.Categories, b => b.MapFrom(c => c.DatasetCategories.Select(d => d.Category)));

            //DatasetChangeLog
            CreateMap<DatasetChangeLog, DatasetChangeLogResponse>();

            //DatasetGroup
            CreateMap<Domain.DatasetGroup, DatasetGroupResponse>()
                .ForMember(a => a.Datasets, b => b.MapFrom(c => c.DatasetGroupDatasets.Select(d => d.Dataset)));
            CreateMap<DatasetGroupCreateRequest, Domain.DatasetGroup>()
                .ForMember(a => a.DatasetGroupDatasets, b => b.MapFrom(c => c.Datasets.Select(d => new DatasetGroupDataset { DatasetId = d.Id })));
            CreateMap<DatasetGroupUpdateRequest, Domain.DatasetGroup>()
                .ForMember(a => a.DatasetGroupDatasets, b => b.MapFrom(c => c.Datasets.Select(d => new Domain.DatasetGroupDataset { DatasetId = d.Id })));
            CreateMap<DatasetGroup, Domain.DatasetGroup>();
            CreateMap<Domain.DatasetGroup, DatasetGroup>();
            CreateMap<DatasetGroupDataset, Domain.DatasetGroupDataset>();
            CreateMap<Domain.DatasetGroupDataset, DatasetGroupDataset>();

            //DataSource
            CreateMap<DataSource, DataSourceResponse>();
            CreateMap<Domain.DataSource, DataSourceResponse>();
            CreateMap<DataSource, Domain.DataSource>();
            CreateMap<DataSourceResponse, GuidId>();
            CreateMap<DataSourceCreateRequest, DataSource>();
            CreateMap<DataSourceUpdateRequest, DataSource>();

            //Duration
            CreateMap<Duration, DurationResponse>();
            CreateMap<DurationResponse, DurationUpsertRequest>();
            CreateMap<DurationUpsertRequest, Duration>().ForMember(a => a.Code, b => b.MapFrom(c => c.Code.ToUpper()));
            CreateMap<DurationUpsertRequest, Domain.Duration>().ForMember(a => a.Code, b => b.MapFrom(c => c.Code.ToUpper()));
            CreateMap<Domain.Duration, DurationResponse>();
            CreateMap<Duration, Domain.Duration>();
            CreateMap<Domain.Duration, Duration>();
            CreateMap<DurationCreateRequest, Duration>();

            // DatasetDuration
            CreateMap<DatasetDuration, Domain.DatasetDuration>();
            CreateMap<Domain.DatasetDuration, DatasetDuration>();

            //Member
            CreateMap<Member, GuidId>();
            CreateMap<Member, MemberResponse>();
            CreateMap<MemberCreateRequest, Member>();
            CreateMap<MemberUpdateRequest, Member>();
            CreateMap<Member, Domain.Member>();

            //MemberGroup
            CreateMap<MemberGroupResponse, GuidId>();
            CreateMap<MemberGroup, MemberGroupResponse>()
                .ForMember(a => a.Members, b => b.MapFrom(c => c.MemberGroupMembers.Select(d => d.Member)));

            CreateMap<MemberGroupCreateRequest, Data.Domain.MemberGroup>()
                .ForMember(a => a.MemberGroupMembers, b => b.MapFrom(c => c.Members.Select(d => new MemberGroupMember { MemberId = d.Id })));

            CreateMap<MemberGroupUpdateRequest, MemberGroup>()
                .ForMember(a => a.MemberGroupMembers, b => b.MapFrom(c => c.Members.Select(d => new MemberGroupMember { MemberId = d.Id })));
            CreateMap<Domain.MemberGroup, MemberGroupResponse>();
            CreateMap<MemberGroup, Domain.MemberGroup>();
            CreateMap<MemberGroupMember, Domain.MemberGroupMember>();
            CreateMap<Domain.Dataset, GuidId>();

            //MemberRole
            CreateMap<Role, MemberRoleResponse>();

            //Transformation
            CreateMap<TransformationResponse, SourceTransformationUpsertRequest>();
            CreateMap<Transformation, LineageTransformationResponse>();
            CreateMap<Domain.Transformation, Domain.LineageTransformation>();

            CreateMap<Transformation, TransformationResponse>()
                .ForMember(a => a.SinkDatasets, b => b.MapFrom(c =>  c.TransformationDatasets.Where(d => d.TransformationDirection == TransformationDirection.Sink).Select(e => e.Dataset)))
                .ForMember(a => a.SourceDatasets, b => b.MapFrom(c => c.TransformationDatasets.Where(d => d.TransformationDirection == TransformationDirection.Source).Select(e => e.Dataset)));
            CreateMap<Domain.Transformation, TransformationResponse>()
                .ForMember(a => a.SinkDatasets, b => b.MapFrom(c => c.TransformationDatasets.Where(d => d.TransformationDirection == TransformationDirection.Sink).Select(e => e.Dataset)))
                .ForMember(a => a.SourceDatasets, b => b.MapFrom(c => c.TransformationDatasets.Where(d => d.TransformationDirection == TransformationDirection.Source).Select(e => e.Dataset)));

            CreateMap<TransformationCreateRequest, Transformation>()
                .ForMember(a => a.TransformationDatasets, b => b.MapFrom(c => 
                    c.SinkDatasets.Select(d => new TransformationDataset { DatasetId = d.Id, TransformationDirection = TransformationDirection.Sink })
                    .Concat(c.SourceDatasets.Select(d => new TransformationDataset { DatasetId = d.Id, TransformationDirection = TransformationDirection.Source }))));

            CreateMap<TransformationUpdateRequest, Transformation>()
                .ForMember(a => a.TransformationDatasets, b => b.MapFrom(c =>
                    c.SinkDatasets.Select(d => new TransformationDataset { DatasetId = d.Id, TransformationDirection = TransformationDirection.Sink, TransformationId = c.Id })
                    .Concat(c.SourceDatasets.Select(d => new TransformationDataset { DatasetId = d.Id, TransformationDirection = TransformationDirection.Source, TransformationId = c.Id }))));

            CreateMap<SourceTransformationUpsertRequest, Transformation>();
            CreateMap<SourceTransformationUpsertRequest, Domain.Transformation>();
            CreateMap<Transformation, Domain.Transformation>();
            CreateMap<Domain.Transformation, Transformation>();
            CreateMap<Domain.TransformationDataset, TransformationDataset>();
            CreateMap<TransformationDataset, Domain.TransformationDataset>();

            // DatasetChangeLog
            CreateMap<DatasetChangeLog, Domain.DatasetChangeLog>();
            CreateMap<Domain.DatasetChangeLog, DatasetChangeLogResponse>()
                .ForMember(a => a.Member, b => b.MapFrom(c => 
                new MemberResponse
                {
                    Id = c.MemberId,
                    CreatedDate = c.CreatedDate,
                    MemberRole = c.Member.MemberRole,
                    Name = c.Name,
                    Email = c.Email
                }));


            // IdentityProvider
            CreateMap<IdentityProvider, Domain.IdentityProvider>();

            // DataField
            CreateMap<Domain.DataField, DataFieldResponse>();
            CreateMap<Domain.DataField, DataField>();

            // LineageDataset
            CreateMap<Domain.LineageDataset, LineageDatasetResponse>()
                .ForMember(a => a.SourceTransformations, b => b.MapFrom(c => c.SourceTransformations))
                .ForMember(a => a.SinkTransformations, b => b.MapFrom(c => c.SinkTransformations));
            CreateMap<Domain.LineageTransformation, LineageTransformationResponse>();

            //Enums
            CreateMapEnum<Role>();
            CreateMapEnum<DatasetStatus>();
            CreateMapEnum<Confidentiality>();
            CreateMapEnum<RefinementLevel>();
        }

        void CreateMapEnum<TEnum>()
        {
            CreateMap<TEnum, EnumResponse>()
                .ForMember(a => a.Id, b => b.MapFrom(c => Convert.ToInt32(c)))
                .ForMember(a => a.Description, b => b.MapFrom(c => c.EnumNameToDescription()));
        }
    }
}