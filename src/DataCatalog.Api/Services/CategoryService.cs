using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Repositories;
using DataCatalog.Common.Utils;

namespace DataCatalog.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDatasetCategoryRepository _datasetCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _environment;

        public CategoryService(ICategoryRepository categoryRepository, IDatasetCategoryRepository datasetCategoryRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _datasetCategoryRepository = datasetCategoryRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _environment = EnvironmentUtil.GetCurrentEnvironment().ToLower();
        }

        public async Task<IEnumerable<Category>> ListAsync(bool includeEmpty = false)
        {
            var categories = await _categoryRepository.ListAsync();
            var result = categories.Select(x => _mapper.Map<Category>(x));

            if (includeEmpty)
                return result;

            var datasetCategories = await _datasetCategoryRepository.ListAsync();
            var categoriesWithDataSets = datasetCategories.Select(x => x.CategoryId).Distinct();

            return result.Where(x => categoriesWithDataSets.Contains(x.Id));
        }

        public async Task<Category> FindByIdAsync(Guid id)
        {
            var category = await _categoryRepository.FindByIdAsync(id);

            if (category != null)
                return _mapper.Map<Category>(category);

            return null;
        }

        public async Task<Category> SaveAsync(Category category)
        {
            var categoryEntity = new DataCatalog.Data.Model.Category 
                                    { 
                                        Colour = category.Colour, 
                                        CreatedDate = category.CreatedDate, 
                                        Id = category.Id, 
                                        ImageUri = category.ImageUri, 
                                        ModifiedDate = category.ModifiedDate, 
                                        Name = category.Name, 
                                        OriginEnvironment = _environment
            };

            await _categoryRepository.AddAsync(categoryEntity);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<Category>(categoryEntity);
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            var existingCategory = await _categoryRepository.FindByIdAsync(category.Id);

            if (existingCategory == null)
                throw new ObjectNotFoundException($"Could not find category with id {category.Id}");

            existingCategory.Name = category.Name;
            existingCategory.Colour = category.Colour;
            existingCategory.ImageUri = category.ImageUri;
            existingCategory.ModifiedDate = DateTime.UtcNow;

            _categoryRepository.Update(existingCategory);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<Category>(existingCategory);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingCategory = await _categoryRepository.FindByIdAsync(id);

            if (existingCategory == null)
            {
                throw new ObjectNotFoundException($"Could not find category with id {id}");
            }

            var refs = await _datasetCategoryRepository.ListAsync(id);
            if (refs.Any())
            {
                throw new InvalidOperationException("This category has references to datasets and cannot de deleted");
            }

            _categoryRepository.Remove(existingCategory);
            await _unitOfWork.CompleteAsync();
        }
    }
}
