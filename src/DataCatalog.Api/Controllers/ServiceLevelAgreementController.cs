using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services;
using DataCatalog.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward, Role.User)]
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceLevelAgreementController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IServiceLevelAgreementService _serviceLevelAgreementService;

        public ServiceLevelAgreementController(IMapper mapper, IServiceLevelAgreementService serviceLevelAgreementService)
        {
            _mapper = mapper;
            _serviceLevelAgreementService = serviceLevelAgreementService;
        }

        /// <summary>
        /// Get all SLA's
        /// </summary>
        /// <returns>A list of SLA's</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceLevelAgreementResponse>>> GetAllAsync()
        {
            var agreements = await _serviceLevelAgreementService.ListAsync();

            if (agreements == null)
                return NotFound();

            var result = _mapper.Map<IEnumerable<ServiceLevelAgreement>, IEnumerable<ServiceLevelAgreementResponse>>(agreements);

            return Ok(result);
        }
    }
}