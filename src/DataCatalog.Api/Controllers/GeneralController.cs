using DataCatalog.Api.Data.Dto;
using DataCatalog.Api.Enums;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Api.Extensions;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward, Role.User)]
    [ApiController]
    [Route("api/[controller]")]
    public class GeneralController : ControllerBase
    {
        private readonly IHierarchyService _hierarchyService;
        private readonly IDurationService _durationService;
        private readonly IMapper _mapper;

        public GeneralController(IHierarchyService hierarchyService, IDurationService durationService, IMapper mapper)
        {
            _hierarchyService = hierarchyService;
            _durationService = durationService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get sort types
        /// </summary>
        /// <returns>A list of sort types</returns>
        [HttpGet]
        [Route("sorttype")]
        public ActionResult<EnumResponse[]> GetSortTypeValues()
        {
            return GetEnums<SortType>();
        }

        /// <summary>
        /// Get dataset statuses
        /// </summary>
        /// <returns>A list of dataset statuses</returns>
        [HttpGet]
        [Route("datasetstatus")]
        public ActionResult<EnumResponse[]> GetDatasetStatusValues()
        {
            return GetEnums<DatasetStatus>();
        }

        /// <summary>
        /// Get member roles
        /// </summary>
        /// <returns>A list of member roles</returns>
        [HttpGet]
        [Route("memberrole")]
        public ActionResult<EnumResponse[]> GetMemberRoles()
        {
            return GetEnums<Role>();
        }

        /// <summary>
        /// Get confidentialities
        /// </summary>
        /// <returns>A list of confidentialities</returns>
        [HttpGet]
        [Route("confidentiality")]
        public ActionResult<EnumResponse[]> GetConfidentialities()
        {
            return GetEnums<Confidentiality>();
        }

        /// <summary>
        /// Get refinement levels
        /// </summary>
        /// <returns>A list of refinement levels</returns>
        [HttpGet]
        [Route("refinementlevel")]
        public ActionResult<EnumResponse[]> GetRefinementLevels()
        {
            return GetEnums<RefinementLevel>();
        }

        /// <summary>
        /// Get source types
        /// </summary>
        /// <returns>A list of refinement levels</returns>
        [HttpGet]
        [Route("sourcetypes")]
        public ActionResult<EnumResponse[]> GetSourceTypes()
        {
            return GetEnums<SourceType>();
        }

        /// <summary>
        /// Get durations
        /// </summary>
        /// <returns>A list of durations</returns>
        [HttpGet]
        [Route("duration")]
        [Obsolete("Use GET api/Duration endpoint instead")]
        public async Task<ActionResult<DurationResponse[]>> GetDuration()
        {
            var durations = await _durationService.ListAsync();

            if (durations == null)
                return NotFound();

            var result = _mapper.Map<IEnumerable<Data.Domain.Duration>, IEnumerable<DurationResponse>>(durations);

            return Ok(result);
        }

        /// <summary>
        /// Get hierarchies
        /// </summary>
        /// <returns>A list of hierarchies with their child hierarchies</returns>
        [HttpGet]
        [Route("hierarchies")]
        [Obsolete("Use GET api/Hierarchy endpoint instead")]
        public async Task<ActionResult<HierarchyResponse[]>> GetHierarchies()
        {
            var hierarchies = await _hierarchyService.ListAsync();

            if (hierarchies == null)
                return NotFound();

            var result = _mapper.Map<IEnumerable<Data.Domain.Hierarchy>, IEnumerable<HierarchyResponse>>(hierarchies);

            return Ok(result);
        }

        private EnumResponse[] GetEnums<TEnum>() =>
            EnumExtensions.GetEnums<TEnum>().Select(a => new EnumResponse
            {
                Id = Convert.ToInt32(a),
                Description = a.EnumNameToDescription()
            }).ToArray();
    }
}