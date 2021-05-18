using System;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Domain
{
    public class MemberGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<MemberGroupMember> MemberGroupMembers { get; set; } = new List<MemberGroupMember>();
        public List<Dataset> Datasets { get; set; } = new List<Dataset>();
    }
}
