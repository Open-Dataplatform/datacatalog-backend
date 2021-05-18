using System;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Domain
{
    public class Hierarchy
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ParentHierarchyId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<Hierarchy> ChildHierarchies { get; set; }
        public Hierarchy ParentHierarchy { get; set; }
        public List<Dataset> Datasets { get; set; } = new List<Dataset>();
    }
}
