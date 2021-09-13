using AutoFixture;
using AutoFixture.AutoMoq;
using DataCatalog.Data.Model;
using DataCatalog.Api.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using DataCatalog.Data;

namespace DataCatalog.Api.IntegrationTests.Repositories
{
    public class MemberGroupRepositoryTests : BaseTest, IDisposable
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly DataCatalogContext _context;
        private readonly List<Member> _members;
        private readonly List<MemberGroup> _memberGroups;
        private readonly List<MemberGroupMember> _memberGroupMembers;
        private readonly List<Dataset> _datasets;

        public MemberGroupRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataCatalogContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new DataCatalogContext(options);

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _members = _fixture.Create<IEnumerable<Member>>().ToList();
            _memberGroups = _fixture.Create<IEnumerable<MemberGroup>>().ToList();
            _memberGroupMembers = _fixture.Create<IEnumerable<MemberGroupMember>>().ToList();
            _datasets = _fixture.Create<IEnumerable<Dataset>>().ToList();

            _members.ForEach(m => m.MemberGroupMembers = null);
            _members.ForEach(m => m.DatasetChangeLogs = null);
            _members.ForEach(m => m.IdentityProvider = null);
            _memberGroups.ForEach(m => m.Datasets = null);
            _memberGroups.ForEach(m => m.MemberGroupMembers = null);
            _memberGroupMembers.ForEach(m => m.Member = null);
            _memberGroupMembers.ForEach(m => m.MemberGroup = null);
            _datasets.ForEach(d => d.Contact = null);
            _datasets.ForEach(d => d.DataFields = null);
            _datasets.ForEach(d => d.DatasetCategories = null);
            _datasets.ForEach(d => d.TransformationDatasets = null);
            _datasets.ForEach(d => d.DatasetChangeLogs = null);
            _datasets.ForEach(d => d.DatasetDurations = null);
            _datasets.ForEach(d => d.DataContracts = null);

            _memberGroupMembers[0].MemberId = _members[0].Id;
            _memberGroupMembers[0].MemberGroupId = _memberGroups[0].Id;
            _memberGroupMembers[1].MemberId = _members[1].Id;
            _memberGroupMembers[1].MemberGroupId = _memberGroups[0].Id;
            _memberGroupMembers[2].MemberId = _members[1].Id;
            _memberGroupMembers[2].MemberGroupId = _memberGroups[1].Id;
            _datasets[0].ContactId = _memberGroups[0].Id;
            _datasets[1].ContactId = _memberGroups[1].Id;
            _datasets[2].ContactId = _memberGroups[1].Id;

            _members.ForEach(c => _context.Members.Add(c));
            _memberGroups.ForEach(c => _context.MemberGroups.Add(c));
            _memberGroupMembers.ForEach(c => _context.MemberGroupMembers.Add(c));
            _datasets.ForEach(c => _context.Datasets.Add(c));
            
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task ListAsync_ShouldReturnGroupsIncludingDatasets()
        {
            // ARRANGE
            var memberGroupRepository = new MemberGroupRepository(_context);

            // ACT
            var memberGroups = await memberGroupRepository.ListAsync();

            // ASSERT
            var memberGroupArray = memberGroups as MemberGroup[] ?? memberGroups.ToArray();
            memberGroupArray.Should().NotBeNull();
            memberGroupArray.Length.Should().Be(3);
            memberGroupArray.Single(g => g.Id == _memberGroups[0].Id).Datasets.Count.Should().Be(1);
            memberGroupArray.Single(g => g.Id == _memberGroups[0].Id).Datasets.First().Id.Should().Be(_datasets[0].Id);
            memberGroupArray.Single(g => g.Id == _memberGroups[2].Id).Datasets.Count.Should().Be(0);
        }

        [Fact]
        public async Task AddAsync_ShouldAddMemberGroup()
        {
            // ARRANGE
            var memberGroupRepository = new MemberGroupRepository(_context);
            var memberGroupEntity = _fixture.Create<MemberGroup>();
            
            // ACT
            await memberGroupRepository.AddAsync(memberGroupEntity);
            await _context.SaveChangesAsync();

            // ASSERT
            var memberGroups = await memberGroupRepository.ListAsync();
            var memberGroupArray = memberGroups as MemberGroup[] ?? memberGroups.ToArray();
            memberGroupArray.Should().NotBeNull();
            memberGroupArray.Length.Should().Be(4);
            memberGroupArray.SingleOrDefault(c => c.Id == memberGroupEntity.Id).Should().NotBeNull();
        }

