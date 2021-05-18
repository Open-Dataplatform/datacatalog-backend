using System;

namespace DataCatalog.Api.Data.Domain
{
    public class MemberGroupMember
    {
        public Guid MemberId { get; set; }
        public Guid MemberGroupId { get; set; }

        public Member Member { get; set; }
        public MemberGroup MemberGroup { get; set; }
    }
}
