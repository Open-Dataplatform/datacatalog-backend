using System;
using DataCatalog.Common.Enums;
using MediatR;

namespace DataCatalog.Api.DomainEvents
{
    public class DatasetProvisionedEvent : INotification
    {
        public Guid DatasetId { get; set; }
        public ProvisionDatasetStatusEnum Status { get; set; }
        public string Error { get; set; }
    }
}