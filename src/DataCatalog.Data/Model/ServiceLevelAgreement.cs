using System.Collections.Generic;
using DataCatalog.Common.Data;

namespace DataCatalog.Data.Model
{
    public class ServiceLevelAgreement : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }

        public List<Dataset> Datasets { get; set; }
    }
}