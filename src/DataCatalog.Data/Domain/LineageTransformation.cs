using System;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Domain
{
    public class LineageTransformation
    {
        public Guid Id { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<LineageDataset> Datasets { get; set; } = new List<LineageDataset>();
    }
}
