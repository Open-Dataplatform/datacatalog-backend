﻿using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Common.Data;
using DataCatalog.Common.Interfaces;
using DataCatalog.Api.Services.AD;
using DataCatalog.Api.Services.Storage;
using DataCatalog.Api.Extensions;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward, Role.User)]
    [ApiController]
    [Route("api/[controller]")]
    public class DatasetController : ControllerBase
    {
        private readonly IDatasetService _datasetService;
        private readonly IMapper _mapper;
        private readonly Current _current;
        private readonly IGroupService _groupService;
        private readonly IStorageService _storageService;
        private readonly IAllUsersGroupProvider _allUsersGroupProvider;

        public DatasetController(
            IDatasetService datasetService,
            IMapper mapper,
            Current current,
            IGroupService groupService,
            IStorageService storageService,
            IAllUsersGroupProvider allUsersGroupProvider)
        {
            _datasetService = datasetService;
            _mapper = mapper;
            _current = current;
            _groupService = groupService;
            _storageService = storageService;
            _allUsersGroupProvider = allUsersGroupProvider;
        }

        /// <summary>
        /// Get a dataset
        /// </summary>
        /// <param name="id">The id of the dataset to get</param>
        /// <returns>A dataset</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<DatasetResponse>> FindByIdAsync(Guid id)
        {
            var dataset = await _datasetService.FindByIdAsync(id);

            if (dataset == null)
                return NotFound();

            var result = _mapper.Map<Data.Domain.Dataset, DatasetResponse>(dataset);

            return Ok(result);
        }

        /// <summary>
        /// Get all datasets
        /// </summary>
        /// <returns>A list of dataset summaries</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DatasetResponse>>> GetAllAsync()
        {
            var datasets = await _datasetService.GetAllSummariesAsync();
            var result = _mapper.Map<IEnumerable<Data.Domain.Dataset>, IEnumerable<DatasetResponse>>(datasets);

            return Ok(result);
        }

        /// <summary>
        /// Create a new dataset
        /// </summary>
        /// <param name="request">The dataset to create</param>
        /// <returns>The created dataset</returns>
        [AuthorizeRoles(Role.Admin, Role.DataSteward)]
        [HttpPost]
        public async Task<ActionResult<DatasetResponse>> PostAsync(DatasetCreateRequest request)
        {
            request.Owner = User.FindFirst(ClaimsUtility.ClaimUserIdentity).Value;
            return _mapper.Map<DatasetResponse>(await _datasetService.SaveAsync(request));
        }

        /// <summary>
        /// Update a dataset
        /// </summary>
        /// <param name="request">The dataset to update</param>
        /// <returns>The updated dataset</returns>
        [AuthorizeRoles(Role.Admin, Role.DataSteward)]
        [HttpPut]
        public async Task<ActionResult<DatasetResponse>> PutAsync(DatasetUpdateRequest request)
        {
            var dbDataset = await _datasetService.FindByIdAsync(request.Id);
            var dataset = await _datasetService.UpdateAsync(request);

            var directoryMetadata = await _storageService.GetDirectoryMetadataWithRetry(request.Id);

            if (directoryMetadata != null)
            {
                directoryMetadata.TryGetValue(GroupConstants.ReaderGroup, out var readerGroupId);

                var datasetHadAllUsersBefore = dbDataset.ShouldHaveAllUsersGroup();
                var datasetShouldHaveAllUsersNow = dataset.ShouldHaveAllUsersGroup();

                if (readerGroupId != null)
                {
                    if (datasetHadAllUsersBefore && !datasetShouldHaveAllUsersNow)
                    {
                        // Updated from public to non-public -> Remove AllUsersGroup
                        await _groupService.RemoveGroupMemberAsync(readerGroupId, _allUsersGroupProvider.GetAllUsersGroup());
                    }
                    else if (!datasetHadAllUsersBefore && datasetShouldHaveAllUsersNow)
                    {
                        // Updated from non-public to public -> Add AllUsersGroup
                        await _groupService.AddGroupMemberAsync(readerGroupId, _allUsersGroupProvider.GetAllUsersGroup());
                    }
                }
            }

            return Ok(_mapper.Map<DatasetResponse>(dataset));
        }

       

        /// <summary>
        /// Delete a dataset
        /// </summary>
        /// <param name="request">The id of the dataset to delete</param>
        [AuthorizeRoles(Role.Admin, Role.DataSteward)]
        [HttpDelete("{request:guid}")]
        public async Task<ActionResult> DeleteAsync(Guid request)
        {
            await _datasetService.DeleteAsync(request);

            return Ok();
        }

        /// <summary>
        /// Get a list of datasets in a given category
        /// </summary>
        /// <param name="request">The search request</param>
        /// <returns>A list of dataset summaries</returns>
        [HttpPost]
        [Route("search/category")]
        public async Task<ActionResult<IEnumerable<DatasetSummaryResponse>>> GetByCategoryAsync(DatasetSearchByCategoryRequest request)
        {
            var datasets = await _datasetService.GetDatasetByCategoryAsync(request.CategoryId, request.SortType, request.Take, request.PageSize, request.PageIndex);
            var result = _mapper.Map<IEnumerable<Data.Domain.Dataset>, IEnumerable<DatasetSummaryResponse>>(datasets);

            return Ok(result);
        }

        /// <summary>
        /// Search for datasets using a search term
        /// </summary>
        /// <param name="request">The search request</param>
        /// <returns>A list of dataset summaries</returns>
        /// <remarks>A blank search term will return all datasets</remarks>
        [HttpPost]
        [Route("search/term")]
        public async Task<ActionResult<DatasetSummaryResponse[]>> GetBySearchTermAsync(DatasetSearchByTermRequest request)
        {
            var datasets = await _datasetService.GetDatasetsBySearchTermAsync(request.SearchTerm, request.SortType, request.Take, request.PageSize, request.PageIndex);
            var result = _mapper.Map<IEnumerable<Data.Domain.Dataset>, IEnumerable<DatasetSummaryResponse>>(datasets);

            return Ok(result);
        }

        /// <summary>
        /// Search for datasets using a search term
        /// </summary>
        /// <param name="request">The search request</param>
        /// <returns>A list of dataset names</returns>
        /// <remarks>A blank search term will return all datasets</remarks>
        [HttpPost]
        [Route("search/predictive")]
        public async Task<ActionResult<IEnumerable<DatasetResponse>>> GetNameBySearchTermAsync(DatasetSearchByTermRequest request)
        {
            var datasets = await _datasetService.GetDatasetsBySearchTermAsync(request.SearchTerm, request.SortType, request.Take, request.PageSize, request.PageIndex);
            var result = _mapper.Map<IEnumerable<Data.Domain.Dataset>, IEnumerable<DatasetResponse>>(datasets);

            return Ok(result);
        }

        /// <summary>
        /// Get the location path for a dataset
        /// </summary>
        /// <param name="request">The location request</param>
        /// <returns>The location path</returns>
        [AuthorizeRoles(Role.Admin, Role.DataSteward)]
        [HttpPost]
        [Route("location")]
        public async Task<ActionResult<DatasetLocationResponse>> GetDatasetLocationAsync(DatasetLocationRequest request)
        {
            var location = await _datasetService.GetDatasetLocationAsync(request.Hierarchy.Id, request.Name);
            var result = new DatasetLocationResponse
            {
                Location = location
            };

            return Ok(result);
        }

        /// <summary>
        /// Get the lineage of a dataset
        /// </summary>
        /// <param name="id">The id of the dataset</param>
        /// <returns>A lineage model with source and sink transformations</returns>
        [HttpGet]
        [Route("lineage/{id}")]
        public async Task<ActionResult<LineageDatasetResponse>> GetLineageAsync(Guid id)
        {
            var lineageDataset = await _datasetService.GetDatasetLineageAsync(id);
            var result = _mapper.Map<Data.Domain.LineageDataset, LineageDatasetResponse>(lineageDataset);

            return result;
        }

        /// <summary>
        /// Given an id of a dataset in raw, this endpoint will return an unsaved copy almost ready for stock
        /// </summary>
        /// <param name="id">Id for dataset in raw</param>
        /// <returns>The stock copy</returns>
        [AuthorizeRoles(Role.Admin, Role.DataSteward)]
        [HttpPost]
        [Route("promote/{id}")]
        public async Task<ActionResult<DatasetResponse>> CopyDatasetInRawAsync(Guid id)
        {
            var dataset = await _datasetService.CopyDatasetInRawAsync(id);

            if (dataset == null && dataset.RefinementLevel != RefinementLevel.Raw)
                return NotFound();

            var result = _mapper.Map<Data.Domain.Dataset, DatasetResponse>(dataset);

            return Ok(result);
        }

    }
}

