using DataCatalog.Common.Data;
using System;
using System.Collections.Generic;

namespace DataCatalog.Data.Model
{
    public class Member : Entity
    {
        public Guid IdentityProviderId { get; set; }
        public string ExternalId { get; set; }

        public List<MemberGroupMember> MemberGroupMembers { get; set; } = new List<MemberGroupMember>();
        public List<DatasetChangeLog> DatasetChangeLogs { get; set; } = new List<DatasetChangeLog>();
        public IdentityProvider IdentityProvider { get; set; }
    }
}