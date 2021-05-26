using AutoMapper;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward, Role.User)]
    [ApiController]
    [Route("api/[controller]")]
    public class DatasetGroupController : ControllerBase
    {
        private readonly IDatasetGroupService _datasetGroupService;
        private readonly IMapper _mapper;

        public DatasetGroupController(IDatasetGroupService datasetGroupService, IMapper mapper)
        {
            _datasetGroupService = datasetGroupService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all dataset groups
        /// </summary>
        /// <returns>A list of all dataset groups</returns>
        /// <remarks>Restricted to only working with the current member's dataset groups</remarks>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var datasetGroups = await _datasetGroupService.ListAsync();

            if (datasetGroups == null)
                return NotFound();

            var result = _mapper.Map<IEnumerable<Data.Domain.DatasetGroup>, IEnumerable<DatasetGroupResponse>>(datasetGroups);

            return Ok(result);
        }

        /// <summary>
        /// Get a dataset group by id
        /// </summary>
        /// <param name="id">The id of the dataset group to get</param>
        /// <returns>The dataset group</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var datasetGroup = await _datasetGroupService.FindByIdAsync(id);

            if (datasetGroup == null)
                return NotFound();

            var result = _mapper.Map<Data.Domain.DatasetGroup, DatasetGroupResponse>(datasetGroup);

            return Ok(result);
        }

        /// <summary>
        /// Create a new dataset group
        /// </summary> 
        /// <param name="request">The dataset group to create</param>
        /// <returns>The created dataset group</returns>
        [AuthorizeRoles(Role.Admin, Role.DataSteward)]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] DatasetGroupCreateRequest request)
        {
            var datasetGroup = _mapper.Map<DatasetGroupCreateRequest, Data.Domain.DatasetGroup>(request);
            var newDatasetGroup = await _datasetGroupService.SaveAsync(datasetGroup);

            return Ok(_mapper.Map<DatasetGroupResponse>(newDatasetGroup));
        }

        /// <summary>
        /// Update a dataset group
        /// </summary>
        /// <param name="request">The dataset group to update</param>
        /// <returns>The updated dataset group</returns>
        /// <remarks>Restricted to only working with the current member's dataset groups</remarks>
        [AuthorizeRoles(Role.Admin, Role.DataSteward)]
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] DatasetGroupUpdateRequest request)
        {
            var datasetGroup = _mapper.Map<DatasetGroupUpdateRequest, Data.Domain.DatasetGroup>(request);
            var updatedDatasetGroup = await _datasetGroupService.UpdateAsync(datasetGroup);

            return Ok(_mapper.Map<DatasetGroupResponse>(updatedDatasetGroup));
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="id">The id of the category to delete</param>
        /// <remarks>Categories with references to dataset cannot be deleted!</remarks>
        [AuthorizeRoles(Role.Admin, Role.DataSteward)]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _datasetGroupService.DeleteAsync(id);

            return Ok();
        }
    }
}