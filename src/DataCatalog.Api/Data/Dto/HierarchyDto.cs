using DataCatalog.Api.Data.Common;
using System;

namespace DataCatalog.Api.Data.Dto
{
    public class HierarchyCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ParentHierarchyId { get; set; }
    }

    public class HierarchyUpdateRequest : HierarchyCreateRequest
    {
        public Guid Id { get; set; }
    }

    public class HierarchyResponse: Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ParentHierarchyId { get; set; }

        public HierarchyResponse[] ChildHierarchies { get; set; }
    }
}
