using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Api.Data;
using DataCatalog.Api.Data.Common;
using DataCatalog.Api.Data.Model;
using DataCatalog.Api.Enums;
using DataCatalog.Api.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DataCatalog.Api.IntegrationTests.Repositories
{
    public class MemberRepositoryTests : BaseTest, IDisposable
    {
        // Repo data
        private readonly IdentityProvider _identityProvider;
        private readonly Member _member1;

        private readonly DataCatalogContext _context;

        public MemberRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataCatalogContext>().UseInMemoryDatabase(databaseName: "Member").Options;
            _context = new DataCatalogContext(options);

            _identityProvider = CreateIdentityProvider();
            _member1 = CreateMember(_identityProvider);
            var member2 = CreateMember(_identityProvider);

            _context.IdentityProvider.Add(_identityProvider);
            _context.Members.Add(_member1);
            _context.Members.Add(member2);

            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task FindByExternalIdAsync_InvalidId_ShouldReturnNone()
        {
            // ARRANGE
            var memberRepository = new MemberRepository(_context);
            var invalidId = Guid.NewGuid().ToString();

            // ACT
            var member = await memberRepository.FindByExternalIdAsync(invalidId, _identityProvider.Id);

            // ASSERT
            member.Should().BeNull();
        }


        [Fact]
        public async Task FindByExternalIdAsync_InvalidIdentityProviderId_ShouldReturnNull()
        {
            // ARRANGE
            var memberRepository = new MemberRepository(_context);
            var invalidIdentityProviderId = Guid.NewGuid();

            // ACT
            var member = await memberRepository.FindByExternalIdAsync(_member1.ExternalId, invalidIdentityProviderId);

            // ASSERT
            member.Should().BeNull();
        }

        [Fact]
        public async Task FindByExternalIdAsync_ValidId_ShouldReturnMember()
        {
            // ARRANGE
            var memberRepository = new MemberRepository(_context);

            // ACT
            var member = await memberRepository.FindByExternalIdAsync(_member1.ExternalId, _identityProvider.Id);

            // ASSERT
            member.Should().NotBeNull();
            member.Id.Should().Be(_member1.Id);
        }

        [Fact]
        public async Task FindByIdAsync_InvalidId_ShouldReturnNull()
        {
            // ARRANGE
            var memberRepository = new MemberRepository(_context);
            var invalidId = Guid.NewGuid();

            // ACT
            var member = await memberRepository.FindByIdAsync(invalidId);

            // ASSERT
            member.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_ValidId_ShouldReturnMember()
        {
            // ARRANGE
            var memberRepository = new MemberRepository(_context);

            // ACT
            var member = await memberRepository.FindByIdAsync(_member1.Id);

            // ASSERT
            member.Should().NotBeNull();
            member.Id.Should().Be(_member1.Id);
        }
    }
}
