using System;
using System.Threading.Tasks;
using DataCatalog.Api.DomainEvents;
using DataCatalog.Common.Enums;
using DataCatalog.DatasetResourceManagement.Messages;
using MediatR;
using Rebus.Handlers;

namespace DataCatalog.Api.MessageHandlers
{
    public class DatasetProvisionedHandler : IHandleMessages<DatasetProvisionedMessage>
    {
        private readonly IMediator _mediator;

        public DatasetProvisionedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Handle(DatasetProvisionedMessage message)
        {
            var parseSuccess = Enum.TryParse(message.Status, out ProvisionDatasetStatusEnum status);
            if (!parseSuccess)
            {
                throw new Exception($"Could not parse status to a ProvisionDatasetStatusEnum in field 'Status' with the value: {message.Status}");
            }

            return _mediator.Publish(new DatasetProvisionedEvent
            {
                DatasetId = message.DatasetId,
                Status = status,
                Error = message.Error
            });
        }
    }
}