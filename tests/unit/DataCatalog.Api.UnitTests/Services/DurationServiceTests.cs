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
    public class DurationServiceTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

        public DurationServiceTests()
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
            var durationRepositoryMock = new Mock<IDurationRepository>();
            var durationEntities = _fixture.Create<IEnumerable<Duration>>();
            durationRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(durationEntities);
            _fixture.Inject(durationRepositoryMock.Object);
            _fixture.Freeze<IDurationRepository>();
            var durationService = _fixture.Create<DurationService>();

            // Act
            var durations = await durationService.ListAsync();

            // Assert
            var durationsArray = durations as Data.Domain.Duration[] ?? durations.ToArray();
            durationsArray.Should().NotBeNull();
            durationsArray.Length.Should().Be(3);
        }

        [Fact]
        public async Task FindByIdAsync_CalledWithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var durationRepositoryMock = new Mock<IDurationRepository>();
            var invalidId = Guid.NewGuid();
            durationRepositoryMock.Setup(x => x.FindByIdAsync(invalidId)).ReturnsAsync((Duration)null);
            _fixture.Inject(durationRepositoryMock.Object);
            _fixture.Freeze<IDurationRepository>();
            var durationService = _fixture.Create<DurationService>();

            // Act
            var duration = await durationService.FindByIdAsync(invalidId);

            // Assert
            duration.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_CalledWithValidId_ShouldReturnDuration()
        {
            // Arrange
            var durationRepositoryMock = new Mock<IDurationRepository>();
            var durationEntity = _fixture.Create<Duration>();
            durationRepositoryMock.Setup(x => x.FindByIdAsync(durationEntity.Id)).ReturnsAsync(durationEntity);
            _fixture.Inject(durationRepositoryMock.Object);
            _fixture.Freeze<IDurationRepository>();
            var durationService = _fixture.Create<DurationService>();

            // Act
            var duration = await durationService.FindByIdAsync(durationEntity.Id);

            // Assert
            duration.Should().NotBeNull();
        }

        [Fact]
        public async Task SaveAsync_MustCallUpdateAndComplete()
        {
            // Arrange
            var durationRepositoryMock = new Mock<IDurationRepository>();
            var durationEntity = _fixture.Create<Data.Domain.Duration>();
            durationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Duration>()));
            _fixture.Inject(durationRepositoryMock.Object);
            _fixture.Freeze<IDurationRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDurationRepository>();
            var durationService = _fixture.Create<DurationService>();

            // Act
            await durationService.SaveAsync(durationEntity);

            // Assert
            durationRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Duration>()), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_InvalidDurationId_MustNotCallUpdateOrComplete()
        {
            // Arrange
            var durationRepositoryMock = new Mock<IDurationRepository>();
            var durationEntity = _fixture.Create<Data.Domain.Duration>();
            durationRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Duration)null);
            _fixture.Inject(durationRepositoryMock.Object);
            _fixture.Freeze<IDurationRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDurationRepository>();
            var durationService = _fixture.Create<DurationService>();

            // Act
            await durationService.UpdateAsync(durationEntity);

            // Assert
            durationRepositoryMock.Verify(mock => mock.Update(It.IsAny<Duration>()), Times.Never());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Never());
        }

        [Fact]
        public async Task UpdateAsync_ValidDurationId_MustCallUpdateAndComplete()
        {
            // Arrange
            var durationRepositoryMock = new Mock<IDurationRepository>();
            var durationEntity = _fixture.Create<Duration>();
            var domainModelEntity = _fixture.Create<Data.Domain.Duration>();
            domainModelEntity.Id = durationEntity.Id;
            durationRepositoryMock.Setup(x => x.FindByIdAsync(durationEntity.Id)).ReturnsAsync(durationEntity);
            _fixture.Inject(durationRepositoryMock.Object);
            _fixture.Freeze<IDurationRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDurationRepository>();
            var durationService = _fixture.Create<DurationService>();

            // Act
            await durationService.UpdateAsync(domainModelEntity);

            // Assert
            durationRepositoryMock.Verify(mock => mock.Update(durationEntity), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_InvalidDurationId_MustNotCallRemoveOrComplete()
        {
            // Arrange
            var durationRepositoryMock = new Mock<IDurationRepository>();
            var durationEntity = _fixture.Create<Data.Domain.Duration>();
            durationRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Duration)null);
            _fixture.Inject(durationRepositoryMock.Object);
            _fixture.Freeze<IDurationRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDurationRepository>();
            var durationService = _fixture.Create<DurationService>();

            // Act
            await durationService.DeleteAsync(durationEntity.Id);

            // Assert
            durationRepositoryMock.Verify(mock => mock.Update(It.IsAny<Duration>()), Times.Never());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Never());
        }

        [Fact]
        public async Task DeleteAsync_ValidDurationId_MustCallRemoveAndComplete()
        {
            // Arrange
            var durationRepositoryMock = new Mock<IDurationRepository>();
            var durationEntity = _fixture.Create<Duration>();
            var domainModelEntity = _fixture.Create<Data.Domain.Duration>();
            domainModelEntity.Id = durationEntity.Id;
            durationRepositoryMock.Setup(x => x.FindByIdAsync(durationEntity.Id)).ReturnsAsync(durationEntity);
            _fixture.Inject(durationRepositoryMock.Object);
            _fixture.Freeze<IDurationRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDurationRepository>();
            var durationService = _fixture.Create<DurationService>();

            // Act
            await durationService.DeleteAsync(domainModelEntity.Id);

            // Assert
            durationRepositoryMock.Verify(mock => mock.Remove(durationEntity), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }
    }
}
