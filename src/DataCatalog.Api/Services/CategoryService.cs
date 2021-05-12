using AutoMapper;
using DataCatalog.Api.Data.Model;
using DataCatalog.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Energinet.DataPlatform.Shared.Environments;

namespace DataCatalog.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDatasetCategoryRepository _datasetCategoryRepository;
        private readonly IUnitIOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private static string _environment;

        public CategoryService(IEnvironment environment, ICategoryRepository categoryRepository, IDatasetCategoryRepository datasetCategoryRepository, IMapper mapper, IUnitIOfWork unitIOfWork)
        {
            _categoryRepository = categoryRepository;
            _datasetCategoryRepository = datasetCategoryRepository;
            _unitOfWork = unitIOfWork;
            _mapper = mapper;
            _environment = environment.Name.ToLower();
        }

        public async Task<IEnumerable<Data.Domain.Category>> ListAsync(bool includeEmpty = false)
        {
            var categories = await _categoryRepository.ListAsync();
            var result = categories.Select(x => _mapper.Map<Data.Domain.Category>(x));

            if (includeEmpty)
                return result;

            var datasetCategories = await _datasetCategoryRepository.ListAsync();
            var categoriesWithDataSets = datasetCategories.Select(x => x.CategoryId).Distinct();

            return result.Where(x => categoriesWithDataSets.Contains(x.Id));
        }

        public async Task<Data.Domain.Category> FindByIdAsync(Guid id)
        {
            var category = await _categoryRepository.FindByIdAsync(id);

            if (category != null)
                return _mapper.Map<Data.Domain.Category>(category);

            return null;
        }

        public async Task SaveAsync(Data.Domain.Category category)
        {
            var categoryEntity = new Category 
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
        }

        public async Task UpdateAsync(Data.Domain.Category category)
        {
            var existingCategory = await _categoryRepository.FindByIdAsync(category.Id);

            if (existingCategory == null)
                return;

            existingCategory.Name = category.Name;
            existingCategory.Colour = category.Colour;
            existingCategory.ImageUri = category.ImageUri;
            existingCategory.ModifiedDate = DateTime.UtcNow;

            _categoryRepository.Update(existingCategory);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingCategory = await _categoryRepository.FindByIdAsync(id);

            if (existingCategory == null)
                return;

            var refs = await _datasetCategoryRepository.ListAsync(id);
            if (refs.Any())
                throw new InvalidOperationException("This category has references to datasets and cannot de deleted");

            _categoryRepository.Remove(existingCategory);
            await _unitOfWork.CompleteAsync();
        }
    }
}
