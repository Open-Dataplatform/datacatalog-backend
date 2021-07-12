using System;
using System.Threading;
using AutoFixture.Xunit2;
using DataCatalog.Api.DomainEventHandlers;
using DataCatalog.Api.DomainEvents;
using DataCatalog.Api.Repositories;
using DataCatalog.Api.UnitTests.AutoMoqAttribute;
using DataCatalog.Common.Enums;
using Moq;
using Xunit;

namespace DataCatalog.Api.UnitTests.DomainEventHandlers
{
    // ReSharper disable once InconsistentNaming
    public class DatasetProvisionedEventHandler_Should
    {
        [Theory]
        [MapperAutoMoq]
        public void SendAMessageUsingTheMessageBus(
            [Frozen] Mock<IDatasetRepository> datasetRepositoryMock,
            DatasetProvisionedEventHandler sut,
            DatasetProvisionedEvent datasetProvisionedEvent
        )
        {
            // Act
            sut.Handle(datasetProvisionedEvent, new CancellationToken());

            // Assert
            datasetRepositoryMock.Verify(x => x.UpdateProvisioningStatusAsync(It.Is<Guid>(datasetId => 
                datasetId.Equals(datasetProvisionedEvent.DatasetId)
            ), It.Is<ProvisionDatasetStatusEnum>(status => status == datasetProvisionedEvent.Status)), Times.Once);
        }
    }
}