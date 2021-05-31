using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Common.Data;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;
using DataCatalog.Api.Exceptions;
using DataCatalog.Api.MessageBus;
using DataCatalog.Api.Repositories;

namespace DataCatalog.Api.Services
{
    public class DatasetService : IDatasetService
    {
        private readonly IDatasetRepository _datasetRepository;
        private readonly IHierarchyRepository _hierarchyRepository;
        private readonly ITransformationRepository _transformationRepository;
        private readonly ITransformationDatasetRepository _transformationDatasetRepository;
        private readonly IDurationRepository _durationRepository;
        private readonly IDatasetDurationRepository _datasetDurationRepository;
        private readonly IDatasetChangeLogRepository _datasetChangeLogRepository;
        private readonly IDataSourceRepository _dataSourceRepository;
        private readonly IUnitIOfWork _unitOfWork;
        private readonly IMessageBusSender<DatasetCreated> _messageBusSender;
        private readonly Current _current;
        private readonly IMapper _mapper;

        public DatasetService(
            IDatasetRepository datasetRepository, 
            IHierarchyRepository hierarchyRepository, 
            ITransformationRepository transformationRepository, 
            ITransformationDatasetRepository transformationDatasetRepository, 
            IDatasetDurationRepository datasetDurationRepository, 
            IDurationRepository durationRepository, 
            IDatasetChangeLogRepository datasetChangeLogRepository, 
            IDataSourceRepository dataSourceRepository, 
            IMapper mapper, 
            IUnitIOfWork unitIOfWork, 
            IMessageBusSender<DatasetCreated> messageBusSender,
            Current current)
        {
            _datasetRepository = datasetRepository;
            _hierarchyRepository = hierarchyRepository;
            _transformationRepository = transformationRepository;
            _transformationDatasetRepository = transformationDatasetRepository;
            _durationRepository = durationRepository;
            _datasetDurationRepository = datasetDurationRepository;
            _datasetChangeLogRepository = datasetChangeLogRepository;
            _dataSourceRepository = dataSourceRepository;
            _unitOfWork = unitIOfWork;
            _messageBusSender = messageBusSender;
            _current = current;
            _mapper = mapper;
        }

        public async Task<Dataset> FindByIdAsync(Guid id)
        {
            var dataset = await _datasetRepository.FindByIdAsync(id);

            if (dataset != null)
                return _mapper.Map<Dataset>(dataset);

            return null;
        }

        public async Task<IEnumerable<Dataset>> GetAllSummariesAsync(bool onlyPublished)
        {
            var datasets = await _datasetRepository.ListSummariesAsync(onlyPublished);
            return datasets.Select(x => _mapper.Map<Dataset>(x));
        }

        public async Task<Dataset> SaveAsync(DatasetCreateRequest request)
        {
            await ValidateAsync(request);

            var dbDataset = _mapper.Map<DataCatalog.Data.Model.Dataset>(request);

            if (string.IsNullOrWhiteSpace(dbDataset.Location))
                dbDataset.Location = await GetDatasetLocation(request.Hierarchy.Id, request.Name);

            if (request.SourceTransformation?.SourceDatasets?.Any() == true)
                await InsertOrUpdateSourceTransformation(request.SourceTransformation, dbDataset);

            await InsertOrUpdateDuration(request.Frequency, dbDataset, DurationType.Frequency);
            await InsertOrUpdateDuration(request.Resolution, dbDataset, DurationType.Resolution);
            InsertOrUpdateDataContracts(dbDataset, request.DataSources);
            await AddChangeLog(dbDataset);
            dbDataset.ProvisionStatus = ProvisionDatasetStatusEnum.Pending;

            await _unitOfWork.CompleteAsync();

            var hierarchy = await _hierarchyRepository.FindByIdAsync(dbDataset.HierarchyId);
            await _messageBusSender.PublishAsync(new DatasetCreated
            {
                DatasetId = dbDataset.Id.ToString(),
                Container = request.RefinementLevel switch
                {
                    RefinementLevel.Raw => "RAW",
                    RefinementLevel.Stock => "STOCK",
                    _ => "REFINED"
                },
                DatasetName = dbDataset.Name,
                Owner = request.Owner,
                Hierarchy = $"{GetHierarchyName(hierarchy).ToLower()}",
                Public = request.Confidentiality == Confidentiality.Public
            }, "DatasetCreated");

            return _mapper.Map<Dataset>(dbDataset);
        }

