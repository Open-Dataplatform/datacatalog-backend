using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using DataCatalog.Common.Interfaces;

namespace DataCatalog.Api.Data.Dto
{
    public class MemberCreateRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Role MemberRole { get; set; }        
    }

    public class MemberUpdateRequest : GuidId, IUpdateRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Role MemberRole { get; set; }
    }

    public class MemberResponse: Entity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Role MemberRole { get; set; }
    }
}