using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;

using DataCatalog.Data.Model;
using DataCatalog.Api.Repositories;
using DataCatalog.Api.Services;
using FluentAssertions;
using Moq;
using Xunit;
using DataCatalog.Api.Data;

namespace DataCatalog.Api.UnitTests.Services
{
    public class CategoryServiceShould
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

        public CategoryServiceShould()
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
            
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "test");
        }

        [Fact]
        public async Task Return_Null_When_FindById_Does_Not_Have_A_Category()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var categoryId = Guid.NewGuid();
            categoryRepositoryMock.Setup(x => x.FindByIdAsync(categoryId)).ReturnsAsync((Category) null);
            _fixture.Inject(categoryRepositoryMock.Object);
            _fixture.Freeze<ICategoryRepository>();
            var sut = _fixture.Create<CategoryService>();

            // Act
            var category = await sut.FindByIdAsync(categoryId);

            // Assert
            category.Should().BeNull();
        }

        [Fact]
        public async Task Return_Category_When_Repository_Has_A_Category()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var categoryEntity = _fixture.Create<Category>();
            categoryRepositoryMock.Setup(x => x.FindByIdAsync(categoryEntity.Id)).ReturnsAsync(categoryEntity);
            _fixture.Inject(categoryRepositoryMock.Object);
            _fixture.Freeze<ICategoryRepository>();
            var sut = _fixture.Create<CategoryService>();

            // Act
            var category = await sut.FindByIdAsync(categoryEntity.Id);

            // Assert
            category.Should().NotBeNull();
            category.Colour.Should().Be(categoryEntity.Colour);
            category.ImageUri.Should().Be(categoryEntity.ImageUri);
            category.Name.Should().Be(categoryEntity.Name);
            category.Id.Should().Be(categoryEntity.Id);
            category.CreatedDate.Should().Be(categoryEntity.CreatedDate);
            category.ModifiedDate.Should().Be(categoryEntity.ModifiedDate);
        }

        [Fact]
        public async Task Return_No_Categories_Without_DataSets_When_IncludeEmpty_Is_False_On_ListAsync()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var categoryEntities = _fixture.Create<IEnumerable<Category>>();
            categoryRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(categoryEntities);
            _fixture.Inject(categoryRepositoryMock.Object);
            _fixture.Freeze<ICategoryRepository>();
            var sut = _fixture.Create<CategoryService>();

            // Act
            var categories = await sut.ListAsync(false);

            // Assert
            categories.Should().BeEmpty();
        }

        [Fact]
        public async Task Return_Only_Relevant_Categories_When_IncludeEmpty_Is_False_On_ListAsync()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var datasetCategoryRepositoryMock = new Mock<IDatasetCategoryRepository>();
            var categoryEntities = _fixture.Create<IEnumerable<Category>>();
            var enumerable = categoryEntities as Category[] ?? categoryEntities.ToArray();
            var datasetCategoryEntities = enumerable.Select(x => new DatasetCategory { CategoryId = x.Id, DatasetId = Guid.NewGuid() });
            datasetCategoryEntities = datasetCategoryEntities.SkipLast(1);
            categoryRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(enumerable);
            datasetCategoryRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(datasetCategoryEntities);
            _fixture.Inject(categoryRepositoryMock.Object);
            _fixture.Inject(datasetCategoryRepositoryMock.Object);
            _fixture.Freeze<ICategoryRepository>();
            _fixture.Freeze<IDatasetCategoryRepository>();
            var sut = _fixture.Create<CategoryService>();

            // Act
            var categories = await sut.ListAsync(false);

            // Assert
            var categoryList = categories as Data.Domain.Category[] ?? categories.ToArray();
            categoryList.Length.Should().Be(enumerable.Length-1);
            foreach (var category in categoryList)
            {
                var expected = enumerable.FirstOrDefault(x => Equals(x.Id, category.Id));

                expected.Should().NotBeNull();
                expected.Colour.Should().Be(category.Colour);
                expected.ImageUri.Should().Be(category.ImageUri);
                expected.Name.Should().Be(category.Name);
                expected.Id.Should().Be(category.Id);
                expected.CreatedDate.Should().Be(category.CreatedDate);
                expected.ModifiedDate.Should().Be(category.ModifiedDate);
            }
        }

        [Fact]
        public async Task Return_All_Categories_When_IncludeEmpty_Is_False_And_All_Categories_Contains_Datasets_On_ListAsync()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var datasetCategoryRepositoryMock = new Mock<IDatasetCategoryRepository>();
            var categoryEntities = _fixture.Create<IEnumerable<Category>>();
            var enumerable = categoryEntities as Category[] ?? categoryEntities.ToArray();
            var datasetCategoryEntities = enumerable.Select(x => new DatasetCategory { CategoryId = x.Id, DatasetId = Guid.NewGuid() });
            categoryRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(enumerable);
            datasetCategoryRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(datasetCategoryEntities);
            _fixture.Inject(categoryRepositoryMock.Object);
            _fixture.Inject(datasetCategoryRepositoryMock.Object);
            _fixture.Freeze<ICategoryRepository>();
            _fixture.Freeze<IDatasetCategoryRepository>();
            var sut = _fixture.Create<CategoryService>();

            // Act
            var categories = await sut.ListAsync(false);

            // Assert
            var categoryList = categories as Data.Domain.Category[] ?? categories.ToArray();
            categoryList.Length.Should().Be(enumerable.Length);
            foreach(var category in categoryList)
            {
                var expected = enumerable.FirstOrDefault(x => Equals(x.Id, category.Id));

                expected.Should().NotBeNull();
                expected.Colour.Should().Be(category.Colour);
                expected.ImageUri.Should().Be(category.ImageUri);
                expected.Name.Should().Be(category.Name);
                expected.Id.Should().Be(category.Id);
                expected.CreatedDate.Should().Be(category.CreatedDate);
                expected.ModifiedDate.Should().Be(category.ModifiedDate);
            }
        }

        [Fact]
        public async Task Return_All_Categories_When_IncludeEmpty_Is_True_On_ListAsync()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var categoryEntities = _fixture.Create<IEnumerable<Category>>();
            var enumerable = categoryEntities as Category[] ?? categoryEntities.ToArray();
            categoryRepositoryMock.Setup(x => x.ListAsync()).ReturnsAsync(enumerable);
            _fixture.Inject(categoryRepositoryMock.Object);
            _fixture.Freeze<ICategoryRepository>();
            var sut = _fixture.Create<CategoryService>();

            // Act
            var categories = await sut.ListAsync(true);

            // Assert
            var categoryList = categories as Data.Domain.Category[] ?? categories.ToArray();
            categoryList.Length.Should().Be(enumerable.Length);
            foreach (var category in categoryList)
            {
                var expected = enumerable.FirstOrDefault(x => Equals(x.Id, category.Id));

                expected.Should().NotBeNull();
                expected.Colour.Should().Be(category.Colour);
                expected.ImageUri.Should().Be(category.ImageUri);
                expected.Name.Should().Be(category.Name);
                expected.Id.Should().Be(category.Id);
                expected.CreatedDate.Should().Be(category.CreatedDate);
                expected.ModifiedDate.Should().Be(category.ModifiedDate);
            }
        }
    }
}
