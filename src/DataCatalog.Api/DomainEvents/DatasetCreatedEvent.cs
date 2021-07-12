using System;
using MediatR;

namespace DataCatalog.Api.DomainEvents
{
    public class DatasetCreatedEvent : INotification 
    {
        public Guid DatasetId { get; set; }
        public string DatasetName { get; set; }
        public string Container { get; set; }
        public string Hierarchy { get; set; }
        public string Owner { get; set; }
        public bool Public { get; set; }
    }
}