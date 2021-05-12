using DataCatalog.Api.Data.Common;

namespace DataCatalog.Api.Data.Dto
{
    public class DatasetChangeLogResponse : Created
    {
        public MemberResponse Member { get; set; }
    }
}