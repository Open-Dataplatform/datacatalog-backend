using DataCatalog.Api.Data.Common;
using System;
using System.Collections.Generic;

namespace DataCatalog.Api.Data.Model
{
    public class Category : ReplicantEntity
    {
        public string Name { get; set; }
        public string Colour { get; set; }
        public Uri ImageUri { get; set; }

        public List<DatasetCategory> DatasetCategories { get; set; } = new List<DatasetCategory>();
    }
}