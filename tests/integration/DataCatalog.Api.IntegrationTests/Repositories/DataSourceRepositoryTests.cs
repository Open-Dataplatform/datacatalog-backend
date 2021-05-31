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
    public class DataSourceRepositoryTests : BaseTest, IDisposable
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly DataCatalogContext _context;
        private readonly List<DataSource> _dataSources;

        public DataSourceRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataCatalogContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new DataCatalogContext(options);

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _dataSources = _fixture.Create<IEnumerable<DataSource>>().ToList();
            _dataSources.First().OriginDeleted = true;
            _dataSources.Skip(1).ToList().ForEach(c => c.OriginDeleted = false);
            _dataSources.ForEach(c => _context.DataSources.Add(c));
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task ListAsync_ForAdmin_ShouldReturnAll()
        {
            // ARRANGE
            var current = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role>
                {
                    Role.Admin
                }
            };
            var dataSourceRepository = new DataSourceRepository(_context, current);

            // ACT
            var dataSources = await dataSourceRepository.ListAsync();

            // ASSERT
            var dataSourceArray = dataSources as DataSource[] ?? dataSources.ToArray();
            dataSourceArray.Should().NotBeNull("because ListAsync should return empty list if empty");
            dataSourceArray.Length.Should().Be(3, "because context contains at least two entries");
            dataSourceArray.Count(c => c.Id == _dataSources[0].Id).Should().Be(1, "because we have one entry with that id");
            dataSourceArray.Count(c => c.Id == _dataSources[1].Id).Should().Be(1, "because we have one entry with that id");
            dataSourceArray.Count(c => c.Id == _dataSources[2].Id).Should().Be(1, "because we have one entry with that id");
        }

        [Fact]
        public async Task ListAsync_ForNonAdmin_ShouldNotReturnAll()
        {
            // ARRANGE
            var current = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role>
                {
                    Role.User
                }
            };
            var dataSourceRepository = new DataSourceRepository(_context, current);

            // ACT
            var dataSources = await dataSourceRepository.ListAsync();

            // ASSERT
            var dataSourceArray = dataSources as DataSource[] ?? dataSources.ToArray();
            dataSourceArray.Should().NotBeNull("because ListAsync should return empty list if empty");
            dataSourceArray.Length.Should().Be(2, "because context contains two entries but only one not deleted");
            dataSourceArray.SingleOrDefault(c => c.Id == _dataSources[1].Id).Should().NotBeNull("because we have one entry with that id");
            dataSourceArray.SingleOrDefault(c => c.Id == _dataSources[2].Id).Should().NotBeNull("because we have one entry with that id");
        }

        [Fact]
        public async Task AddAsync_ShouldAddDataSource()
        {
            // ARRANGE
            var current = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role>
                {
                    Role.Admin
                }
            };
            var dataSourceRepository = new DataSourceRepository(_context, current);
            var dataSourceEntity = _fixture.Create<DataSource>();
            
            // ACT
            await dataSourceRepository.AddAsync(dataSourceEntity);
            await _context.SaveChangesAsync();

            // ASSERT
            var dataSources = await dataSourceRepository.ListAsync();
            var dataSourceArray = dataSources as DataSource[] ?? dataSources.ToArray();
            dataSourceArray.Should().NotBeNull("because ListAsync should return empty list if empty");
            dataSourceArray.Length.Should().BeGreaterOrEqualTo(2, "because context contains at least two entries not deleted");
            dataSourceArray.SingleOrDefault(c => c.Id == dataSourceEntity.Id).Should().NotBeNull("because we have one entry with that id");
        }

        [Fact]
        public async Task FindByIdAsync_InvalidId_ShouldReturnNull()
        {
            // ARRANGE
            var current = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role>
                {
                    Role.User
                }
            };
            var dataSourceRepository = new DataSourceRepository(_context, current);
            var invalidId = Guid.NewGuid();

            // ACT
            var dataSource = await dataSourceRepository.FindByIdAsync(invalidId);

            // ASSERT
            dataSource.Should().BeNull("because there is not data source with this invalid id");
        }

        [Fact]
        public async Task FindByIdAsync_FindDeleted_ForAdmin_ShouldReturnDataSource()
        {
            // ARRANGE
            var current = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role>
                {
                    Role.Admin
                }
            };
            var dataSourceRepository = new DataSourceRepository(_context, current);

            // ACT
            var dataSource = await dataSourceRepository.FindByIdAsync(_dataSources[0].Id);

            // ASSERT
            dataSource.Should().NotBeNull();
        }

        [Fact]
        public async Task FindByIdAsync_FindDeleted_ForNonAdmin_ShouldReturnNull()
        {
            // ARRANGE
            var current = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role> { Role.User }
            };
            var dataSourceRepository = new DataSourceRepository(_context, current);

            // ACT
            var dataSource = await dataSourceRepository.FindByIdAsync(_dataSources[0].Id);

            // ASSERT
            dataSource.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_FindExisting_ForAdmin_ShouldReturnDataSource()
        {
            // ARRANGE
            var current = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role>
                {
                    Role.Admin
                }
            };
            var dataSourceRepository = new DataSourceRepository(_context, current);

            // ACT
            var dataSource = await dataSourceRepository.FindByIdAsync(_dataSources[1].Id);

            // ASSERT
            dataSource.Should().NotBeNull();
        }

        [Fact]
        public async Task FindByIdAsync_FindExisting_ForNonAdmin_ShouldReturnNull()
        {
            // ARRANGE
            var current = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role>
                {
                    Role.User
                }
            };
            var dataSourceRepository = new DataSourceRepository(_context, current);

            // ACT
            var dataSource = await dataSourceRepository.FindByIdAsync(_dataSources[0].Id);

            // ASSERT
            dataSource.Should().BeNull();
        }

        [Fact]
        public async Task AnyAsync_NotFound_ShouldReturnFalse()
        {
            // ARRANGE
            var current = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role>
                {
                    Role.User
                }
            };
            var dataSourceRepository = new DataSourceRepository(_context, current);
            _dataSources[0].SourceType = SourceType.DataPlatform;

            // ACT
            var found = await dataSourceRepository.AnyAsync(new [] { _dataSources[0].Id }, new[] { SourceType.External });

            // ASSERT
            found.Should().BeFalse();
        }

        [Fact]
        public async Task AnyAsync_Found_ShouldReturnTrue()
        {
            // ARRANGE
            var current = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role>
                {
                    Role.User
                }
            };
            var dataSourceRepository = new DataSourceRepository(_context, current);

            // ACT
            var found = await dataSourceRepository.AnyAsync(new[] { _dataSources[0].Id }, new[] { _dataSources[0].SourceType });

            // ASSERT
            found.Should().BeTrue();
        }

        [Fact]
        public async Task Update_ChangeProperties_PropertiesShouldBeChanged()
        {
            // ARRANGE
            var current = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role>
                {
                    Role.Admin
                }
            };
            var dataSourceRepository = new DataSourceRepository(_context, current);
            var dataSourceToUpdate = _dataSources[2];
            var newDataSourceValues = _fixture.Create<DataSource>();
            dataSourceToUpdate.Name = newDataSourceValues.Name;
            dataSourceToUpdate.Description = newDataSourceValues.Description;
            dataSourceToUpdate.SourceType = (SourceType)(((int)dataSourceToUpdate.SourceType + 1) % Enum.GetNames(typeof(SourceType)).Length);
            dataSourceToUpdate.ContactInfo = newDataSourceValues.ContactInfo;
            
            // ACT
            dataSourceRepository.Update(dataSourceToUpdate);
            await _context.SaveChangesAsync();

            // ASSERT
            var updatedDataSource = await dataSourceRepository.FindByIdAsync(dataSourceToUpdate.Id);
            updatedDataSource.Should().NotBeNull();
            updatedDataSource.Name.Should().Be(newDataSourceValues.Name, "because we changed the name");
            updatedDataSource.Description.Should().Be(newDataSourceValues.Description, "because we changed the description");
            updatedDataSource.ContactInfo.Should().Be(newDataSourceValues.ContactInfo, "because we changed the contact info");
            updatedDataSource.SourceType.Should().Be(newDataSourceValues.SourceType, "because we changed the source type");
        }

        [Fact]
        public async Task Remove_ShouldBeRemoved()
        {
            // ARRANGE
            var current = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role>
                {
                    Role.Admin
                }
            };
            var dataSourceRepository = new DataSourceRepository(_context, current);
            var theDataSourceToRemove = await dataSourceRepository.FindByIdAsync(_dataSources[0].Id);

            // ACT
            dataSourceRepository.Remove(theDataSourceToRemove);
            await _context.SaveChangesAsync();

            // ASSERT
            var nonExistingDataSource = await dataSourceRepository.FindByIdAsync(theDataSourceToRemove.Id);
            nonExistingDataSource.Should().BeNull("because it was deleted");
        }
    }
}
