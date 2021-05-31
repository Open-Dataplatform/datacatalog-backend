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
    public class HierarchyController : ControllerBase
    {
        private readonly IHierarchyService _hierarchyService;
        private readonly IMapper _mapper;

        public HierarchyController(IHierarchyService hierarchyService, IMapper mapper)
        {
            _hierarchyService = hierarchyService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all hierarchies
        /// </summary>
        /// <returns>A list of all hierarchies</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var hierarchies = await _hierarchyService.ListAsync();

            if (hierarchies == null)
                return NotFound();

            var result = _mapper.Map<IEnumerable<Data.Domain.Hierarchy>, IEnumerable<HierarchyResponse>>(hierarchies);

            return Ok(result);
        }

        /// <summary>
        /// Get a hierarchy by id
        /// </summary>
        /// <param name="id">The id of the hierarchy to get</param>
        /// <returns>The hierarchy</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var hierarchy = await _hierarchyService.FindByIdAsync(id);

            if (hierarchy == null)
                return NotFound();

            var result = _mapper.Map<Data.Domain.Hierarchy, HierarchyResponse>(hierarchy);

            return Ok(result);
        }

        /// <summary>
        /// Create a new hierarchy
        /// </summary> 
        /// <param name="request">The hierarchy to create</param>
        /// <returns>The created hierarchy</returns>
        [AuthorizeRoles(Role.Admin)]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] HierarchyCreateRequest request)
        {
            var hierarchy = _mapper.Map<HierarchyCreateRequest, Data.Domain.Hierarchy>(request);
            await _hierarchyService.SaveAsync(hierarchy);

            return Ok(hierarchy.Id);
        }

        /// <summary>
        /// Update hierarchy
        /// </summary>
        /// <param name="request">The hierarchy to create</param>
        /// <returns>The created hierarchy</returns>
        [AuthorizeRoles(Role.Admin)]
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] HierarchyUpdateRequest request)
        {
            var hierarchy = _mapper.Map<HierarchyUpdateRequest, Data.Domain.Hierarchy>(request);
            await _hierarchyService.UpdateAsync(hierarchy);

            return Ok(hierarchy.Id);
        }

        /// <summary>
        /// Delete a data source
        /// </summary>
        /// <param name="id">The id of the hierarchy to delete</param>
        [AuthorizeRoles(Role.Admin)]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _hierarchyService.DeleteAsync(id);

            return Ok();
        }
    }
}