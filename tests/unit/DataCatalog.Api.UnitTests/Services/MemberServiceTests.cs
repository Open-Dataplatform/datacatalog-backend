using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;

using DataCatalog.Data.Model;
using DataCatalog.Api.Repositories;
using DataCatalog.Api.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using DataCatalog.Api.Data;

namespace DataCatalog.Api.UnitTests.Services
{
    public class MemberServiceTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

        public MemberServiceTests()
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
        public async Task GetOrCreateAsync_ThrowException_WhenMemberCannotBeCreatedOrFoundLater()
        {
            // ARRANGE
            var externalId = Guid.NewGuid().ToString();
            var identityProviderId = Guid.NewGuid();
            var memberRepositoryMock = new Mock<IMemberRepository>();
            memberRepositoryMock
                .Setup(x => x.FindByExternalIdAsync(externalId, identityProviderId))
                .Returns(Task.FromResult((Member) null));
            var unitOfWorkServiceMock = new Mock<IUnitOfWork>();
            unitOfWorkServiceMock
                .Setup(x => x.CompleteAsync())
                .Throws(new DbUpdateException());
            _fixture.Inject(memberRepositoryMock.Object);
            _fixture.Freeze<IMemberRepository>();
            _fixture.Inject(unitOfWorkServiceMock.Object);
            _fixture.Freeze<IUnitOfWork>();
            var memberService = _fixture.Create<MemberService>();

            // ACT
            var result = await memberService.GetOrCreateAsync(externalId, identityProviderId);

            // ASSERT
            result.Should().BeNull("because mapping null should return null");
        }

        [Fact]
        public async Task GetOrCreateAsync_ReturnMember_WhenMemberCannotBeCreatedButFoundLater()
        {
            // ARRANGE
            var externalId = Guid.NewGuid().ToString();
            var identityProviderId = Guid.NewGuid();
            Member aMember = new Member { Id = Guid.NewGuid() };
            var memberRepositoryMock = new Mock<IMemberRepository>();
            memberRepositoryMock
                .SetupSequence(x => x.FindByExternalIdAsync(externalId, identityProviderId))
                .Returns(Task.FromResult((Member) null))
                .Returns(Task.FromResult(aMember));
            var unitOfWorkServiceMock = new Mock<IUnitOfWork>();
            unitOfWorkServiceMock
                .Setup(x => x.CompleteAsync())
                .Throws(new DbUpdateException());
            _fixture.Inject(memberRepositoryMock.Object);
            _fixture.Freeze<IMemberRepository>();
            _fixture.Inject(unitOfWorkServiceMock.Object);
            _fixture.Freeze<IUnitOfWork>();
            var memberService = _fixture.Create<MemberService>();

            // ACT
            var result = await memberService.GetOrCreateAsync(externalId, identityProviderId);

            // ASSERT
            result.Should().NotBeNull("because member must be found in second pass");
        }

        [Fact]
        public async Task GetOrCreateAsync_ReturnMember_WhenMemberAreFound_EnsureOnlyOneCallToFind()
        {
            // ARRANGE
            var externalId = Guid.NewGuid().ToString();
            var identityProviderId = Guid.NewGuid();
            var aMember = new Member { Id = Guid.NewGuid() };
            var memberRepositoryMock = new Mock<IMemberRepository>();
            memberRepositoryMock
                .Setup(x => x.FindByExternalIdAsync(externalId, identityProviderId))
                .Returns(Task.FromResult(aMember));
            var unitOfWorkServiceMock = new Mock<IUnitOfWork>();
            unitOfWorkServiceMock
                .Setup(x => x.CompleteAsync())
                .Throws(new DbUpdateException());
            _fixture.Inject(memberRepositoryMock.Object);
            _fixture.Freeze<IMemberRepository>();
            _fixture.Inject(unitOfWorkServiceMock.Object);
            _fixture.Freeze<IUnitOfWork>();
            var memberService = _fixture.Create<MemberService>();

            // ACT
            var result = await memberService.GetOrCreateAsync(externalId, identityProviderId);

            // ASSERT
            result.Should().NotBeNull("because member must be found");
            memberRepositoryMock.Verify(m => m.FindByExternalIdAsync(externalId, identityProviderId), Times.Exactly(1));
        }
    }
}
