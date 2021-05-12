using AutoMapper;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Api.Enums;
using DataCatalog.Api.Extensions;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward, Role.User)]
    [ApiController]
    [Route("api/[controller]")]
    public class DataContractController : ControllerBase
    {
        private readonly IDataContractService _dataContractService;
        private readonly IMapper _mapper;

        public DataContractController(IDataContractService dataContractService, IMapper mapper)
        {
            _dataContractService = dataContractService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all data contracts
        /// </summary>
        /// <returns>All data contracts</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var dataContracts = await _dataContractService.ListAsync();

            if (dataContracts == null)
                return NotFound();

            var result = _mapper.Map<IEnumerable<Data.Domain.DataContract>, IEnumerable<DataContractResponse>>(dataContracts);

            return Ok(result);
        }

        /// <summary>
        /// Get a data contract
        /// </summary>
        /// <param name="id">The id of the data contract to get</param>
        /// <returns>A data contract</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var dataContract = await _dataContractService.FindByIdAsync(id);

            if (dataContract == null)
                return NotFound();

            var result = _mapper.Map<Data.Domain.DataContract, DataContractResponse>(dataContract);

            return Ok(result);
        }

        /// <summary>
        /// Get all data contracts for a dataset
        /// </summary>
        /// <param name="id">The id of the dataset to get data contracts for</param>
        /// <returns>A list of data contracts</returns>
        [HttpGet]
        [Route("bydataset/{id}")]
        public async Task<ActionResult<IEnumerable<DataContractResponse>>> GetByDatasetIdAsync(Guid id)
        {
            var dataContracts = await _dataContractService.GetByDatasetIdAsync(id);

            if (dataContracts == null)
                return NotFound();

            var result = _mapper.Map<IEnumerable<Data.Domain.DataContract>, IEnumerable<DataContractResponse>> (dataContracts);

            return Ok(result);
        }

        /// <summary>
        /// Get all data contracts for a data source
        /// </summary>
        /// <param name="id">The id of the data source to get data contracts for</param>
        /// <returns>A list of data contracts</returns>
        [HttpGet]
        [Route("bydatasource/{id}")]
        public async Task<ActionResult<IEnumerable<DataContractResponse>>> GetByDataSourceIdAsync(Guid id)
        {
            var dataContracts = await _dataContractService.GetByDataSourceIdAsync(id);

            if (dataContracts == null)
                return NotFound();

            var result = _mapper.Map<IEnumerable<Data.Domain.DataContract>, IEnumerable<DataContractResponse>>(dataContracts);

            return Ok(result);
        }

        /// <summary>
        /// Create a new data contract
        /// </summary>
        /// <param name="request">The data contract to create</param>
        /// <returns>The created data contract</returns>
        [AuthorizeRoles(Role.Admin)]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] DataContractCreateRequest request)
        {
            if (!ModelState.IsValid)
                return StatusCode((int)HttpStatusCode.BadRequest, ModelState.GetErrorMessages());

            var dataContract = _mapper.Map<DataContractCreateRequest, Data.Domain.DataContract>(request);
            await _dataContractService.SaveAsync(dataContract);

            return Ok(dataContract.Id);
        }

        /// <summary>
        /// Update a data contract
        /// </summary>
        /// <param name="request">The data contract to update</param>
        /// <returns>The updated data contract</returns>
        [AuthorizeRoles(Role.Admin)]
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] DataContractUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return StatusCode((int)HttpStatusCode.BadRequest, ModelState.GetErrorMessages());

            var dataContract = _mapper.Map<DataContractUpdateRequest, Data.Domain.DataContract>(request);
            await _dataContractService.UpdateAsync(dataContract);

            return Ok(dataContract.Id);
        }

        /// <summary>
        /// Delete a data contract
        /// </summary>
        /// <param name="id">The id of the data contract to delete</param>
        [AuthorizeRoles(Role.Admin)]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _dataContractService.DeleteAsync(id);

            return Ok();
        }
    }
}