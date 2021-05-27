using DataCatalog.Api.Enums;
using DataCatalog.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Roles = DataCatalog.Api.Infrastructure.Roles;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward, Role.User)]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private Roles _roles;

        public UserController(Roles roles)
        {
            _roles = roles;
        }

        /// <summary>
        /// Get name and roles for current user
        /// </summary>
        /// <returns>User name and roles</returns>
        [HttpGet]
        public IActionResult GetUserInfo()
        {
            var roles = new List<string>();

            if (User.IsInRole(_roles.Admin))
                roles.Add(Role.Admin.ToString());
            if (User.IsInRole(_roles.DataSteward))
                roles.Add(Role.DataSteward.ToString());
            if (User.IsInRole(_roles.User))
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
