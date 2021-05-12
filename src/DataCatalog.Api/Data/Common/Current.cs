using DataCatalog.Api.Enums;
using System;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Common
{
    /// <summary>
    /// This class has been made for carrying any information required across services within a request scope.
    /// This is so we don't need HttpContext or other web dependencies in our service layer.
    /// </summary>
    public class Current
    {
        public Guid MemberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public List<Role> Roles { get; set; }

        public Current()
        {
            Roles = new List<Role>();
        }
    }
}