        public async Task<Dataset> UpdateAsync(DatasetUpdateRequest request)
        {
            await ValidateAsync(request);

            var dbDataset = await _datasetRepository.FindByIdAsync(request.Id);
            _mapper.Map(request, dbDataset);

            if (string.IsNullOrWhiteSpace(dbDataset.Location))
                dbDataset.Location = await GetDatasetLocation(request.Hierarchy.Id, request.Name);

            if (request.SourceTransformation?.SourceDatasets?.Any() != true)
            {
                // Find sink dataset relation
                var sinkTransformationDataset = await GetSinkTransformationDataset(dbDataset);
                if (sinkTransformationDataset != null)
                {
                    // Delete sink dataset relation
                    var sourceTransformation = sinkTransformationDataset.Transformation;
                    sourceTransformation.TransformationDatasets.Remove(sinkTransformationDataset);
                    // If the transformation is no longer connected to any sink datasets, delete it
                    if (sourceTransformation.TransformationDatasets.All(a => a.TransformationDirection != TransformationDirection.Sink))
                        await DeleteSourceTransformationAsync(sourceTransformation);
                }
            }
            else
                await InsertOrUpdateSourceTransformation(request.SourceTransformation, dbDataset);

            await InsertOrUpdateDuration(request.Frequency, dbDataset, DurationType.Frequency);
            await InsertOrUpdateDuration(request.Resolution, dbDataset, DurationType.Resolution);
            InsertOrUpdateDataContracts(dbDataset, request.DataSources);
            await AddChangeLog(dbDataset);
            dbDataset.Version++;
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<Dataset>(dbDataset);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingDataset = await _datasetRepository.FindByIdAsync(id);

            if (existingDataset == null)
                return;

            _datasetRepository.Remove(existingDataset);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<Dataset>> GetDatasetByCategoryAsync(Guid categoryId, SortType sortType, int take, int pageSize, int pageIndex, bool filterUnpublished)
        {
            var datasets = await _datasetRepository.GetDatasetByCategoryAsync(categoryId, sortType, take, pageSize, pageIndex, filterUnpublished);
            return _mapper.Map<IEnumerable<Dataset>>(datasets);
        }

        public async Task<IEnumerable<Dataset>> GetDatasetsBySearchTermAsync(string searchTerm, SortType sortType, int take, int pageSize, int pageIndex, bool filterUnpublished)
        {
            var datasets = await _datasetRepository.GetDatasetsBySearchTermQueryAsync(searchTerm, sortType, take, pageSize, pageIndex, filterUnpublished);
            return _mapper.Map<IEnumerable<Dataset>>(datasets);
        }

        public async Task<string> GetDatasetLocationAsync(Guid? hierarchyId, string name)
        {
            return await GetDatasetLocation(hierarchyId, name);
        }

        public async Task<LineageDataset> GetDatasetLineageAsync(Guid id)
        {
            var dataset = await FindByIdAsync(id);

            //Get the data set itself, with the list of transformations.
            var lineage = _mapper.Map<LineageDataset>(dataset);

            //Get sink transformations.
            await GetLineageTransformations(lineage, TransformationDirection.Sink, 1);

            //Get source transformations.
            await GetLineageTransformations(lineage, TransformationDirection.Source);

            return lineage;
        }

        public async Task<Dataset> CopyDatasetInRawAsync(Guid id)
        {
            var dataset = await _datasetRepository.FindByIdAsync(id);

            if (dataset is not {RefinementLevel: RefinementLevel.Raw})
            {
                return null;
            }
            dataset.Id = Guid.NewGuid();
            dataset.CreatedDate = DateTime.Now;
            dataset.ModifiedDate = DateTime.Now;

            var promotedName = $"promoted_{dataset.Name}";
            var currentLocation = dataset.Name.ToLower().Replace(' ', '-');
            var newLocation = promotedName.ToLower().Replace(' ', '-');
            dataset.Location = dataset.Location.Replace(currentLocation, newLocation);
            dataset.Name = promotedName;
            dataset.DatasetChangeLogs = new List<DataCatalog.Data.Model.DatasetChangeLog>();
            dataset.RefinementLevel++;
            dataset.DataContracts = null;
            dataset.DataFields.ForEach(f =>
            {
                f.Id = Guid.NewGuid();
                f.DatasetId = dataset.Id;
            });
            dataset.DatasetCategories.ForEach(d => d.DatasetId = dataset.Id);
            dataset.DatasetDurations.ForEach(d => d.DatasetId = dataset.Id);

            return _mapper.Map<Dataset>(dataset);

        }

        public async Task HandleMessage(string messageJson)
        {
            var message = DatasetProvisionedDeserializer.Deserialize(messageJson);
            await _datasetRepository.UpdateProvisioningStatusAsync(message.DatasetId, message.Status);
            await _unitOfWork.CompleteAsync();
        }

        private async Task<string> GetDatasetLocation(Guid? hierarchyId, string datasetName)
        {
            var parentHierarchyName = "<parentHierarchy>";
            var hierarchyName = "<hierarchy>";
            var datasetNameOut = "<datasetName>";

            if (hierarchyId.HasValue)
            {
                var hierarchy = await _hierarchyRepository.FindByIdAsync(hierarchyId.Value);
                parentHierarchyName = GetLocationName(hierarchy.ParentHierarchy.Name);
                hierarchyName = GetLocationName(hierarchy.Name);
            }

            if (!string.IsNullOrWhiteSpace(datasetName)) 
                datasetNameOut = GetLocationName(datasetName);

            return $"{parentHierarchyName}/{hierarchyName}/{datasetNameOut}";
        }

        private string GetLocationName(string name)
        {
            name = Regex.Replace(name.ToLower(), @"\s+", "-");  // To lower and replace space, tab, newline with dash
            name = Regex.Replace(name, @"[^\w\.-]", "");        // Remove anything but alphanumeric, dot and dash
            name = Regex.Replace(name, "[-]{2,}", "-");         // Remove any double hyphens
            name = name.TrimEnd('-').TrimStart('-');            // Remove any leading or trailing hyphens
            return name;
        }

        private async Task InsertOrUpdateSourceTransformation(SourceTransformationUpsertRequest request, DataCatalog.Data.Model.Dataset dataset)
        {
            if (request.Id.HasValue)
            {
                // Find existing transformation, update descriptions and add dataset as sink.
                var sourceTransformation = await _transformationRepository.FindByIdAsync(request.Id.Value);
                if (sourceTransformation == null) throw new NotFoundException();
                _mapper.Map(request, sourceTransformation);
                var sinkRelationExists = sourceTransformation.TransformationDatasets
                    .Any(a => a.Dataset == dataset && a.TransformationDirection == TransformationDirection.Sink);
                if (!sinkRelationExists)
                    AddTransformationDataset(sourceTransformation, dataset.Id, TransformationDirection.Sink);
            }
            else
            {
                // Add new transformation and transformation relations
                var sourceTransformation = _mapper.Map<DataCatalog.Data.Model.Transformation>(request);
                AddTransformationDataset(sourceTransformation, dataset, TransformationDirection.Sink);
                foreach (var id in request.SourceDatasets)
                    AddTransformationDataset(sourceTransformation, id.Id, TransformationDirection.Source);
                await _transformationRepository.AddAsync(_mapper.Map<DataCatalog.Data.Model.Transformation>(sourceTransformation));
            }
        }

        private void AddTransformationDataset(DataCatalog.Data.Model.Transformation transformation, Guid datasetId, TransformationDirection transformationDirection)
        {
            transformation.TransformationDatasets.Add(new DataCatalog.Data.Model.TransformationDataset
            {
                DatasetId = datasetId,
                TransformationDirection = transformationDirection
            });
        }

        private void AddTransformationDataset(DataCatalog.Data.Model.Transformation transformation, DataCatalog.Data.Model.Dataset dataset, TransformationDirection transformationDirection)
        {
            transformation.TransformationDatasets.Add(new DataCatalog.Data.Model.TransformationDataset
            {
                Dataset = dataset,
                TransformationDirection = transformationDirection
            });
        }

        private async Task InsertOrUpdateDuration(DurationUpsertRequest request, DataCatalog.Data.Model.Dataset dataset, DurationType durationType)
        {
            //Get existing relation for the given duration type
            var datasetDuration = await _datasetDurationRepository.FindByDatasetAndTypeAsync(dataset.Id, durationType);

            var requestedCode = request?.Code?.ToUpper();
            if (datasetDuration?.Duration.Code != requestedCode)
            {
                //Remove existing relation
                if (datasetDuration != null)
                    _datasetDurationRepository.Remove(datasetDuration);

                //If a code is set, create new relation
                if (requestedCode != null)
                {
                    // Find existing duration
                    var duration = await _durationRepository.FindByCodeAsync(requestedCode);

                    // Also check if the same duration has just been added, but is not saved yet
                    duration ??= dataset.DatasetDurations.Where(a => a.Duration != null).Select(a => a.Duration).FirstOrDefault(a => a.Code == requestedCode);

                    // If no existing duration is found, map the request and create new
                    duration ??= _mapper.Map<DataCatalog.Data.Model.Duration>(request);

                    // Create new duration relation
                    dataset.DatasetDurations.Add(new DataCatalog.Data.Model.DatasetDuration
                    {
                        Dataset = dataset,
                        DurationType = durationType,
                        Duration = duration
                    });
                }
            }
        }

        private void InsertOrUpdateDataContracts(DataCatalog.Data.Model.Dataset dataset, GuidId[] requests)
        {
            if (requests != null)
            {
                var existing = dataset.DataContracts.ToArray();

                foreach (var r in requests)
                    if (existing.All(a => a.DataSourceId != r.Id))
                        dataset.DataContracts.Add(new DataCatalog.Data.Model.DataContract { DataSourceId = r.Id });

                foreach (var e in existing)
                    if (requests.All(a => a.Id != e.DataSourceId))
                        dataset.DataContracts.Remove(e);
            }
        }

        private async Task AddChangeLog(DataCatalog.Data.Model.Dataset dataset)
        {
            await _datasetChangeLogRepository.AddAsync(
                new DataCatalog.Data.Model.DatasetChangeLog
                {
                    Dataset = _mapper.Map<DataCatalog.Data.Model.Dataset>(dataset),
                    MemberId = _current.MemberId,
                    Name = _current.Name,
                    Email = _current.Email
                }
            );
        }

        private async Task<DataCatalog.Data.Model.TransformationDataset> GetSinkTransformationDataset(DataCatalog.Data.Model.Dataset dataset)
        {
            return await _transformationDatasetRepository.FindByDatasetIdAndDirectionAsync(dataset.Id, TransformationDirection.Sink);
        }

        private async Task DeleteSourceTransformationAsync(DataCatalog.Data.Model.Transformation sourceTransformation)
        {
            _transformationDatasetRepository.Remove(sourceTransformation.TransformationDatasets);
            await _transformationRepository.RemoveAsync(sourceTransformation);
        }

        private async Task GetLineageTransformations(LineageDataset lineage, TransformationDirection transformationDirection, int maxRelations = -1)
        {
            //Get transformations.
            var otherDirection = transformationDirection == TransformationDirection.Sink
                ? TransformationDirection.Source
                : TransformationDirection.Sink;
            var transformationDatasets = await _transformationDatasetRepository.FindAllTransformationDatasetsForDatasetIdAndDirectionAsync(lineage.Id, otherDirection);
            var transformations = _mapper.Map<IEnumerable<TransformationDataset>>(transformationDatasets).Select(a => a.Transformation).ToArray();

            //Add transformation links to lineage response
            foreach (var transformation in transformations)
            {
                var lineageTransformation = _mapper.Map<LineageTransformation>(transformation);
                //Add transformation
                if (transformationDirection == TransformationDirection.Sink)
                    lineage.SinkTransformations.Add(lineageTransformation);
                else
                    lineage.SourceTransformations.Add(lineageTransformation);

                var transformationDatasetsForTransformation = await _transformationDatasetRepository.FindAllByTransformationIdAndDirectionAsync(transformation.Id, transformationDirection);
                var datasets = _mapper.Map<IEnumerable<LineageDataset>>(_mapper.Map<IEnumerable<TransformationDataset>>(transformationDatasetsForTransformation).Select(a => a.Dataset).ToArray());

                foreach (var ds in datasets)
                {
                    lineageTransformation.Datasets.Add(ds);
                    if (maxRelations != 0)
                        await GetLineageTransformations(ds, transformationDirection, --maxRelations);
                }
            }
        }

        private async Task ValidateAsync(DatasetCreateRequest request)
        {
            var exceptions = new List<ValidationException>();

            if (request.Categories?.Any() != true)
                exceptions.Add(new ValidationException("Dataset must be assigned at least one category", nameof(request.Categories)));

            if (request.Contact == null)
                exceptions.Add(new ValidationException("Dataset must have a contact", nameof(request.Contact)));

            if (request.Hierarchy == null || request.Hierarchy.Id.Equals(Guid.Empty))
                exceptions.Add(new ValidationException("Dataset must be assigned to a hierarchy", nameof(request.Hierarchy)));

            if (string.IsNullOrWhiteSpace(request.Name))
                exceptions.Add(new ValidationException("Dataset must have a name", nameof(request.Name)));

            if (request.DataSources?.Any() == true)
            {
                var dsIds = request.DataSources.Select(a => a.Id).ToArray();
                var refinementSourceMatchEx = new ValidationException("Refinement level does not match the selected data source(s)",
                    nameof(request.DataSources), nameof(request.RefinementLevel));

                var dataSourcesValid = await AreDataSourcesValid(dsIds, request.RefinementLevel == RefinementLevel.Raw);
                if (!dataSourcesValid) exceptions.Add(refinementSourceMatchEx);
            }

            if (request.DataFields?.Any() == true)
                for (int i = 0; i < request.DataFields.Length; i++)
                {
                    var df = request.DataFields[i];

                    if (string.IsNullOrWhiteSpace(df.Name))
                        exceptions.Add(new ValidationException("Data field must have a name", $"{i}, {nameof(df.Name)}"));

                    if (string.IsNullOrWhiteSpace(df.Type))
                        exceptions.Add(new ValidationException("Data field must have a type", $"{df.Name ?? i.ToString()}, {nameof(df.Type)}"));
                }

            if (exceptions.Any()) throw new ValidationExceptionCollection("Dataset could not be created", exceptions);
        }

        private async Task<bool> AreDataSourcesValid(Guid[] dsIds, bool raw)
        {
            if (raw)
                return !await _dataSourceRepository.AnyAsync(dsIds, new List<SourceType> { SourceType.DataPlatform });
            return !await _dataSourceRepository.AnyAsync(dsIds, new List<SourceType> { SourceType.External, SourceType.Internal });
        }

        private string GetHierarchyName(DataCatalog.Data.Model.Hierarchy hierarchy)
        {
            var name = hierarchy.Name;
            while (hierarchy.ParentHierarchy != null)
            {
                hierarchy = hierarchy.ParentHierarchy;
                name = $"{hierarchy.Name}/{name}";
            }

            return name;
        }
    }
}
