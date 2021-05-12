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
using DataCatalog.Api.Data.Common;
using DataCatalog.Api.Enums;
using DataCatalog.Api.Exceptions;
using Energinet.DataPlatform.Shared.Environments;
using Xunit;

namespace DataCatalog.Api.UnitTests.Services
{
    public class DatasetGroupServiceTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

        public DatasetGroupServiceTests()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
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

        [Theory]
        [InlineData(Role.User, 1)]
        [InlineData(Role.DataSteward, 1)]
        [InlineData(Role.Admin, 3)]
        public async Task ListAsync_ShouldReturnList(Role role, int numberOfDatasetGroups)
        {
            // Arrange
            var datasetGroupRepositoryMock = new Mock<IDatasetGroupRepository>();
            var datasetGroupEntities = _fixture.Create<IEnumerable<DatasetGroup>>().ToList();
            datasetGroupRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(datasetGroupEntities);
            _fixture.Inject(datasetGroupRepositoryMock.Object);
            _fixture.Freeze<IDatasetGroupRepository>();
            var current = new Current {MemberId = datasetGroupEntities.First().MemberId, Roles = new List<Role> {role}};
            _fixture.Inject(current);
            var datasetGroupService = _fixture.Create<DatasetGroupService>();

            // Act
            var datasetGroups = await datasetGroupService.ListAsync();

            // Assert
            var datasetGroupsArray = datasetGroups as Data.Domain.DatasetGroup[] ?? datasetGroups.ToArray();
            datasetGroupsArray.Should().NotBeNull();
            datasetGroupsArray.Length.Should().Be(numberOfDatasetGroups);
        }

        [Fact]
        public async Task FindByIdAsync_InvalidId_ShouldReturnNull()
        {
            // Arrange
            var datasetGroupRepositoryMock = new Mock<IDatasetGroupRepository>();
            var invalidId = Guid.NewGuid();
            datasetGroupRepositoryMock.Setup(x => x.FindByIdAsync(invalidId)).ReturnsAsync((DatasetGroup)null);
            _fixture.Inject(datasetGroupRepositoryMock.Object);
            _fixture.Freeze<IDatasetGroupRepository>();
            var datasetGroupService = _fixture.Create<DatasetGroupService>();

            // Act
            var datasetGroup = await datasetGroupService.FindByIdAsync(invalidId);

            // Assert
            datasetGroup.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_HiddenIdForCaller_ShouldReturnNull()
        {
            // Arrange
            var datasetGroupRepositoryMock = new Mock<IDatasetGroupRepository>();
            var datasetGroup = _fixture.Create<DatasetGroup>();
            datasetGroup.MemberId = Guid.NewGuid();
            datasetGroupRepositoryMock.Setup(x => x.FindByIdAsync(datasetGroup.Id)).ReturnsAsync(datasetGroup);
            _fixture.Inject(datasetGroupRepositoryMock.Object);
            _fixture.Freeze<IDatasetGroupRepository>();
            var current = new Current {  Roles = new List<Role> {Role.DataSteward} };
            _fixture.Inject(current);
            var datasetGroupService = _fixture.Create<DatasetGroupService>();

            // Act
            var result = await datasetGroupService.FindByIdAsync(datasetGroup.Id);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_AllVisibleForAdmin_ShouldReturnNull()
        {
            // Arrange
            var datasetGroupRepositoryMock = new Mock<IDatasetGroupRepository>();
            var datasetGroup = _fixture.Create<DatasetGroup>();
            datasetGroup.MemberId = Guid.NewGuid();
            datasetGroupRepositoryMock.Setup(x => x.FindByIdAsync(datasetGroup.Id)).ReturnsAsync(datasetGroup);
            _fixture.Inject(datasetGroupRepositoryMock.Object);
            _fixture.Freeze<IDatasetGroupRepository>();
            var current = new Current { Roles = new List<Role> { Role.Admin } };
            _fixture.Inject(current);
            var datasetGroupService = _fixture.Create<DatasetGroupService>();

            // Act
            var result = await datasetGroupService.FindByIdAsync(datasetGroup.Id);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void SaveAsync_CalledWithUnknownMember_ShouldThrowException()
        {
            // Arrange
            var memberRepositoryMock = new Mock<IMemberRepository>();
            memberRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Member)null);
            _fixture.Inject(memberRepositoryMock);
            _fixture.Freeze<IMemberRepository>();
            var datasetGroupService = _fixture.Create<DatasetGroupService>();
            var datasetGroup = _fixture.Create<Data.Domain.DatasetGroup>();

            // Act / Assert
            Func<Task> f = async () => await datasetGroupService.SaveAsync(datasetGroup);
            f.Should().Throw<ValidationExceptionCollection>().WithMessage("*Member not found with id*");
        }

        [Fact]
        public void SaveAsync_CalledByDataStewardAndOtherMemberId_ShouldThrowException()
        {
            // Arrange
            var memberRepositoryMock = new Mock<IMemberRepository>();
            var member = _fixture.Create<Member>();
            memberRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(member);
            _fixture.Inject(memberRepositoryMock);
            _fixture.Freeze<IMemberRepository>();
            var current = new Current { MemberId = Guid.NewGuid(), Roles = new List<Role> {Role.DataSteward} };
            _fixture.Inject(current);
            var datasetGroup = _fixture.Create<Data.Domain.DatasetGroup>();
            var datasetGroupService = _fixture.Create<DatasetGroupService>();

            // Act / Assert
            Func<Task> f = async () => await datasetGroupService.SaveAsync(datasetGroup);
            f.Should().Throw<ValidationExceptionCollection>().WithMessage("*You can only create DatasetGroups for yourself*");
        }

        [Fact]
        public void SaveAsync_CalledWithUnknownDatasetId_ShouldThrowException()
        {
            // Arrange
            var memberRepositoryMock = new Mock<IMemberRepository>();
            var member = _fixture.Create<Member>();
            memberRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(member);
            _fixture.Inject(memberRepositoryMock);
            _fixture.Freeze<IMemberRepository>();
            var datasetRepositoryMock = new Mock<IDatasetRepository>();
            datasetRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Dataset)null);
            _fixture.Inject(datasetRepositoryMock.Object);
            _fixture.Freeze<IDatasetRepository>();
            var current = new Current { MemberId = Guid.NewGuid(), Roles = new List<Role> {Role.DataSteward} };
            _fixture.Inject(current);
            var datasetGroup = _fixture.Create<Data.Domain.DatasetGroup>();
            datasetGroup.MemberId = current.MemberId;
            var datasetGroupService = _fixture.Create<DatasetGroupService>();

