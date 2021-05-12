using System;
using DataCatalog.Api.Enums;

namespace DataCatalog.Api.Data.Domain
{
    public class Member
    {
        public Guid IdentityProviderId { get; set; }
        public string ExternalId { get; set; }
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public Role MemberRole { get; set; }
    }
}
