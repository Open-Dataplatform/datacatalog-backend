using DataCatalog.Api.Data.Dto;
using DataCatalog.Api.Enums;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward, Role.User)]
    [ApiController]
    [Route("api/[controller]")]
    public class DataSourceController : ControllerBase
    {
        private readonly IDataSourceService _dataSourceService;
        private readonly IMapper _mapper;

        public DataSourceController(IDataSourceService dataSourceService, IMapper mapper)
        {
            _dataSourceService = dataSourceService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all data sources
        /// </summary>
        /// <returns>A list of all data sources</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var dataSources = await _dataSourceService.ListAsync();

            if (dataSources == null)
                return NotFound();

            var result = _mapper.Map<IEnumerable<Data.Domain.DataSource>, IEnumerable<DataSourceResponse>>(dataSources);

            return Ok(result);
        }

        /// <summary>
        /// Get a data source by id
        /// </summary>
        /// <param name="id">The id of the data source to get</param>
        /// <returns>The category</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var dataSource = await _dataSourceService.FindByIdAsync(id);

            if (dataSource == null)
                return NotFound();

            var result = _mapper.Map<Data.Domain.DataSource, DataSourceResponse>(dataSource);

            return Ok(result);
        }

        /// <summary>
        /// Create a new data source
        /// </summary> 
        /// <param name="request">The data source to create</param>
        /// <returns>The created data source</returns>
        [AuthorizeRoles(Role.Admin)]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] DataSourceCreateRequest request)
        {
            var dataSource = _mapper.Map<DataSourceCreateRequest, Data.Domain.DataSource>(request);
            await _dataSourceService.SaveAsync(dataSource);

            return Ok(dataSource.Id);
        }

        /// <summary>
        /// Update data source
        /// </summary>
        /// <param name="request">The data source to create</param>
        /// <returns>The created data source</returns>
        [AuthorizeRoles(Role.Admin)]
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] DataSourceUpdateRequest request)
        {
            var dataSource = _mapper.Map<DataSourceUpdateRequest, Data.Domain.DataSource>(request);
            await _dataSourceService.UpdateAsync(dataSource);

            return Ok(dataSource.Id);
        }

        /// <summary>
        /// Delete a data source
        /// </summary>
        /// <param name="id">The id of the data source to delete</param>
        /// <remarks>Data sources with references to data contracts cannot be deleted!</remarks>
        [AuthorizeRoles(Role.Admin)]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _dataSourceService.DeleteAsync(id);

            return Ok();
        }
    }
}