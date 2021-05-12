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
    public class DataContractRepositoryTests : BaseTest, IDisposable
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly DataCatalogContext _context;
        private readonly List<DataContract> _dataContracts;

        public DataContractRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataCatalogContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new DataCatalogContext(options);

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _dataContracts = _fixture.Create<IEnumerable<DataContract>>().ToList();
            _dataContracts.First().OriginDeleted = true;
            _dataContracts.Skip(1).ToList().ForEach(c => c.OriginDeleted = false);
            _dataContracts.ForEach(c => _context.DataContracts.Add(c));
            
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
            var dataContractRepository = new DataContractRepository(_context, current);

            // ACT
            var dataContracts = await dataContractRepository.ListAsync();

            // ASSERT
            var dataContractArray = dataContracts as DataContract[] ?? dataContracts.ToArray();
            dataContractArray.Should().NotBeNull("because ListAsync should return empty list if empty");
            dataContractArray.Length.Should().Be(3, "because context contains at least two entries");
            dataContractArray.Count(c => c.Id == _dataContracts[0].Id).Should().Be(1, "because we have one entry with that id");
            dataContractArray.Count(c => c.Id == _dataContracts[1].Id).Should().Be(1, "because we have one entry with that id");
            dataContractArray.Count(c => c.Id == _dataContracts[2].Id).Should().Be(1, "because we have one entry with that id");
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
            var dataContractRepository = new DataContractRepository(_context, current);

            // ACT
            var dataContracts = await dataContractRepository.ListAsync();

            // ASSERT
            var dataContractArray = dataContracts as DataContract[] ?? dataContracts.ToArray();
            dataContractArray.Should().NotBeNull("because ListAsync should return empty list if empty");
            dataContractArray.Length.Should().Be(2, "because context contains two entries but only one not deleted");
            dataContractArray.SingleOrDefault(c => c.Id == _dataContracts[1].Id).Should().NotBeNull("because we have one entry with that id");
            dataContractArray.SingleOrDefault(c => c.Id == _dataContracts[2].Id).Should().NotBeNull("because we have one entry with that id");
        }

        [Fact]
        public async Task AddAsync_ShouldAddDataContract()
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
            var dataContractRepository = new DataContractRepository(_context, current);
            var dataContractEntity = _fixture.Create<DataContract>();
            
            // ACT
            await dataContractRepository.AddAsync(dataContractEntity);
            await _context.SaveChangesAsync();

            // ASSERT
            var dataContracts = await dataContractRepository.ListAsync();
            var dataContractArray = dataContracts as DataContract[] ?? dataContracts.ToArray();
            dataContractArray.Should().NotBeNull("because ListAsync should return empty list if empty");
            dataContractArray.Length.Should().BeGreaterOrEqualTo(2, "because context contains at least two entries not deleted");
            dataContractArray.SingleOrDefault(c => c.Id == dataContractEntity.Id).Should().NotBeNull("because we have one entry with that id");
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
            var dataContractRepository = new DataContractRepository(_context, current);
            var invalidId = Guid.NewGuid();

            // ACT
            var dataContract = await dataContractRepository.FindByIdAsync(invalidId);

            // ASSERT
            dataContract.Should().BeNull("because there is not data contract with this invalid id");
        }

        [Fact]
        public async Task FindByIdAsync_FindDeleted_ForAdmin_ShouldReturnDataContract()
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
            var dataContractRepository = new DataContractRepository(_context, current);

            // ACT
            var dataContract = await dataContractRepository.FindByIdAsync(_dataContracts[0].Id);

            // ASSERT
            dataContract.Should().NotBeNull();
        }

        [Fact]
        public async Task FindByIdAsync_FindDeleted_ForNonAdmin_ShouldReturnNull()
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
            var dataContractRepository = new DataContractRepository(_context, current);

            // ACT
            var dataContract = await dataContractRepository.FindByIdAsync(_dataContracts[0].Id);

            // ASSERT
            dataContract.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_FindExisting_ForAdmin_ShouldReturnDataContract()
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
            var dataContractRepository = new DataContractRepository(_context, current);

            // ACT
            var dataContract = await dataContractRepository.FindByIdAsync(_dataContracts[1].Id);

            // ASSERT
            dataContract.Should().NotBeNull();
        }

        [Fact]
        public async Task FindByIdAsync_FindExisting_ForNonAdmin_ShouldReturnDataContract()
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
            var dataContractRepository = new DataContractRepository(_context, current);

            // ACT
            var dataContract = await dataContractRepository.FindByIdAsync(_dataContracts[1].Id);

            // ASSERT
            dataContract.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByDatasetIdAsync_InvalidId_ShouldReturnNull()
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
            var dataContractRepository = new DataContractRepository(_context, current);
            var invalidId = Guid.NewGuid();

            // ACT
            var dataContracts = await dataContractRepository.GetByDatasetIdAsync(invalidId);

            // ASSERT
            var dataContractsArray = dataContracts as DataContract[] ?? dataContracts.ToArray();
            dataContractsArray.Should().NotBeNull();
            dataContractsArray.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByDatasetIdAsync_FindDeleted_ForAdmin_ShouldReturnDataContract()
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
            var dataContractRepository = new DataContractRepository(_context, current);

            // ACT
            var dataContracts = await dataContractRepository.GetByDatasetIdAsync(_dataContracts[0].DatasetId);

            // ASSERT
            var dataContractsArray = dataContracts as DataContract[] ?? dataContracts.ToArray();
            dataContractsArray.Should().NotBeNull();
            dataContractsArray.Length.Should().Be(1);
            dataContractsArray.First().DatasetId.Should().Be(_dataContracts[0].DatasetId);
        }

        [Fact]
        public async Task GetByDatasetIdAsync_FindDeleted_ForNonAdmin_ShouldReturnNull()
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
            var dataContractRepository = new DataContractRepository(_context, current);

            // ACT
            var dataContracts = await dataContractRepository.GetByDatasetIdAsync(_dataContracts[0].DatasetId);

            // ASSERT
            var dataContractsArray = dataContracts as DataContract[] ?? dataContracts.ToArray();
            dataContractsArray.Should().NotBeNull();
            dataContractsArray.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByDatasetIdAsync_FindExisting_ForAdmin_ShouldReturnDataContract()
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
            var dataContractRepository = new DataContractRepository(_context, current);

            // ACT
            var dataContracts = await dataContractRepository.GetByDatasetIdAsync(_dataContracts[1].DatasetId);

            // ASSERT
            var dataContractsArray = dataContracts as DataContract[] ?? dataContracts.ToArray();
            dataContractsArray.Should().NotBeNull();
            dataContractsArray.Length.Should().Be(1);
            dataContractsArray.First().DatasetId.Should().Be(_dataContracts[1].DatasetId);
        }

        [Fact]
        public async Task GetByDatasetIdAsync_FindExisting_ForNonAdmin_ShouldReturnDataContract()
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
            var dataContractRepository = new DataContractRepository(_context, current);

            // ACT
            var dataContracts = await dataContractRepository.GetByDatasetIdAsync(_dataContracts[1].DatasetId);

            // ASSERT
            var dataContractsArray = dataContracts as DataContract[] ?? dataContracts.ToArray();
            dataContractsArray.Should().NotBeNull();
            dataContractsArray.Length.Should().Be(1);
            dataContractsArray.First().DatasetId.Should().Be(_dataContracts[1].DatasetId);
        }

        [Fact]
        public async Task GetByDataSourceIdAsync_InvalidId_ShouldReturnNull()
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
            var dataContractRepository = new DataContractRepository(_context, current);
            var invalidId = Guid.NewGuid();

            // ACT
            var dataContracts = await dataContractRepository.GetByDataSourceIdAsync(invalidId);

            // ASSERT
            var dataContractsArray = dataContracts as DataContract[] ?? dataContracts.ToArray();
            dataContractsArray.Should().NotBeNull();
            dataContractsArray.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByDataSourceIdAsync_FindDeleted_ForAdmin_ShouldReturnDataContract()
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
            var dataContractRepository = new DataContractRepository(_context, current);

            // ACT
            var dataContracts = await dataContractRepository.GetByDataSourceIdAsync(_dataContracts[0].DataSourceId);

            // ASSERT
            var dataContractsArray = dataContracts as DataContract[] ?? dataContracts.ToArray();
            dataContractsArray.Should().NotBeNull();
            dataContractsArray.Length.Should().Be(1);
            dataContractsArray.First().DataSourceId.Should().Be(_dataContracts[0].DataSourceId);
        }

        [Fact]
        public async Task GetByDataSourceIdAsync_FindDeleted_ForNonAdmin_ShouldReturnNull()
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
            var dataContractRepository = new DataContractRepository(_context, current);

            // ACT
            var dataContracts = await dataContractRepository.GetByDatasetIdAsync(_dataContracts[0].DataSourceId);

            // ASSERT
            var dataContractsArray = dataContracts as DataContract[] ?? dataContracts.ToArray();
            dataContractsArray.Should().NotBeNull();
            dataContractsArray.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByDataSourceIdAsync_FindExisting_ForAdmin_ShouldReturnDataContract()
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
            var dataContractRepository = new DataContractRepository(_context, current);

            // ACT
            var dataContracts = await dataContractRepository.GetByDataSourceIdAsync(_dataContracts[1].DataSourceId);

            // ASSERT
            var dataContractsArray = dataContracts as DataContract[] ?? dataContracts.ToArray();
            dataContractsArray.Should().NotBeNull();
            dataContractsArray.Length.Should().Be(1);
            dataContractsArray.First().DataSourceId.Should().Be(_dataContracts[1].DataSourceId);
        }

        [Fact]
        public async Task GetByDataSourceIdAsync_FindExisting_ForNonAdmin_ShouldReturnDataContract()
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
            var dataContractRepository = new DataContractRepository(_context, current);

            // ACT
            var dataContracts = await dataContractRepository.GetByDataSourceIdAsync(_dataContracts[1].DataSourceId);

            // ASSERT
            var dataContractsArray = dataContracts as DataContract[] ?? dataContracts.ToArray();
            dataContractsArray.Should().NotBeNull();
            dataContractsArray.Length.Should().Be(1);
            dataContractsArray.First().DataSourceId.Should().Be(_dataContracts[1].DataSourceId);
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
            var dataContractRepository = new DataContractRepository(_context, current);
            var dataContractToUpdate = _dataContracts[2];
            dataContractToUpdate.DatasetId = _dataContracts[0].DatasetId;
            dataContractToUpdate.DataSourceId = _dataContracts[1].DataSourceId;

            // ACT
            dataContractRepository.Update(dataContractToUpdate);
            await _context.SaveChangesAsync();

            // ASSERT
            var updatedDataContract = await dataContractRepository.FindByIdAsync(dataContractToUpdate.Id);
            updatedDataContract.Should().NotBeNull();
            updatedDataContract.DatasetId.Should().Be(dataContractToUpdate.DatasetId, "because we changed the dataset id");
            updatedDataContract.DataSourceId.Should().Be(dataContractToUpdate.DataSourceId, "because we changed the data source id");
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
            var dataContractRepository = new DataContractRepository(_context, current);
            var theDataContractToRemove = await dataContractRepository.FindByIdAsync(_dataContracts[0].Id);

            // ACT
            dataContractRepository.Remove(theDataContractToRemove);
            await _context.SaveChangesAsync();

            // ASSERT
            var nonExistingDataContract = await dataContractRepository.FindByIdAsync(theDataContractToRemove.Id);
            nonExistingDataContract.Should().BeNull("because it was deleted");
        }
    }
}
