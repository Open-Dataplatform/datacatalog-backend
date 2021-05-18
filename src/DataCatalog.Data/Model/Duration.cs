using DataCatalog.Api.Data.Common;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Model
{
    public class Duration : Entity
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public List<DatasetDuration> DatasetsDurations { get; set; } = new List<DatasetDuration>();
    }
}