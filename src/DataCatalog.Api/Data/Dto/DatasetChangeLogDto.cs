using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Data.Dto
{
    public class DatasetChangeLogResponse : Created
    {
        public MemberResponse Member { get; set; }
        public DatasetChangeType DatasetChangeType { get; set; }
        public DatasetPermissionChangeResponse DatasetPermissionChange { get; set; }
    }

    public class DatasetPermissionChangeResponse
    {
        public PermissionChangeType PermissionChangeType { get; set; }
        public AccessType AccessType { get; set; }
        public AccessMemberType AccessMemberType { get; set; }
        public string DisplayName { get; set; }
        public string Mail { get; set; }
    }
}