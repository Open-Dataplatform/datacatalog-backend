using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;

using DataCatalog.Data.Model;
using DataCatalog.Api.Repositories;
using DataCatalog.Api.Services;
using FluentAssertions;
using Moq;
using Xunit;
using DataCatalog.Api.Data;

namespace DataCatalog.Api.UnitTests.Services
{
    public class HierarchyServiceTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

        public HierarchyServiceTests()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

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
            var hierarchyRepositoryMock = new Mock<IHierarchyRepository>();
            var hierarchyEntities = _fixture.Create<IEnumerable<Hierarchy>>();
            hierarchyRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(hierarchyEntities);
            _fixture.Inject(hierarchyRepositoryMock.Object);
            _fixture.Freeze<IHierarchyRepository>();
            var hierarchyService = _fixture.Create<HierarchyService>();

            // Act
            var hierarchies = await hierarchyService.ListAsync();

            // Assert
            var hierarchiesArray = hierarchies as Data.Domain.Hierarchy[] ?? hierarchies.ToArray();
            hierarchiesArray.Should().NotBeNull();
            hierarchiesArray.Length.Should().Be(3);
        }

        [Fact]
        public async Task FindByIdAsync_CalledWithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var hierarchyRepositoryMock = new Mock<IHierarchyRepository>();
            var invalidId = Guid.NewGuid();
            hierarchyRepositoryMock.Setup(x => x.FindByIdAsync(invalidId)).ReturnsAsync((Hierarchy)null);
            _fixture.Inject(hierarchyRepositoryMock.Object);
            _fixture.Freeze<IHierarchyRepository>();
            var hierarchyService = _fixture.Create<HierarchyService>();

            // Act
            var hierarchy = await hierarchyService.FindByIdAsync(invalidId);

            // Assert
            hierarchy.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_CalledWithValidId_ShouldReturnHierarchy()
        {
            // Arrange
            var hierarchyRepositoryMock = new Mock<IHierarchyRepository>();
            var hierarchyEntity = _fixture.Create<Hierarchy>();
            hierarchyRepositoryMock.Setup(x => x.FindByIdAsync(hierarchyEntity.Id)).ReturnsAsync(hierarchyEntity);
            _fixture.Inject(hierarchyRepositoryMock.Object);
            _fixture.Freeze<IHierarchyRepository>();
            var hierarchyService = _fixture.Create<HierarchyService>();

            // Act
            var hierarchy = await hierarchyService.FindByIdAsync(hierarchyEntity.Id);

            // Assert
            hierarchy.Should().NotBeNull();
        }

        [Fact]
        public async Task SaveAsync_MustCallUpdateAndComplete()
        {
            // Arrange
            var hierarchyRepositoryMock = new Mock<IHierarchyRepository>();
            var hierarchyEntity = _fixture.Create<Data.Domain.Hierarchy>();
            hierarchyRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Hierarchy>()));
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(hierarchyRepositoryMock.Object);
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IHierarchyRepository>();
            _fixture.Freeze<IUnitIOfWork>();
            var hierarchyService = _fixture.Create<HierarchyService>();

            // Act
            await hierarchyService.SaveAsync(hierarchyEntity);

            // Assert
            hierarchyRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Hierarchy>()), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_InvalidHierarchyId_MustNotCallUpdateOrComplete()
        {
            // Arrange
            var hierarchyRepositoryMock = new Mock<IHierarchyRepository>();
            var hierarchyEntity = _fixture.Create<Data.Domain.Hierarchy>();
            hierarchyRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Hierarchy)null);
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(hierarchyRepositoryMock.Object);
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IHierarchyRepository>();
            _fixture.Freeze<IUnitIOfWork>();
            var hierarchyService = _fixture.Create<HierarchyService>();

            // Act
            await hierarchyService.UpdateAsync(hierarchyEntity);

            // Assert
            hierarchyRepositoryMock.Verify(mock => mock.Update(It.IsAny<Hierarchy>()), Times.Never());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Never());
        }

        [Fact]
        public void UpdateAsync_HaveCyclicProblem_ShouldThrowException()
        {
            // Arrange
            var hierarchyRepositoryMock = new Mock<IHierarchyRepository>();
            var hierarchyEntities = _fixture.Create<IEnumerable<Hierarchy>>().ToList();
            hierarchyEntities[0].ParentHierarchyId = null;
            hierarchyEntities[1].ParentHierarchyId = hierarchyEntities[0].Id;
            hierarchyEntities[2].ParentHierarchyId = hierarchyEntities[1].Id;
            hierarchyRepositoryMock.Setup(x => x.FindByIdAsync(hierarchyEntities[0].Id)).ReturnsAsync(hierarchyEntities[0]);
            hierarchyRepositoryMock.Setup(x => x.FindByIdAsync(hierarchyEntities[1].Id)).ReturnsAsync(hierarchyEntities[1]);
            hierarchyRepositoryMock.Setup(x => x.FindByIdAsync(hierarchyEntities[2].Id)).ReturnsAsync(hierarchyEntities[2]);
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Inject(hierarchyRepositoryMock.Object);
            _fixture.Freeze<IHierarchyRepository>();
            _fixture.Freeze<IHierarchyRepository>();
            var hierarchyToUpdate = _fixture.Create<Data.Domain.Hierarchy>();
            hierarchyToUpdate.Id = hierarchyEntities[0].Id;
            hierarchyToUpdate.ParentHierarchyId = hierarchyEntities[2].Id;
            var hierarchyService = _fixture.Create<HierarchyService>();

            // ACT / ASSERT
            Func<Task> f = async () => await hierarchyService.UpdateAsync(hierarchyToUpdate);
            f.Should().Throw<Exception>().WithMessage("Cyclic hierarchies are not allowed");
        }


        [Fact]
        public async Task UpdateAsync_ValidHierarchyId_MustCallUpdateAndComplete()
        {
            // Arrange
            var hierarchyRepositoryMock = new Mock<IHierarchyRepository>();
            var hierarchyEntities = _fixture.Create<IEnumerable<Hierarchy>>().ToList();
            hierarchyEntities[0].ParentHierarchyId = null;
            hierarchyEntities[1].ParentHierarchyId = hierarchyEntities[0].Id;
            hierarchyEntities[2].ParentHierarchyId = null;
            hierarchyRepositoryMock.Setup(x => x.FindByIdAsync(hierarchyEntities[0].Id)).ReturnsAsync(hierarchyEntities[0]);
            hierarchyRepositoryMock.Setup(x => x.FindByIdAsync(hierarchyEntities[1].Id)).ReturnsAsync(hierarchyEntities[1]);
            hierarchyRepositoryMock.Setup(x => x.FindByIdAsync(hierarchyEntities[2].Id)).ReturnsAsync(hierarchyEntities[2]);
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Inject(hierarchyRepositoryMock.Object);
            _fixture.Freeze<IHierarchyRepository>();
            _fixture.Freeze<IHierarchyRepository>();
            var hierarchyToUpdate = _fixture.Create<Data.Domain.Hierarchy>();
            hierarchyToUpdate.Id = hierarchyEntities[2].Id;
            hierarchyToUpdate.ParentHierarchyId = hierarchyEntities[1].Id;
            var hierarchyService = _fixture.Create<HierarchyService>();

            // Act
            await hierarchyService.UpdateAsync(hierarchyToUpdate);

            // Assert
            hierarchyRepositoryMock.Verify(mock => mock.Update(hierarchyEntities[2]), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_InvalidHierarchyId_MustNotCallDeleteOrComplete()
        {
            // Arrange
            var hierarchyRepositoryMock = new Mock<IHierarchyRepository>();
            var hierarchyEntity = _fixture.Create<Data.Domain.Hierarchy>();
            hierarchyRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Hierarchy)null);
            _fixture.Inject(hierarchyRepositoryMock.Object);
            _fixture.Freeze<IHierarchyRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IHierarchyRepository>();
            var hierarchyService = _fixture.Create<HierarchyService>();

            // Act
            await hierarchyService.DeleteAsync(hierarchyEntity.Id);

            // Assert
            hierarchyRepositoryMock.Verify(mock => mock.Update(It.IsAny<Hierarchy>()), Times.Never());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Never());
        }

        [Fact]
        public async Task DeleteAsync_ValidHierarchyId_MustCallUpdateAndComplete()
        {
            // Arrange
            var hierarchyRepositoryMock = new Mock<IHierarchyRepository>();
            var hierarchyEntity = _fixture.Create<Hierarchy>();
            var domainModelEntity = _fixture.Create<Data.Domain.Hierarchy>();
            domainModelEntity.Id = hierarchyEntity.Id;
            hierarchyRepositoryMock.Setup(x => x.FindByIdAsync(hierarchyEntity.Id)).ReturnsAsync(hierarchyEntity);
            _fixture.Inject(hierarchyRepositoryMock.Object);
            _fixture.Freeze<IHierarchyRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IHierarchyRepository>();
            var hierarchyService = _fixture.Create<HierarchyService>();

            // Act
            await hierarchyService.DeleteAsync(domainModelEntity.Id);

            // Assert
            hierarchyRepositoryMock.Verify(mock => mock.Remove(hierarchyEntity), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }
    }
}
