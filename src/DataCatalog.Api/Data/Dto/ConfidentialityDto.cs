﻿using DataCatalog.Api.Data.Common;

namespace DataCatalog.Api.Data.Dto
{
    public class ConfidentialityResponse : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}