        [Fact]
        public async Task FindByIdAsync_InvalidId_ShouldReturnNull()
        {
            // ARRANGE
            var memberGroupRepository = new MemberGroupRepository(_context);
            var invalidId = Guid.NewGuid();

            // ACT
            var memberGroup = await memberGroupRepository.FindByIdAsync(invalidId);

            // ASSERT
            memberGroup.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_FindExisting_ShouldReturnMemberGroup()
        {
            // ARRANGE
            var memberGroupRepository = new MemberGroupRepository(_context);

            // ACT
            var memberGroup = await memberGroupRepository.FindByIdAsync(_memberGroups[1].Id);

            // ASSERT
            memberGroup.Should().NotBeNull();
            memberGroup.Id.Should().Be(_memberGroups[1].Id);
            memberGroup.Datasets.Should().NotBeNull();
            memberGroup.Datasets.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetMemberGroups_InvalidMemberId_ShouldReturnEmptyList()
        {
            // ARRANGE
            var invalidId = Guid.NewGuid();
            var memberGroupRepository = new MemberGroupRepository(_context);

            // ACT
            var memberGroup = await memberGroupRepository.GetMemberGroups(invalidId);

            // ASSERT
            var memberGroupArray = memberGroup as MemberGroup[] ?? memberGroup.ToArray();
            memberGroupArray.Should().NotBeNull();
            memberGroupArray.Should().BeEmpty();
        }

        [Fact]
        public async Task GetMemberGroups_ValidMemberId_ShouldReturnMemberGroups()
        {
            // ARRANGE

            var memberGroupRepository = new MemberGroupRepository(_context);

            // ACT
            var memberGroup = await memberGroupRepository.GetMemberGroups(_members[1].Id);

            // ASSERT
            var memberGroupArray = memberGroup as MemberGroup[] ?? memberGroup.ToArray();
            memberGroupArray.Should().NotBeNull();
            memberGroupArray.Length.Should().Be(2);
            memberGroupArray.Any(m => m.Id == _memberGroups[0].Id).Should().BeTrue();
            memberGroupArray.Single(m => m.Id == _memberGroups[0].Id).Datasets.Should().NotBeNull();
            memberGroupArray.Single(m => m.Id == _memberGroups[0].Id).Datasets.Count.Should().Be(1);
            memberGroupArray.Single(m => m.Id == _memberGroups[0].Id).Datasets.Any(d => d.Id == _datasets[0].Id).Should().BeTrue();
        }

        [Fact]
        public async Task AddMemberAsync_ShouldAddMemberToMemberGroup()
        {
            // ARRANGE
            var memberGroupRepository = new MemberGroupRepository(_context);

            // ACT
            await memberGroupRepository.AddMemberAsync(_memberGroups[2].Id, _members[2].Id);
            await _context.SaveChangesAsync();

            // ASSERT
            var memberGroups = await memberGroupRepository.GetMemberGroups(_members[2].Id);
            var memberGroupArray = memberGroups as MemberGroup[] ?? memberGroups.ToArray();
            memberGroupArray.Should().NotBeNull();
            memberGroupArray.Length.Should().Be(1);
            memberGroupArray.First().Id.Should().Be(_memberGroups[2].Id);
        }

        [Fact]
        public async Task Update_ChangeProperties_PropertiesShouldBeChanged()
        {
            // ARRANGE
            var memberGroupRepository = new MemberGroupRepository(_context);
            var memberGroupToUpdate = _memberGroups[2];
            var newMemberGroupValues = _fixture.Create<MemberGroup>();
            memberGroupToUpdate.Name = newMemberGroupValues.Name;
            memberGroupToUpdate.Description = newMemberGroupValues.Description;
            memberGroupToUpdate.Email = newMemberGroupValues.Email;

            // ACT
            memberGroupRepository.Update(memberGroupToUpdate);
            await _context.SaveChangesAsync();

            // ASSERT
            var updatedMemberGroup = await memberGroupRepository.FindByIdAsync(memberGroupToUpdate.Id);
            updatedMemberGroup.Should().NotBeNull();
            updatedMemberGroup.Name.Should().Be(newMemberGroupValues.Name);
            updatedMemberGroup.Description.Should().Be(newMemberGroupValues.Description);
            updatedMemberGroup.Email.Should().Be(newMemberGroupValues.Email);
        }

        [Fact]
        public async Task Remove_ShouldBeRemoved()
        {
            // ARRANGE
            var memberGroupRepository = new MemberGroupRepository(_context);
            var theMemberGroupToRemove = await memberGroupRepository.FindByIdAsync(_memberGroups[0].Id);

            // ACT
            memberGroupRepository.Remove(theMemberGroupToRemove);
            await _context.SaveChangesAsync();

            // ASSERT
            var nonExistingMemberGroup = await memberGroupRepository.FindByIdAsync(theMemberGroupToRemove.Id);
            nonExistingMemberGroup.Should().BeNull("because it was deleted");
        }
    }
}
