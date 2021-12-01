using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services;
using DataCatalog.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin)] // TODO - replace with new role
    [Route("api/[controller]")]
    [ApiController]
    public class DataAvailabilityController : ControllerBase
    {
        private readonly IDatasetService _datasetService;

        public DataAvailabilityController(IDatasetService datasetService)
        {
            _datasetService = datasetService;
        }

        /// <summary>
        /// Upsert availability info for a dataset
        /// </summary>
        [HttpPost]
        public async Task PostAsync([FromBody] DataAvailabilityInfoDto request)
        {
            await _datasetService.InsertOrUpdateAvailability(request);
        }
    }
}