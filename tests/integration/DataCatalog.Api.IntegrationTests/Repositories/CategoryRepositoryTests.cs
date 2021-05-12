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
    public class CategoryRepositoryTests : BaseTest, IDisposable
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly DataCatalogContext _context;
        private readonly List<Category> _categories;

        public CategoryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataCatalogContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new DataCatalogContext(options);

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _categories = _fixture.Create<IEnumerable<Category>>().ToList();
            _categories.First().OriginDeleted = true;
            _categories.Skip(1).ToList().ForEach(c => c.OriginDeleted = false);
            _categories.ForEach(c => _context.Categories.Add(c));
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
            var categoryRepository = new CategoryRepository(_context, current);

            // ACT
            var categories = await categoryRepository.ListAsync();

            // ASSERT
            var categoryArray = categories as Category[] ?? categories.ToArray();
            categoryArray.Should().NotBeNull("because ListAsync should return empty list if empty");
            categoryArray.Length.Should().Be(3, "because context contains at least two entries");
            categoryArray.Count(c => c.Id == _categories[0].Id).Should().Be(1, "because we have one entry with that id");
            categoryArray.Count(c => c.Id == _categories[1].Id).Should().Be(1, "because we have one entry with that id");
            categoryArray.Count(c => c.Id == _categories[2].Id).Should().Be(1, "because we have one entry with that id");
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
            var categoryRepository = new CategoryRepository(_context, current);

            // ACT
            var categories = await categoryRepository.ListAsync();

            // ASSERT
            var categoryArray = categories as Category[] ?? categories.ToArray();
            categoryArray.Should().NotBeNull("because ListAsync should return empty list if empty");
            categoryArray.Length.Should().Be(2, "because context contains two entries but only one not deleted");
            categoryArray.SingleOrDefault(c => c.Id == _categories[1].Id).Should().NotBeNull("because we have one entry with that id");
            categoryArray.SingleOrDefault(c => c.Id == _categories[2].Id).Should().NotBeNull("because we have one entry with that id");
        }

        [Fact]
        public async Task AddAsync_ShouldAddCategory()
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
            var categoryRepository = new CategoryRepository(_context, current);
            var categoryEntity = _fixture.Create<Category>();

            // ACT
            await categoryRepository.AddAsync(categoryEntity);
            await _context.SaveChangesAsync();

            // ASSERT
            var categories = await categoryRepository.ListAsync();
            var categoryArray = categories as Category[] ?? categories.ToArray();
            categoryArray.Should().NotBeNull("because ListAsync should return empty list if empty");
            categoryArray.Length.Should().BeGreaterOrEqualTo(2, "because context contains at least two entries not deleted");
            categoryArray.SingleOrDefault(c => c.Id == categoryEntity.Id).Should().NotBeNull("because we have one entry with that id");
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
            var categoryRepository = new CategoryRepository(_context, current);
            var invalidId = Guid.NewGuid();

            // ACT
            var category = await categoryRepository.FindByIdAsync(invalidId);

            // ASSERT
            category.Should().BeNull("because there is not category with this invalid id");
        }

        [Fact]
        public async Task FindByIdAsync_FindDeleted_ForAdmin_ShouldReturnCategory()
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
            var categoryRepository = new CategoryRepository(_context, current);

            // ACT
            var category = await categoryRepository.FindByIdAsync(_categories[0].Id);

            // ASSERT
            category.Should().NotBeNull();
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
            var categoryRepository = new CategoryRepository(_context, current);

            // ACT
            var category = await categoryRepository.FindByIdAsync(_categories[0].Id);

            // ASSERT
            category.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_FindExisting_ForAdmin_ShouldReturnCategory()
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
            var categoryRepository = new CategoryRepository(_context, current);

            // ACT
            var category = await categoryRepository.FindByIdAsync(_categories[1].Id);

            // ASSERT
            category.Should().NotBeNull();
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
            var categoryRepository = new CategoryRepository(_context, current);

            // ACT
            var category = await categoryRepository.FindByIdAsync(_categories[0].Id);

            // ASSERT
            category.Should().BeNull();
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
            var categoryRepository = new CategoryRepository(_context, current);
            var categoryToUpdate = _categories[2];
            var newCategoryValues = _fixture.Create<Category>();
            categoryToUpdate.Name = newCategoryValues.Name;
            categoryToUpdate.Colour = newCategoryValues.Colour;
            categoryToUpdate.ImageUri = newCategoryValues.ImageUri;

            // ACT
            categoryRepository.Update(categoryToUpdate);
            await _context.SaveChangesAsync();

            // ASSERT
            var updatedCategory = await categoryRepository.FindByIdAsync(_categories[2].Id);
            updatedCategory.Should().NotBeNull();
            updatedCategory.Name.Should().Be(newCategoryValues.Name, "because we changed the name");
            updatedCategory.Colour.Should().Be(newCategoryValues.Colour, "because we changed the color");
            updatedCategory.ImageUri.Should().Be(newCategoryValues.ImageUri, "because we changed the image uri");
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
            var categoryRepository = new CategoryRepository(_context, current);
            var theCategoryToRemove = await categoryRepository.FindByIdAsync(_categories[0].Id);

            // ACT
            categoryRepository.Remove(theCategoryToRemove);
            await _context.SaveChangesAsync();

            // ASSERT
            var nonExistingCategory = await categoryRepository.FindByIdAsync(theCategoryToRemove.Id);
            nonExistingCategory.Should().BeNull("because it was deleted");
        }
    }
}
