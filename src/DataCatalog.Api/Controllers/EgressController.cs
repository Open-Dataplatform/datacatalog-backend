using System;
using System.Threading.Tasks;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services.Egress;
using DataCatalog.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward, Role.User, Role.ServiceReader)]
    [ApiController]
    [Route("api/[controller]")]
    public class EgressController : ControllerBase
    {
        private const string XAuthorizationHeader = "X-Authorization";
        private readonly IEgressService _egressService;

        public EgressController(IEgressService egressService)
        {
            _egressService = egressService;
        }

        [HttpGet("preview/{datasetId:guid}")]
        public async Task<IActionResult> PreviewData(Guid datasetId, [FromQuery] string fromDate, [FromQuery] string toDate)
        {
            var authorizationHeader = Request.Headers[XAuthorizationHeader];
            var result = await _egressService.FetchData(datasetId, fromDate, toDate, authorizationHeader);
            return new ObjectResult(result);
        }
    }
}