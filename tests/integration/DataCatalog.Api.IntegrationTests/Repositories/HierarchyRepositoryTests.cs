using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
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
    public class HierarchyRepositoryTests : BaseTest, IDisposable
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly DataCatalogContext _context;
        private readonly List<Hierarchy> _hierarchies;

        public HierarchyRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataCatalogContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new DataCatalogContext(options);

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _hierarchies = _fixture.CreateMany<Hierarchy>(6).ToList();
            _hierarchies[0].ParentHierarchyId = null;
            _hierarchies[1].ParentHierarchyId = _hierarchies[0].Id;
            _hierarchies[2].ParentHierarchyId = _hierarchies[1].Id;
            _hierarchies[3].ParentHierarchyId = null;
            _hierarchies[4].ParentHierarchyId = null;
            _hierarchies[5].ParentHierarchyId = _hierarchies[4].Id;
            _hierarchies.ForEach(c => _context.Hierarchies.Add(c));
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task ListAsync_ShouldReturnAll()
        {
            // ARRANGE
            var hierarchyRepository = new HierarchyRepository(_context);

            // ACT
            var hierarchies = await hierarchyRepository.ListAsync();

            // ASSERT
            var hierarchyArray = hierarchies as Hierarchy[] ?? hierarchies.ToArray();
            hierarchyArray.Should().NotBeNull();
            hierarchyArray.Length.Should().Be(3);
            hierarchyArray.Count(c => c.Id == _hierarchies[0].Id).Should().Be(1);
            hierarchyArray.Count(c => c.Id == _hierarchies[3].Id).Should().Be(1);
            hierarchyArray.Count(c => c.Id == _hierarchies[4].Id).Should().Be(1);
        }

        [Fact]
        public async Task FindByIdAsync_InvalidId_ShouldReturnNull()
        {
            // ARRANGE
            var hierarchyRepository = new HierarchyRepository(_context);
            var invalidId = Guid.NewGuid();

            // ACT
            var hierarchy = await hierarchyRepository.FindByIdAsync(invalidId);

            // ASSERT
            hierarchy.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_ValidId_ShouldReturnHierarchy()
        {
            // ARRANGE
            var hierarchyRepository = new HierarchyRepository(_context);

            // ACT
            var hierarchy = await hierarchyRepository.FindByIdAsync(_hierarchies[1].Id);

            // ASSERT
            hierarchy.Should().NotBeNull();
            hierarchy.Id.Should().Be(_hierarchies[1].Id);
        }

        [Fact]
        public async Task AddAsync_ShouldAddHierarchy()
        {
            // ARRANGE
            var hierarchyRepository = new HierarchyRepository(_context);
            var hierarchyEntity = _fixture.Create<Hierarchy>();

            // ACT
            await hierarchyRepository.AddAsync(hierarchyEntity);
            await _context.SaveChangesAsync();

            // ASSERT
            var hierarchy = await hierarchyRepository.FindByIdAsync(hierarchyEntity.Id);
            hierarchy.Should().NotBeNull();
            hierarchy.Id.Should().Be(hierarchyEntity.Id);
        }

        [Fact]
        public async Task Update_ChangeProperties_PropertiesShouldBeChanged()
        {
            // ARRANGE
            var hierarchyRepository = new HierarchyRepository(_context);
            var hierarchyToUpdate = _hierarchies[2];
            var newHierarchyValues = _fixture.Create<Hierarchy>();
            hierarchyToUpdate.Name = newHierarchyValues.Name;
            hierarchyToUpdate.Description = newHierarchyValues.Description;
            hierarchyToUpdate.ParentHierarchyId = newHierarchyValues.ParentHierarchyId;

            // ACT
            hierarchyRepository.Update(hierarchyToUpdate);
            await _context.SaveChangesAsync();

            // ASSERT
            var updatedHierarchy = await hierarchyRepository.FindByIdAsync(hierarchyToUpdate.Id);
            updatedHierarchy.Should().NotBeNull();
            updatedHierarchy.Name.Should().Be(newHierarchyValues.Name);
            updatedHierarchy.Description.Should().Be(newHierarchyValues.Description);
            updatedHierarchy.ParentHierarchyId.Should().Be(newHierarchyValues.ParentHierarchyId);
        }

        [Fact]
        public async Task Remove_ShouldBeRemoved()
        {
            // ARRANGE
            var hierarchyRepository = new HierarchyRepository(_context);
            var theHierarchyToRemove = await hierarchyRepository.FindByIdAsync(_hierarchies[0].Id);

            // ACT
            hierarchyRepository.Remove(theHierarchyToRemove);
            await _context.SaveChangesAsync();

            // ASSERT
            var nonExistingHierarchy = await hierarchyRepository.FindByIdAsync(theHierarchyToRemove.Id);
            nonExistingHierarchy.Should().BeNull();
        }
    }
}