            // Act / Assert
            Func<Task> f = async () => await datasetGroupService.SaveAsync(datasetGroup);
            f.Should().Throw<ValidationExceptionCollection>().WithMessage($"*Dataset with id '{datasetGroup.DatasetGroupDatasets.First().DatasetId}'*");
        }

        [Fact]
        public async Task SaveAsync_MustCallUpdateAndComplete()
        {
            // Arrange
            var datasetGroupRepositoryMock = new Mock<IDatasetGroupRepository>();
            var datasetGroupEntity = _fixture.Create<Data.Domain.DatasetGroup>();
            datasetGroupRepositoryMock.Setup(x => x.AddAsync(It.IsAny<DatasetGroup>()));
            _fixture.Inject(datasetGroupRepositoryMock.Object);
            _fixture.Freeze<IDatasetGroupRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDatasetGroupRepository>();
            var datasetGroupService = _fixture.Create<DatasetGroupService>();

            // Act
            await datasetGroupService.SaveAsync(datasetGroupEntity);

            // Assert
            datasetGroupRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<DatasetGroup>()), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

       [Fact]
        public void DeleteAsync_InvalidDatasetGroupId_MustNotCallDeleteOrComplete()
        {
            // Arrange
            var datasetGroupRepositoryMock = new Mock<IDatasetGroupRepository>();
            datasetGroupRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((DatasetGroup)null);
            _fixture.Inject(datasetGroupRepositoryMock.Object);
            _fixture.Freeze<IDatasetGroupRepository>();
            var datasetGroupService = _fixture.Create<DatasetGroupService>();
            var invalidId = Guid.NewGuid();

            // Act / Assert
            Func<Task> f = async () => await datasetGroupService.DeleteAsync(invalidId);
            f.Should().Throw<NotFoundException>();
        }

        [Fact]
        public async Task DeleteAsync_ValidDatasetGroupId_MustCallUpdateAndComplete()
        {
            // Arrange
            var datasetGroupRepositoryMock = new Mock<IDatasetGroupRepository>();
            var datasetGroupEntity = _fixture.Create<DatasetGroup>();
            var domainModelEntity = _fixture.Create<Data.Domain.DatasetGroup>();
            domainModelEntity.Id = datasetGroupEntity.Id;
            datasetGroupRepositoryMock.Setup(x => x.FindByIdAsync(datasetGroupEntity.Id)).ReturnsAsync(datasetGroupEntity);
            _fixture.Inject(datasetGroupRepositoryMock.Object);
            _fixture.Freeze<IDatasetGroupRepository>();
            var unitOfWorkMock = new Mock<IUnitIOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDatasetGroupRepository>();
            var datasetGroupService = _fixture.Create<DatasetGroupService>();

            // Act
            await datasetGroupService.DeleteAsync(domainModelEntity.Id.Value);

            // Assert
            datasetGroupRepositoryMock.Verify(mock => mock.Remove(datasetGroupEntity), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }
    }
}
