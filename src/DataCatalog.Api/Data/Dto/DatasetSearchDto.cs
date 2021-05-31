using DataCatalog.Common.Enums;
using System;

namespace DataCatalog.Api.Data.Dto
{
    public abstract class DatasetSearchRequest
    {
        public SortType SortType { get; set; }
        public int Take { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }

    public class DatasetSearchByCategoryRequest : DatasetSearchRequest
    {
        public Guid CategoryId { get; set; }
    }

    public class DatasetSearchByTermRequest : DatasetSearchRequest
    {
        public string SearchTerm { get; set; }
    }
}