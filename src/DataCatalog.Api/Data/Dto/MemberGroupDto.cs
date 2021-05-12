using System;
using DataCatalog.Api.Data.Common;
using DataCatalog.Api.Interfaces;

namespace DataCatalog.Api.Data.Dto
{
    public class MemberGroupCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public GuidId[] Members { get; set; }
    }

    public class MemberGroupUpdateRequest : GuidId, IUpdateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public GuidId[] Members { get; set; }
    }

    public class MemberGroupResponse : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public GuidId[] Members { get; set; }
        public GuidId[] Datasets { get; set; }
    }

    public class MemberGroupAddRequest
    {
        public Guid MemberId { get; set; }
        public Guid MemberGroupId { get; set; }
    }

    public class MemberGroupRemoveRequest
    {
        public Guid MemberId { get; set; }
        public Guid MemberGroupId { get; set; }
    }
}
