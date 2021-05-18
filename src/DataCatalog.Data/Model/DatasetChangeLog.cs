using DataCatalog.Api.Data.Common;
using System;

namespace DataCatalog.Api.Data.Model
{
    public class DatasetChangeLog : Created
    {
        public Guid Id { get; set; }
        public Guid DatasetId { get; set; }
        public Guid MemberId { get; set; }

        public Dataset Dataset { get; set; }
        public Member Member { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
    }
}