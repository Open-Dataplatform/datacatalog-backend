using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;
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
    public class DurationController : ControllerBase
    {
        private readonly IDurationService _durationService;
        private readonly IMapper _mapper;

        public DurationController(IDurationService durationService, IMapper mapper)
        {
            _durationService = durationService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all durations
        /// </summary>
        /// <returns>A list of all durations</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var durations = await _durationService.ListAsync();

            if (durations == null)
                return NotFound();

            var result = _mapper.Map<IEnumerable<Data.Domain.Duration>, IEnumerable<DurationResponse>>(durations);

            return Ok(result);
        }

        /// <summary>
        /// Get a duration by id
        /// </summary>
        /// <param name="id">The id of the duration to get</param>
        /// <returns>The duration</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var duration = await _durationService.FindByIdAsync(id);

            if (duration == null)
                return NotFound();

            var result = _mapper.Map<Data.Domain.Duration, DurationResponse>(duration);

            return Ok(result);
        }

        /// <summary>
        /// Create a new duration
        /// </summary> 
        /// <param name="request">The duration to create</param>
        /// <returns>The created duration</returns>
        [AuthorizeRoles(Role.Admin)]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] DurationCreateRequest request)
        {
            var duration = _mapper.Map<DurationCreateRequest, Data.Domain.Duration>(request);
            await _durationService.SaveAsync(duration);

            return Ok(duration.Id);
        }

        /// <summary>
        /// Update duration
        /// </summary>
        /// <param name="request">The duration to create</param>
        /// <returns>The created duration</returns>
        [AuthorizeRoles(Role.Admin)]
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] DurationUpdateRequest request)
        {
            var duration = _mapper.Map<DurationUpdateRequest, Data.Domain.Duration>(request);
            await _durationService.UpdateAsync(duration);

            return Ok(duration.Id);
        }

        /// <summary>
        /// Delete a data source
        /// </summary>
        /// <param name="id">The id of the duration to delete</param>
        [AuthorizeRoles(Role.Admin)]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _durationService.DeleteAsync(id);

            return Ok();
        }
    }
}