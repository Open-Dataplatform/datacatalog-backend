using DataCatalog.Common.Data;
using DataCatalog.Common.Interfaces;
using System;

namespace DataCatalog.Api.Data.Dto
{
    public class CategoryCreateRequest
    {
        public string Name { get; set; }
        public string Colour { get; set; }
        public Uri ImageUri { get; set; }        
    }

    public class CategoryUpdateRequest : CategoryCreateRequest, IUpdateRequest
    {
        public Guid Id { get; set; }
    }

    public class CategoryResponse : GuidId
    {
        public string Name { get; set; }
        public string Colour { get; set; }
        public Uri ImageUri { get; set; }
    }

    public class CategoryColourResponse
    {
        public string Colour { get; set; }
    }
}