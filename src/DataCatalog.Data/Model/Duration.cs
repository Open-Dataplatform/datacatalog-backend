using DataCatalog.Common.Data;
using System.Collections.Generic;

namespace DataCatalog.Data.Model
{
    public class Duration : Entity
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public List<DatasetDuration> DatasetsDurations { get; set; } = new List<DatasetDuration>();
    }
}