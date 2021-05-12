using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using DataCatalog.Api.Data;
using DataCatalog.Api.Data.Model;
using DataCatalog.Api.Repositories;
using DataCatalog.Api.Services;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Energinet.DataPlatform.Shared.Environments;
using Xunit;

namespace DataCatalog.Api.UnitTests.Services
{
    public class DataContractServiceTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

        public DataContractServiceTests()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var environmentMock = new Mock<IEnvironment>();
            environmentMock.Setup(x => x.Name).Returns("test");
            _fixture.Inject(environmentMock.Object);

            // Setup automapper
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperProfile>();
            });

            var mapper = config.CreateMapper();
            _fixture.Inject(mapper);
            _fixture.Freeze<IMapper>();
        }

        [Fact]
        public async Task ListAsync_ShouldReturnList()
        {
            // Arrange
            var dataContractRepositoryMock = new Mock<IDataContractRepository>();
            var dataContractEntities = _fixture.Create<IEnumerable<DataContract>>();
            dataContractRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(dataContractEntities);
            _fixture.Inject(dataContractRepositoryMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var dataContractService = _fixture.Create<DataContractService>();

            // Act
            var dataContracts = await dataContractService.ListAsync();

            // Assert
            var dataContractsArray = dataContracts as Data.Domain.DataContract[] ?? dataContracts.ToArray();
            dataContractsArray.Should().NotBeNull();
            dataContractsArray.Length.Should().Be(3);
        }

        [Fact]
        public async Task FindByIdAsync_CalledWithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var dataContractRepositoryMock = new Mock<IDataContractRepository>();
            var invalidId = Guid.NewGuid();
            dataContractRepositoryMock.Setup(x => x.FindByIdAsync(invalidId)).ReturnsAsync((DataContract)null);
            _fixture.Inject(dataContractRepositoryMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var dataContractService = _fixture.Create<DataContractService>();

            // Act
            var dataContract = await dataContractService.FindByIdAsync(invalidId);

            // Assert
            dataContract.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_CalledWithValidId_ShouldReturnDataContract()
        {
            // Arrange
            var dataContractRepositoryMock = new Mock<IDataContractRepository>();
            var dataContractEntity = _fixture.Create<DataContract>();
            dataContractRepositoryMock.Setup(x => x.FindByIdAsync(dataContractEntity.Id)).ReturnsAsync(dataContractEntity);
            _fixture.Inject(dataContractRepositoryMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var dataContractService = _fixture.Create<DataContractService>();

            // Act
            var dataContract = await dataContractService.FindByIdAsync(dataContractEntity.Id);

            // Assert
            dataContract.Should().NotBeNull();
            dataContract.Id.Should().Be(dataContractEntity.Id);
        }

        [Fact]
        public async Task GetByDatasetIdAsync_CalledWithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var dataContractRepositoryMock = new Mock<IDataContractRepository>();
            var invalidId = Guid.NewGuid();
            dataContractRepositoryMock.Setup(x => x.GetByDatasetIdAsync(invalidId)).ReturnsAsync(new List<DataContract>());
            _fixture.Inject(dataContractRepositoryMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var dataContractService = _fixture.Create<DataContractService>();

            // Act
            var dataContracts = await dataContractService.GetByDatasetIdAsync(invalidId);

            // Assert
            dataContracts.Should().NotBeNull();
            dataContracts.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByDatasetIdAsync_CalledWithValidId_ShouldReturnDataContract()
        {
            // Arrange
            var dataContractRepositoryMock = new Mock<IDataContractRepository>();
            var dataContractEntities = _fixture.Create<List<DataContract>>();
            dataContractRepositoryMock.Setup(x => x.GetByDatasetIdAsync(dataContractEntities.First().Dataset.Id)).ReturnsAsync(new List<DataContract> { dataContractEntities.First() });
            _fixture.Inject(dataContractRepositoryMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var dataContractService = _fixture.Create<DataContractService>();

            // Act
            var dataContracts = await dataContractService.GetByDatasetIdAsync(dataContractEntities.First().Dataset.Id);

            // Assert
            dataContracts.Should().NotBeNull();
            dataContracts.Length.Should().Be(1);
            dataContracts.First().Id.Should().Be(dataContractEntities.First().Id);
        }

        [Fact]
        public async Task GetByDataSourceIdAsync_CalledWithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var dataContractRepositoryMock = new Mock<IDataContractRepository>();
            var invalidId = Guid.NewGuid();
            dataContractRepositoryMock.Setup(x => x.GetByDataSourceIdAsync(invalidId)).ReturnsAsync(new List<DataContract>());
            _fixture.Inject(dataContractRepositoryMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var dataContractService = _fixture.Create<DataContractService>();

            // Act
            var dataContracts = await dataContractService.GetByDataSourceIdAsync(invalidId);

            // Assert
            dataContracts.Should().NotBeNull();
            dataContracts.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByDataSourceIdAsync_CalledWithValidId_ShouldReturnDataContract()
        {
            // Arrange
            var dataContractRepositoryMock = new Mock<IDataContractRepository>();
            var dataContractEntities = _fixture.Create<List<DataContract>>();
            dataContractRepositoryMock.Setup(x => x.GetByDataSourceIdAsync(dataContractEntities.First().DataSource.Id)).ReturnsAsync(new List<DataContract> { dataContractEntities.First() });
            _fixture.Inject(dataContractRepositoryMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var dataContractService = _fixture.Create<DataContractService>();

            // Act
            var dataContracts = await dataContractService.GetByDataSourceIdAsync(dataContractEntities.First().DataSource.Id);

            // Assert
            dataContracts.Should().NotBeNull();
            dataContracts.Length.Should().Be(1);
            dataContracts.First().Id.Should().Be(dataContractEntities.First().Id);
        }

        [Fact]
        public async Task SaveAsync_MustCallUpdateAndComplete()
        {
            // Arrange
            var dataContractRepositoryMock = new Mock<IDataContractRepository>();
            var dataContractEntity = _fixture.Create<Data.Domain.DataContract>();
            dataContractRepositoryMock.Setup(x => x.AddAsync(It.IsAny<DataContract>()));
            _fixture.Inject(dataContractRepositoryMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var dataContractService = _fixture.Create<DataContractService>();

            // Act
            await dataContractService.SaveAsync(dataContractEntity);

            // Assert
            dataContractRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<DataContract>()), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_InvalidDataContractId_MustNotCallUpdateOrComplete()
        {
            // Arrange
            var dataContractRepositoryMock = new Mock<IDataContractRepository>();
            var dataContractEntity = _fixture.Create<Data.Domain.DataContract>();
            dataContractRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((DataContract)null);
            _fixture.Inject(dataContractRepositoryMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var dataContractService = _fixture.Create<DataContractService>();

            // Act
            await dataContractService.UpdateAsync(dataContractEntity);

            // Assert
            dataContractRepositoryMock.Verify(mock => mock.Update(It.IsAny<DataContract>()), Times.Never());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Never());
        }

        [Fact]
        public async Task UpdateAsync_ValidDataContractId_MustCallUpdateAndComplete()
        {
            // Arrange
            var dataContractRepositoryMock = new Mock<IDataContractRepository>();
            var dataContractEntity = _fixture.Create<DataContract>();
            var domainModelEntity = _fixture.Create<Data.Domain.DataContract>();
            domainModelEntity.Id = dataContractEntity.Id;
            dataContractRepositoryMock.Setup(x => x.FindByIdAsync(dataContractEntity.Id)).ReturnsAsync(dataContractEntity);
            _fixture.Inject(dataContractRepositoryMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var dataContractService = _fixture.Create<DataContractService>();

            // Act
            await dataContractService.UpdateAsync(domainModelEntity);

            // Assert
            dataContractRepositoryMock.Verify(mock => mock.Update(dataContractEntity), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_InvalidDataContractId_MustNotCallDeleteOrComplete()
        {
            // Arrange
            var dataContractRepositoryMock = new Mock<IDataContractRepository>();
            var dataContractEntity = _fixture.Create<Data.Domain.DataContract>();
            dataContractRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((DataContract)null);
            _fixture.Inject(dataContractRepositoryMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var dataContractService = _fixture.Create<DataContractService>();

            // Act
            await dataContractService.DeleteAsync(dataContractEntity.Id);

            // Assert
            dataContractRepositoryMock.Verify(mock => mock.Update(It.IsAny<DataContract>()), Times.Never());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Never());
        }

        [Fact]
        public async Task DeleteAsync_ValidDataContractId_MustCallUpdateAndComplete()
        {
            // Arrange
            var dataContractRepositoryMock = new Mock<IDataContractRepository>();
            var dataContractEntity = _fixture.Create<DataContract>();
            var domainModelEntity = _fixture.Create<Data.Domain.DataContract>();
            domainModelEntity.Id = dataContractEntity.Id;
            dataContractRepositoryMock.Setup(x => x.FindByIdAsync(dataContractEntity.Id)).ReturnsAsync(dataContractEntity);
            _fixture.Inject(dataContractRepositoryMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDataContractRepository>();
            var dataContractService = _fixture.Create<DataContractService>();

            // Act
            await dataContractService.DeleteAsync(domainModelEntity.Id);

            // Assert
            dataContractRepositoryMock.Verify(mock => mock.Remove(dataContractEntity), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }
    }
}
