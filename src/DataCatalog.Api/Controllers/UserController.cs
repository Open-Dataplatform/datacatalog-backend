using DataCatalog.Common.Enums;
using DataCatalog.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward, Role.User)]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Get name and roles for current user
        /// </summary>
        /// <returns>User name and roles</returns>
        [HttpGet]
        public ActionResult<Data.Domain.User> GetUserInfo()
        {
            var roles = new List<string>();

            if (User.IsInRole(Role.Admin.ToString()))
                roles.Add(Role.Admin.ToString());
            if (User.IsInRole(Role.DataSteward.ToString()))
                roles.Add(Role.DataSteward.ToString());
            if (User.IsInRole(Role.User.ToString()))
                roles.Add(Role.User.ToString());

            var result = new Data.Domain.User
            {
                Name = ClaimsUtility.GetClaim(User, ClaimsUtility.ClaimName),
                Roles = roles
            };

            return Ok(result);
        }

    }
}
