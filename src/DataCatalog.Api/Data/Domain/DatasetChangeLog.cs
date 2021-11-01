using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Data.Domain
{
    public class DatasetChangeLog
    {
        public Guid Id { get; set; }
        public Guid DatasetId { get; set; }
        public Guid MemberId { get; set; }

        public Dataset Dataset { get; set; }
        public Member Member { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        public DateTime CreatedDate { get; set; }

        public DatasetPermissionChange DatasetPermissionChange { get; set; }
        public DatasetChangeType DatasetChangeType { get; set; }
    }
}
