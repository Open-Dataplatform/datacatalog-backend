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
    public class TransformationServiceTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

        public TransformationServiceTests()
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
            var transformationRepositoryMock = new Mock<ITransformationRepository>();
            var transformationEntities = _fixture.Create<IEnumerable<Transformation>>();
            transformationRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(transformationEntities);
            _fixture.Inject(transformationRepositoryMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var transformationService = _fixture.Create<TransformationService>();

            // Act
            var transformations = await transformationService.ListAsync();

            // Assert
            var transformationsArray = transformations as Data.Domain.Transformation[] ?? transformations.ToArray();
            transformationsArray.Should().NotBeNull();
            transformationsArray.Length.Should().Be(3);
        }

        [Fact]
        public async Task FindByIdAsync_CalledWithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var transformationRepositoryMock = new Mock<ITransformationRepository>();
            var invalidId = Guid.NewGuid();
            transformationRepositoryMock.Setup(x => x.FindByIdAsync(invalidId)).ReturnsAsync((Transformation)null);
            _fixture.Inject(transformationRepositoryMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var transformationService = _fixture.Create<TransformationService>();

            // Act
            var transformation = await transformationService.FindByIdAsync(invalidId);

            // Assert
            transformation.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_CalledWithValidId_ShouldReturnTransformation()
        {
            // Arrange
            var transformationRepositoryMock = new Mock<ITransformationRepository>();
            var transformationEntity = _fixture.Create<Transformation>();
            transformationRepositoryMock.Setup(x => x.FindByIdAsync(transformationEntity.Id)).ReturnsAsync(transformationEntity);
            _fixture.Inject(transformationRepositoryMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var transformationService = _fixture.Create<TransformationService>();

            // Act
            var transformation = await transformationService.FindByIdAsync(transformationEntity.Id);

            // Assert
            transformation.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByDatasetIdsAsync_CalledWithDatasetId_ShouldReturnTransformations()
        {
            // Arrange
            var transformationRepositoryMock = new Mock<ITransformationRepository>();
            var aDatasetId = Guid.NewGuid();
            var transformationEntity = _fixture.Create<Transformation>();
            transformationRepositoryMock.Setup(x => x.GetByDatasetIdsAsync(new []{ aDatasetId })).ReturnsAsync(new [] { transformationEntity});
            _fixture.Inject(transformationRepositoryMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var transformationService = _fixture.Create<TransformationService>();

            // Act
            var transformations = await transformationService.GetByDatasetIdsAsync(new[] { aDatasetId });

            // Assert
            var transformationsArray = transformations as Data.Domain.Transformation[] ?? transformations.ToArray();
            transformationsArray.Should().NotBeNull();
            transformationsArray.Count().Should().Be(1);
        }

        [Fact]
        public async Task SaveAsync_MustCallUpdateAndComplete()
        {
            // Arrange
            var transformationRepositoryMock = new Mock<ITransformationRepository>();
            var transformationEntity = _fixture.Create<Data.Domain.Transformation>();
            transformationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Transformation>()));
            _fixture.Inject(transformationRepositoryMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var transformationService = _fixture.Create<TransformationService>();

            // Act
            await transformationService.SaveAsync(transformationEntity);

            // Assert
            transformationRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Transformation>()), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_InvalidTransformationId_MustNotCallUpdateOrComplete()
        {
            // Arrange
            var transformationRepositoryMock = new Mock<ITransformationRepository>();
            var transformationEntity = _fixture.Create<Data.Domain.Transformation>();
            transformationRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Transformation)null);
            _fixture.Inject(transformationRepositoryMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var transformationService = _fixture.Create<TransformationService>();

            // Act
            await transformationService.UpdateAsync(transformationEntity);

            // Assert
            transformationRepositoryMock.Verify(mock => mock.Update(It.IsAny<Transformation>()), Times.Never());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Never());
        }

        [Fact]
        public async Task UpdateAsync_ValidTransformationId_MustCallUpdateAndComplete()
        {
            // Arrange
            var transformationRepositoryMock = new Mock<ITransformationRepository>();
            var transformationEntity = _fixture.Create<Transformation>();
            var domainModelEntity = _fixture.Create<Data.Domain.Transformation>();
            domainModelEntity.Id = transformationEntity.Id;
            transformationRepositoryMock.Setup(x => x.FindByIdAsync(transformationEntity.Id)).ReturnsAsync(transformationEntity);
            _fixture.Inject(transformationRepositoryMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var transformationService = _fixture.Create<TransformationService>();

            // Act
            await transformationService.UpdateAsync(domainModelEntity);

            // Assert
            transformationRepositoryMock.Verify(mock => mock.Update(transformationEntity), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_InvalidTransformationId_MustNotCallDeleteOrComplete()
        {
            // Arrange
            var transformationRepositoryMock = new Mock<ITransformationRepository>();
            var transformationEntity = _fixture.Create<Data.Domain.Transformation>();
            transformationRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Transformation)null);
            _fixture.Inject(transformationRepositoryMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var transformationService = _fixture.Create<TransformationService>();

            // Act
            await transformationService.DeleteAsync(transformationEntity.Id);

            // Assert
            transformationRepositoryMock.Verify(mock => mock.RemoveAsync(It.IsAny<Transformation>()), Times.Never());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Never());
        }

        [Fact]
        public async Task DeleteAsync_ValidTransformationId_MustCallUpdateAndComplete()
        {
            // Arrange
            var transformationRepositoryMock = new Mock<ITransformationRepository>();
            var transformationEntity = _fixture.Create<Transformation>();
            var domainModelEntity = _fixture.Create<Data.Domain.Transformation>();
            domainModelEntity.Id = transformationEntity.Id;
            transformationRepositoryMock.Setup(x => x.FindByIdAsync(transformationEntity.Id)).ReturnsAsync(transformationEntity);
            _fixture.Inject(transformationRepositoryMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var transformationService = _fixture.Create<TransformationService>();

            // Act
            await transformationService.DeleteAsync(domainModelEntity.Id);

            // Assert
            transformationRepositoryMock.Verify(mock => mock.RemoveAsync(transformationEntity), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }
    }
}
