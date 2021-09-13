﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Common.Data;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;
using DataCatalog.Api.Exceptions;
using DataCatalog.Api.Messages;
using DataCatalog.Api.Repositories;
using Rebus.Bus;
using DataCatalog.Api.Extensions;
using Microsoft.AspNetCore.Http;

namespace DataCatalog.Api.Services
{
    public class DatasetService : IDatasetService
    {
        private readonly IDatasetRepository _datasetRepository;
        private readonly ITransformationRepository _transformationRepository;
        private readonly ITransformationDatasetRepository _transformationDatasetRepository;
        private readonly IDurationRepository _durationRepository;
        private readonly IDatasetDurationRepository _datasetDurationRepository;
        private readonly IDatasetChangeLogRepository _datasetChangeLogRepository;
        private readonly IDataSourceRepository _dataSourceRepository;
        private readonly IBus _bus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Current _current;
        private readonly IMapper _mapper;
        private readonly IPermissionUtils _permissionUtils;

        public DatasetService(
            IDatasetRepository datasetRepository,
            ITransformationRepository transformationRepository,
            ITransformationDatasetRepository transformationDatasetRepository,
            IDatasetDurationRepository datasetDurationRepository,
            IDurationRepository durationRepository,
            IDatasetChangeLogRepository datasetChangeLogRepository,
            IDataSourceRepository dataSourceRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            Current current,
            IBus bus, 
            IPermissionUtils permissionUtils)
        {
            _datasetRepository = datasetRepository;
            _transformationRepository = transformationRepository;
            _transformationDatasetRepository = transformationDatasetRepository;
            _durationRepository = durationRepository;
            _datasetDurationRepository = datasetDurationRepository;
            _datasetChangeLogRepository = datasetChangeLogRepository;
            _dataSourceRepository = dataSourceRepository;
            _unitOfWork = unitOfWork;
            _current = current;
            _bus = bus;
            _mapper = mapper;
            _permissionUtils = permissionUtils;
        }

        public async Task<Dataset> FindByIdAsync(Guid id)
        {
            var dataset = await _datasetRepository.FindByIdAsync(id);

            if (dataset != null)
                return _mapper.Map<Dataset>(dataset);

            return null;
        }

        public async Task<IEnumerable<Dataset>> GetAllSummariesAsync()
        {
            var datasets = await _datasetRepository.ListSummariesAsync();
            return datasets.Select(x => _mapper.Map<Dataset>(x));
        }

        public async Task<Dataset> SaveAsync(DatasetCreateRequest request)
        {
            ValidateAsync(request);

            var dbDataset = _mapper.Map<DataCatalog.Data.Model.Dataset>(request);

            if (request.SourceTransformation?.SourceDatasets?.Any() == true)
                await InsertOrUpdateSourceTransformation(request.SourceTransformation, dbDataset);

            await InsertOrUpdateDuration(request.Frequency, dbDataset, DurationType.Frequency);
            await InsertOrUpdateDuration(request.Resolution, dbDataset, DurationType.Resolution);
            InsertOrUpdateDataContracts(dbDataset, request.DataSources);
            await AddChangeLog(dbDataset);
            dbDataset.ProvisionStatus = ProvisionDatasetStatusEnum.Pending;

            await _unitOfWork.CompleteAsync();

            return await PublishDatasetCreated(dbDataset);
        }

        private async Task<Dataset> PublishDatasetCreated(DataCatalog.Data.Model.Dataset dbDataset)
        {
            var dataset = _mapper.Map<Dataset>(dbDataset);

            // Publish a message that the dataset has been created.
            var datasetCreatedMessage = new DatasetCreatedMessage
            {
                DatasetId = dataset.Id,
                DatasetName = dataset.Name,
                Owner = dataset.Owner,
                AddAllUsersGroup = dataset.ShouldHaveAllUsersGroup()
            };

            await _bus.Publish(datasetCreatedMessage);

            return dataset;
        }

        public async Task<Dataset> UpdateAsync(DatasetUpdateRequest request)
        {
            ValidateAsync(request);

            var dbDataset = await _datasetRepository.FindByIdAsync(request.Id);
            _mapper.Map(request, dbDataset);

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
            {
                throw new ObjectNotFoundException($"Could not find dataset with id {id}");
            }

            _datasetRepository.Remove(existingDataset);
            await _unitOfWork.CompleteAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var existingDataset = await _datasetRepository.FindByIdAsync(id);

            if (existingDataset == null)
            {
                throw new ObjectNotFoundException($"Could not find dataset with id {id}");
            }

            var lineageDataset = await GetDatasetLineageAsync(id);
            if (lineageDataset.SinkTransformations.Any(transformation => transformation.Datasets.Any()))
            {
                throw new BadHttpRequestException(
                    "Cannot delete the dataset since it's a source for others. Delete it's children first");
            }

            existingDataset.IsDeleted = true;
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<Dataset>> GetDatasetByCategoryAsync(Guid categoryId, SortType sortType, int take, int pageSize, int pageIndex)
        {
            var datasets = await _datasetRepository.GetDatasetByCategoryAsync(categoryId, sortType, take, pageSize, pageIndex);
            return _mapper.Map<IEnumerable<Dataset>>(datasets);
        }

        public async Task<IEnumerable<Dataset>> GetDatasetsBySearchTermAsync(string searchTerm, SortType sortType, int take, int pageSize, int pageIndex)
        {
            var datasets = await _datasetRepository.GetDatasetsBySearchTermQueryAsync(searchTerm, sortType, take, pageSize, pageIndex);
            return _mapper.Map<IEnumerable<Dataset>>(datasets);
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

        private void ValidateAsync(DatasetCreateRequest request)
        {
            var exceptions = new List<ValidationException>();

            if (request.Categories?.Any() != true)
                exceptions.Add(new ValidationException("Dataset must be assigned at least one category", nameof(request.Categories)));

            if (request.Contact == null)
                exceptions.Add(new ValidationException("Dataset must have a contact", nameof(request.Contact)));

            if (string.IsNullOrWhiteSpace(request.Name))
                exceptions.Add(new ValidationException("Dataset must have a name", nameof(request.Name)));

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
    }
}
