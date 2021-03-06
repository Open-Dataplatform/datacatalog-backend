using DataCatalog.Api.Infrastructure;
using DataCatalog.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward, Role.User, Role.ServiceReader)]
    [ApiController]
    [Route("api/[controller]")]
    public class FrontendMetricsController : ControllerBase
    {
        [HttpGet("oboflow-init")]
        public IActionResult OboFlowInitiated()
        {
            return Ok();
        }
    }
}