using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Common.Data;

namespace DataCatalog.Api.Data.Dto
{
    public class EntityDto : GuidId
    {
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}