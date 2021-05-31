using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;

using DataCatalog.Common.Data;
using DataCatalog.Data.Model;
using DataCatalog.Common.Enums;
using DataCatalog.Api.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DataCatalog.Data;

namespace DataCatalog.Api.IntegrationTests.Repositories
{
    public class DurationRepositoryTests : BaseTest, IDisposable
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly DataCatalogContext _context;
        private readonly List<Duration> _durations;

        public DurationRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataCatalogContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new DataCatalogContext(options);

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _durations = _fixture.Create<IEnumerable<Duration>>().ToList();
            _durations.ForEach(c => _context.Durations.Add(c));
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
            var durationRepository = new DurationRepository(_context);

            // ACT
            var durations = await durationRepository.ListAsync();

            // ASSERT
            var durationArray = durations as Duration[] ?? durations.ToArray();
            durationArray.Should().NotBeNull();
            durationArray.Length.Should().Be(3);
            durationArray.Count(c => c.Id == _durations[0].Id).Should().Be(1);
            durationArray.Count(c => c.Id == _durations[1].Id).Should().Be(1);
            durationArray.Count(c => c.Id == _durations[2].Id).Should().Be(1);
        }

        [Fact]
        public async Task FindByIdAsync_InvalidId_ShouldReturnNull()
        {
            // ARRANGE
            var durationRepository = new DurationRepository(_context);
            var invalidId = Guid.NewGuid();

            // ACT
            var duration = await durationRepository.FindByIdAsync(invalidId);

            // ASSERT
            duration.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_ValidId_ShouldReturnDuration()
        {
            // ARRANGE
            var durationRepository = new DurationRepository(_context);

            // ACT
            var dataSource = await durationRepository.FindByIdAsync(_durations[1].Id);

            // ASSERT
            dataSource.Should().NotBeNull();
            dataSource.Id.Should().Be(_durations[1].Id);
        }

        [Fact]
        public async Task FindByCodeAsync_InvalidCode_ShouldReturnNull()
        {
            // ARRANGE
            var durationRepository = new DurationRepository(_context);
            var invalidCode = Guid.NewGuid().ToString();

            // ACT
            var duration = await durationRepository.FindByCodeAsync(invalidCode);

            // ASSERT
            duration.Should().BeNull();
        }

        [Fact]
        public async Task FindByCodeAsync_ValidCode_ShouldReturnDuration()
        {
            // ARRANGE
            var durationRepository = new DurationRepository(_context);

            // ACT
            var dataSource = await durationRepository.FindByCodeAsync(_durations[1].Code);

            // ASSERT
            dataSource.Should().NotBeNull();
            dataSource.Id.Should().Be(_durations[1].Id);
        }

        [Fact]
        public async Task AddAsync_ShouldAddDuration()
        {
            // ARRANGE
            var durationRepository = new DurationRepository(_context);
            var durationEntity = _fixture.Create<Duration>();

            // ACT
            await durationRepository.AddAsync(durationEntity);
            await _context.SaveChangesAsync();

            // ASSERT
            var durations = await durationRepository.ListAsync();
            var durationArray = durations as Duration[] ?? durations.ToArray();
            durationArray.Should().NotBeNull();
            durationArray.Length.Should().Be(4);
            durationArray.SingleOrDefault(c => c.Id == durationEntity.Id).Should().NotBeNull();
        }

        [Fact]
        public async Task Update_ChangeProperties_PropertiesShouldBeChanged()
        {
            // ARRANGE
            var durationRepository = new DurationRepository(_context);
            var durationToUpdate = _durations[2];
            var newDurationValues = _fixture.Create<Duration>();
            durationToUpdate.Code = newDurationValues.Code;
            durationToUpdate.Description = newDurationValues.Description;

            // ACT
            durationRepository.Update(durationToUpdate);
            await _context.SaveChangesAsync();

            // ASSERT
            var updatedDuration = await durationRepository.FindByIdAsync(durationToUpdate.Id);
            updatedDuration.Should().NotBeNull();
            updatedDuration.Code.Should().Be(newDurationValues.Code);
            updatedDuration.Description.Should().Be(newDurationValues.Description);
        }

        [Fact]
        public async Task Remove_ShouldBeRemoved()
        {
            // ARRANGE
            var durationRepository = new DurationRepository(_context);
            var theDurationToRemove = await durationRepository.FindByIdAsync(_durations[0].Id);

            // ACT
            durationRepository.Remove(theDurationToRemove);
            await _context.SaveChangesAsync();

            // ASSERT
            var nonExistingDuration = await durationRepository.FindByIdAsync(theDurationToRemove.Id);
            nonExistingDuration.Should().BeNull();
        }
    }
}
