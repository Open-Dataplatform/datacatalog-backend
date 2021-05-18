using System.Collections.Generic;

namespace DataCatalog.Api.Data.Domain
{
    public class User
    {
        public string Name { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
