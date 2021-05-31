using DataCatalog.Common.Data;

namespace DataCatalog.Api.Data.Dto
{
    public class DatasetChangeLogResponse : Created
    {
        public MemberResponse Member { get; set; }
    }
}