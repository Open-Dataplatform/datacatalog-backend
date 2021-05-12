using DataCatalog.Api.Data.Common;
using System;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Model
{
    public class Hierarchy : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid? ParentHierarchyId { get; set; }

        public Hierarchy ParentHierarchy { get; set; }

        public List<Hierarchy> ChildHierarchies { get; set; } = new List<Hierarchy>();
        public List<Dataset> Datasets { get; set; } = new List<Dataset>();
    }
}