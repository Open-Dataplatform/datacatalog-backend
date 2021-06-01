using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;

using DataCatalog.Data.Model;
using DataCatalog.Api.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DataCatalog.Data;

namespace DataCatalog.Api.IntegrationTests.Repositories
{
    public class DatasetGroupRepositoryTests : BaseTest, IDisposable
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly DataCatalogContext _context;
        private readonly List<DatasetGroup> _datasetGroups;

        public DatasetGroupRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataCatalogContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new DataCatalogContext(options);

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _datasetGroups = _fixture.Create<IEnumerable<DatasetGroup>>().ToList();
            _datasetGroups.ForEach(c => _context.DatasetGroups.Add(c));
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
            var datasetGroupRepository = new DatasetGroupRepository(_context);

            // ACT
            var datasetGroups = await datasetGroupRepository.ListAsync();

            // ASSERT
            var datasetGroupArray = datasetGroups as DatasetGroup[] ?? datasetGroups.ToArray();
            datasetGroupArray.Should().NotBeNull();
            datasetGroupArray.Length.Should().Be(3);
            datasetGroupArray.Count(c => c.Id == _datasetGroups[0].Id).Should().Be(1);
            datasetGroupArray.Count(c => c.Id == _datasetGroups[1].Id).Should().Be(1);
            datasetGroupArray.Count(c => c.Id == _datasetGroups[2].Id).Should().Be(1);
        }

        [Fact]
        public async Task FindByIdAsync_InvalidDatasetId_ShouldReturnNull()
        {
            // ARRANGE
            var datasetGroupRepository = new DatasetGroupRepository(_context);
            var invalidDatasetId = Guid.NewGuid();

            // ACT
            var datasetGroup = await datasetGroupRepository.FindByIdAsync(invalidDatasetId);

            // ASSERT
            datasetGroup.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_ValidId_ShouldReturnDatasetGroup()
        {
            // ARRANGE
            var datasetGroupRepository = new DatasetGroupRepository(_context);

            // ACT
            var datasetGroup = await datasetGroupRepository.FindByIdAsync(_datasetGroups[1].Id);

            // ASSERT
            datasetGroup.Should().NotBeNull();
            datasetGroup.Id.Should().Be(_datasetGroups[1].Id);
        }

        [Fact]
        public async Task AddAsync_ShouldAddDatasetGroup()
        {
            // ARRANGE
            var datasetGroupRepository = new DatasetGroupRepository(_context);
            var datasetGroupEntity = _fixture.Create<DatasetGroup>();

            // ACT
            await datasetGroupRepository.AddAsync(datasetGroupEntity);
            await _context.SaveChangesAsync();

            // ASSERT
            var datasetGroups = await datasetGroupRepository.ListAsync();
            var datasetGroupArray = datasetGroups as DatasetGroup[] ?? datasetGroups.ToArray();
            datasetGroupArray.Should().NotBeNull();
            datasetGroupArray.Length.Should().Be(4);
            datasetGroupArray.SingleOrDefault(c => c.Id == datasetGroupEntity.Id).Should().NotBeNull();
        }

       [Fact]
        public async Task Remove_ShouldBeRemoved()
        {
            // ARRANGE
            var datasetGroupRepository = new DatasetGroupRepository(_context);
            var theDatasetGroupToRemove = await datasetGroupRepository.FindByIdAsync(_datasetGroups[0].Id);

            // ACT
            datasetGroupRepository.Remove(theDatasetGroupToRemove);
            await _context.SaveChangesAsync();

            // ASSERT
            var nonExistingDatasetGroup = await datasetGroupRepository.FindByIdAsync(theDatasetGroupToRemove.Id);
            nonExistingDatasetGroup.Should().BeNull();
        }
    }
}
