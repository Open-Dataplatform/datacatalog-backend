using DataCatalog.Common.Data;
using System;

namespace DataCatalog.Data.Model
{
    public class MemberGroupMember : Created
    {
        public Guid MemberId { get; set; }
        public Guid MemberGroupId { get; set; }

        public Member Member { get; set; }
        public MemberGroup MemberGroup { get; set; }
    }
}