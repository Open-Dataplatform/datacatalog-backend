﻿using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Common.Extensions;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward, Role.User)]
    [ApiController]
    [Route("api/[controller]")]
    public class GeneralController : ControllerBase
    {
        private readonly IDurationService _durationService;
        private readonly IMapper _mapper;

        public GeneralController(IDurationService durationService, IMapper mapper)
        {
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
        /// Get source types
        /// </summary>
        /// <returns>A list of source types</returns>
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
        /// Dummy endpoint for exposing the DataFieldType to Swagger
        /// </summary>
        [HttpGet]
        [Route("dataFieldType")]
        [Obsolete("Dummy endpoint - Use enum generated by NSwag")]
        public async Task<ActionResult<DataFieldType>> GetDataFieldType()
        {
            return Ok(DataFieldType.Boolean);
        }

        private EnumResponse[] GetEnums<TEnum>() =>
            EnumExtensions.GetEnums<TEnum>().Select(a => new EnumResponse
            {
                Id = Convert.ToInt32(a),
                Description = a.EnumNameToDescription()
            }).ToArray();
    }
}