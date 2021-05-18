using DataCatalog.Api.Data.Common;
using System;

namespace DataCatalog.Api.Data.Model
{
    public class MemberGroupMember : Created
    {
        public Guid MemberId { get; set; }
        public Guid MemberGroupId { get; set; }

        public Member Member { get; set; }
        public MemberGroup MemberGroup { get; set; }
    }
}