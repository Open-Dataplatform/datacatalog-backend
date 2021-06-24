using DataCatalog.Common.Enums;
using Microsoft.AspNetCore.Authorization;

namespace DataCatalog.Api.Infrastructure
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params Role[] allowedRoles)
        {
            Roles = string.Join(',', allowedRoles);
        }
    }
}
