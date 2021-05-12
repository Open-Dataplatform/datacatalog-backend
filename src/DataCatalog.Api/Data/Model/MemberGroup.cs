using DataCatalog.Api.Data.Common;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Model
{
    public class MemberGroup : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }

        public List<MemberGroupMember> MemberGroupMembers { get; set; } = new List<MemberGroupMember>();
        public List<Dataset> Datasets { get; set; } = new List<Dataset>();
    }
}