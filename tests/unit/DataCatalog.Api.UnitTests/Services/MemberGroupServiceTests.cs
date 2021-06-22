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
    public class MemberGroupServiceTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

        public MemberGroupServiceTests()
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
            var memberGroupRepositoryMock = new Mock<IMemberGroupRepository>();
            var memberGroupEntities = _fixture.Create<IEnumerable<MemberGroup>>();
            memberGroupRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(memberGroupEntities);
            _fixture.Inject(memberGroupRepositoryMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var memberGroupService = _fixture.Create<MemberGroupService>();

            // Act
            var memberGroups = await memberGroupService.ListAsync();

            // Assert
            var memberGroupsArray = memberGroups as Data.Domain.MemberGroup[] ?? memberGroups.ToArray();
            memberGroupsArray.Should().NotBeNull();
            memberGroupsArray.Length.Should().Be(3);
        }

        [Fact]
        public async Task FindByIdAsync_CalledWithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var memberGroupRepositoryMock = new Mock<IMemberGroupRepository>();
            var invalidId = Guid.NewGuid();
            memberGroupRepositoryMock.Setup(x => x.FindByIdAsync(invalidId)).ReturnsAsync((MemberGroup)null);
            _fixture.Inject(memberGroupRepositoryMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var memberGroupService = _fixture.Create<MemberGroupService>();

            // Act
            var memberGroup = await memberGroupService.FindByIdAsync(invalidId);

            // Assert
            memberGroup.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_CalledWithValidId_ShouldReturnMemberGroup()
        {
            // Arrange
            var memberGroupRepositoryMock = new Mock<IMemberGroupRepository>();
            var memberGroupEntity = _fixture.Create<MemberGroup>();
            memberGroupRepositoryMock.Setup(x => x.FindByIdAsync(memberGroupEntity.Id)).ReturnsAsync(memberGroupEntity);
            _fixture.Inject(memberGroupRepositoryMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var memberGroupService = _fixture.Create<MemberGroupService>();

            // Act
            var memberGroup = await memberGroupService.FindByIdAsync(memberGroupEntity.Id);

            // Assert
            memberGroup.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMemberGroups_ShouldReturnMemberGroups()
        {
            // Arrange
            var memberGroupRepositoryMock = new Mock<IMemberGroupRepository>();
            var memberGroupsEntity = _fixture.Create<IEnumerable<MemberGroup>>();
            var memberGuid = Guid.NewGuid();
            memberGroupRepositoryMock.Setup(x => x.GetMemberGroups(memberGuid)).ReturnsAsync(memberGroupsEntity);
            _fixture.Inject(memberGroupRepositoryMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var memberGroupService = _fixture.Create<MemberGroupService>();

            // Act
            var memberGroups = await memberGroupService.GetMemberGroups(memberGuid);

            // Assert
            var memberGroupArray = memberGroups as Data.Domain.MemberGroup[] ?? memberGroups.ToArray();
            memberGroupArray.Should().NotBeNull();
            memberGroupArray.Length.Should().Be(3);
        }

        [Fact]
        public async Task SaveAsync_MustCallUpdateAndComplete()
        {
            // Arrange
            var memberGroupRepositoryMock = new Mock<IMemberGroupRepository>();
            var memberGroupEntity = _fixture.Create<Data.Domain.MemberGroup>();
            memberGroupRepositoryMock.Setup(x => x.AddAsync(It.IsAny<MemberGroup>()));
            _fixture.Inject(memberGroupRepositoryMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var memberGroupService = _fixture.Create<MemberGroupService>();

            // Act
            await memberGroupService.SaveAsync(memberGroupEntity);

            // Assert
            memberGroupRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<MemberGroup>()), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_InvalidMemberGroupId_MustNotCallUpdateOrComplete()
        {
            // Arrange
            var memberGroupRepositoryMock = new Mock<IMemberGroupRepository>();
            var memberGroupEntity = _fixture.Create<Data.Domain.MemberGroup>();
            memberGroupRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((MemberGroup)null);
            _fixture.Inject(memberGroupRepositoryMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var memberGroupService = _fixture.Create<MemberGroupService>();

            // Act
            await memberGroupService.UpdateAsync(memberGroupEntity);

            // Assert
            memberGroupRepositoryMock.Verify(mock => mock.Update(It.IsAny<MemberGroup>()), Times.Never());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Never());
        }

        [Fact]
        public async Task UpdateAsync_ValidMemberGroupId_MustCallUpdateAndComplete()
        {
            // Arrange
            var memberGroupRepositoryMock = new Mock<IMemberGroupRepository>();
            var memberGroupEntity = _fixture.Create<MemberGroup>();
            var domainModelEntity = _fixture.Create<Data.Domain.MemberGroup>();
            domainModelEntity.Id = memberGroupEntity.Id;
            memberGroupRepositoryMock.Setup(x => x.FindByIdAsync(memberGroupEntity.Id)).ReturnsAsync(memberGroupEntity);
            _fixture.Inject(memberGroupRepositoryMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var memberGroupService = _fixture.Create<MemberGroupService>();

            // Act
            await memberGroupService.UpdateAsync(domainModelEntity);

            // Assert
            memberGroupRepositoryMock.Verify(mock => mock.Update(memberGroupEntity), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task AddMemberAsync_MustCallAddMemberAsyncAndComplete()
        {
            // Arrange
            var memberGroupRepositoryMock = new Mock<IMemberGroupRepository>();
            _fixture.Inject(memberGroupRepositoryMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var memberGroupId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var memberGroupService = _fixture.Create<MemberGroupService>();

            // Act
            await memberGroupService.AddMemberAsync(memberGroupId, memberId);

            // Assert
            memberGroupRepositoryMock.Verify(mock => mock.AddMemberAsync(memberGroupId, memberId), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task RemoveMemberAsync_MustCallAddMemberAsyncAndComplete()
        {
            // Arrange
            var memberGroupRepositoryMock = new Mock<IMemberGroupRepository>();
            _fixture.Inject(memberGroupRepositoryMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var memberGroupId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var memberGroupService = _fixture.Create<MemberGroupService>();

            // Act
            await memberGroupService.RemoveMemberAsync(memberGroupId, memberId);

            // Assert
            memberGroupRepositoryMock.Verify(mock => mock.RemoveMemberAsync(memberGroupId, memberId), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_InvalidMemberGroupId_MustNotCallDeleteOrComplete()
        {
            // Arrange
            var memberGroupRepositoryMock = new Mock<IMemberGroupRepository>();
            var memberGroupEntity = _fixture.Create<Data.Domain.MemberGroup>();
            memberGroupRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((MemberGroup)null);
            _fixture.Inject(memberGroupRepositoryMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var memberGroupService = _fixture.Create<MemberGroupService>();

            // Act
            await memberGroupService.DeleteAsync(memberGroupEntity.Id);

            // Assert
            memberGroupRepositoryMock.Verify(mock => mock.Update(It.IsAny<MemberGroup>()), Times.Never());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Never());
        }

        [Fact]
        public async Task DeleteAsync_ValidMemberGroupId_MustCallUpdateAndComplete()
        {
            // Arrange
            var memberGroupRepositoryMock = new Mock<IMemberGroupRepository>();
            var memberGroupEntity = _fixture.Create<MemberGroup>();
            var domainModelEntity = _fixture.Create<Data.Domain.MemberGroup>();
            domainModelEntity.Id = memberGroupEntity.Id;
            memberGroupRepositoryMock.Setup(x => x.FindByIdAsync(memberGroupEntity.Id)).ReturnsAsync(memberGroupEntity);
            _fixture.Inject(memberGroupRepositoryMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IMemberGroupRepository>();
            var memberGroupService = _fixture.Create<MemberGroupService>();

            // Act
            await memberGroupService.DeleteAsync(domainModelEntity.Id);

            // Assert
            memberGroupRepositoryMock.Verify(mock => mock.Remove(memberGroupEntity), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }
    }
}
