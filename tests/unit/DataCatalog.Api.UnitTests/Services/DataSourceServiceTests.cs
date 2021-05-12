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
    public class DataSourceServiceTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

        public DataSourceServiceTests()
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
            var dataSourceRepositoryMock = new Mock<IDataSourceRepository>();
            var dataSourceEntities = _fixture.Create<IEnumerable<DataSource>>();
            dataSourceRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(dataSourceEntities);
            _fixture.Inject(dataSourceRepositoryMock.Object);
            _fixture.Freeze<IDataSourceRepository>();
            var dataSourceService = _fixture.Create<DataSourceService>();

            // Act
            var dataSources = await dataSourceService.ListAsync();

            // Assert
            var dataSourcesArray = dataSources as Data.Domain.DataSource[] ?? dataSources.ToArray();
            dataSourcesArray.Should().NotBeNull();
            dataSourcesArray.Length.Should().Be(3);
        }

        [Fact]
        public async Task FindByIdAsync_CalledWithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var dataSourceRepositoryMock = new Mock<IDataSourceRepository>();
            var invalidId = Guid.NewGuid();
            dataSourceRepositoryMock.Setup(x => x.FindByIdAsync(invalidId)).ReturnsAsync((DataSource)null);
            _fixture.Inject(dataSourceRepositoryMock.Object);
            _fixture.Freeze<IDataSourceRepository>();
            var dataSourceService = _fixture.Create<DataSourceService>();

            // Act
            var dataSource = await dataSourceService.FindByIdAsync(invalidId);

            // Assert
            dataSource.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_CalledWithValidId_ShouldReturnDataSource()
        {
            // Arrange
            var dataSourceRepositoryMock = new Mock<IDataSourceRepository>();
            var dataSourceEntity = _fixture.Create<DataSource>();
            dataSourceRepositoryMock.Setup(x => x.FindByIdAsync(dataSourceEntity.Id)).ReturnsAsync(dataSourceEntity);
            _fixture.Inject(dataSourceRepositoryMock.Object);
            _fixture.Freeze<IDataSourceRepository>();
            var dataSourceService = _fixture.Create<DataSourceService>();

            // Act
            var dataSource = await dataSourceService.FindByIdAsync(dataSourceEntity.Id);

            // Assert
            dataSource.Should().NotBeNull();
        }

        [Fact]
        public async Task SaveAsync_MustCallUpdateAndComplete()
        {
            // Arrange
            var dataSourceRepositoryMock = new Mock<IDataSourceRepository>();
            var dataSourceEntity = _fixture.Create<Data.Domain.DataSource>();
            dataSourceRepositoryMock.Setup(x => x.AddAsync(It.IsAny<DataSource>()));
            _fixture.Inject(dataSourceRepositoryMock.Object);
            _fixture.Freeze<IDataSourceRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDataSourceRepository>();
            var dataSourceService = _fixture.Create<DataSourceService>();

            // Act
            await dataSourceService.SaveAsync(dataSourceEntity);

            // Assert
            dataSourceRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<DataSource>()), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_InvalidDataSourceId_MustNotCallUpdateOrComplete()
        {
            // Arrange
            var dataSourceRepositoryMock = new Mock<IDataSourceRepository>();
            var dataSourceEntity = _fixture.Create<Data.Domain.DataSource>();
            dataSourceRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((DataSource)null);
            _fixture.Inject(dataSourceRepositoryMock.Object);
            _fixture.Freeze<IDataSourceRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDataSourceRepository>();
            var dataSourceService = _fixture.Create<DataSourceService>();

            // Act
            await dataSourceService.UpdateAsync(dataSourceEntity);

            // Assert
            dataSourceRepositoryMock.Verify(mock => mock.Update(It.IsAny<DataSource>()), Times.Never());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Never());
        }

        [Fact]
        public async Task UpdateAsync_ValidDataSourceId_MustCallUpdateAndComplete()
        {
            // Arrange
            var dataSourceRepositoryMock = new Mock<IDataSourceRepository>();
            var dataSourceEntity = _fixture.Create<DataSource>();
            var domainModelEntity = _fixture.Create<Data.Domain.DataSource>();
            domainModelEntity.Id = dataSourceEntity.Id;
            dataSourceRepositoryMock.Setup(x => x.FindByIdAsync(dataSourceEntity.Id)).ReturnsAsync(dataSourceEntity);
            _fixture.Inject(dataSourceRepositoryMock.Object);
            _fixture.Freeze<IDataSourceRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDataSourceRepository>();
            var dataSourceService = _fixture.Create<DataSourceService>();

            // Act
            await dataSourceService.UpdateAsync(domainModelEntity);

            // Assert
            dataSourceRepositoryMock.Verify(mock => mock.Update(dataSourceEntity), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_InvalidDataSourceId_MustNotCallDeleteOrComplete()
        {
            // Arrange
            var dataSourceRepositoryMock = new Mock<IDataSourceRepository>();
            var dataSourceEntity = _fixture.Create<Data.Domain.DataSource>();
            dataSourceRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((DataSource)null);
            _fixture.Inject(dataSourceRepositoryMock.Object);
            _fixture.Freeze<IDataSourceRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDataSourceRepository>();
            var dataSourceService = _fixture.Create<DataSourceService>();

            // Act
            await dataSourceService.DeleteAsync(dataSourceEntity.Id);

            // Assert
            dataSourceRepositoryMock.Verify(mock => mock.Update(It.IsAny<DataSource>()), Times.Never());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Never());
        }

        [Fact]
        public async Task DeleteAsync_ValidDataSourceId_MustCallUpdateAndComplete()
        {
            // Arrange
            var dataSourceRepositoryMock = new Mock<IDataSourceRepository>();
            var dataSourceEntity = _fixture.Create<DataSource>();
            var domainModelEntity = _fixture.Create<Data.Domain.DataSource>();
            domainModelEntity.Id = dataSourceEntity.Id;
            dataSourceRepositoryMock.Setup(x => x.FindByIdAsync(dataSourceEntity.Id)).ReturnsAsync(dataSourceEntity);
            _fixture.Inject(dataSourceRepositoryMock.Object);
            _fixture.Freeze<IDataSourceRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDataSourceRepository>();
            var dataSourceService = _fixture.Create<DataSourceService>();

            // Act
            await dataSourceService.DeleteAsync(domainModelEntity.Id);

            // Assert
            dataSourceRepositoryMock.Verify(mock => mock.Remove(dataSourceEntity), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }
    }
}
