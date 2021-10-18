using System;
using System.Net;
using System.Threading.Tasks;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services.Egress;
using DataCatalog.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward, Role.User)]
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
            return result.Match<IActionResult>(jsonResult => new OkObjectResult(jsonResult), 
                exception =>
                {
                    var message = exception.Message;
                    if (string.IsNullOrEmpty(message))
                    {
                        message = "Unknown error occured while attempting to access the Egress Api";
                    }
                    var exceptionResult = new ObjectResult(message)
                    {
                        StatusCode = exception switch
                        {
                            AuthorizationException => (int)HttpStatusCode.Forbidden,
                            ConfigurationException => (int)HttpStatusCode.NotFound,
                            _ => (int)HttpStatusCode.InternalServerError
                        }
                    };

                    return exceptionResult;
                });
        }
    }
}