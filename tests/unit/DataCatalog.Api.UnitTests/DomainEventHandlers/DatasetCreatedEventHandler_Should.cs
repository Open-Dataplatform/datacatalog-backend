using System.Threading;
using AutoFixture.Xunit2;
using DataCatalog.Api.DomainEventHandlers;
using DataCatalog.Api.DomainEvents;
using DataCatalog.Api.Messages;
using DataCatalog.Api.UnitTests.AutoMoqAttribute;
using Moq;
using Rebus.Bus;
using Xunit;

namespace DataCatalog.Api.UnitTests.DomainEventHandlers
{
    // ReSharper disable once InconsistentNaming
    public class DatasetCreatedEventHandler_Should
    {
        [Theory]
        [MapperAutoMoq]
        public void SendAMessageUsingTheMessageBus(
            [Frozen] Mock<IBus> busMock,
            DatasetCreatedEventHandler sut,
            DatasetCreatedEvent datasetCreatedEvent
            )
        {
            // Act
            sut.Handle(datasetCreatedEvent, new CancellationToken());

            // Assert
            busMock.Verify(x => x.Publish(It.Is<DatasetCreatedMessage>(m => 
                m.DatasetId.Equals(datasetCreatedEvent.DatasetId) &&
                m.Public.Equals(datasetCreatedEvent.Public) && 
                m.Container.Equals(datasetCreatedEvent.Container) &&
                m.Hierarchy.Equals(datasetCreatedEvent.Hierarchy) &&
                m.Owner.Equals(datasetCreatedEvent.Owner) &&
                m.DatasetName.Equals(datasetCreatedEvent.DatasetName)
                ), null), Times.Once);
        }
    }
}