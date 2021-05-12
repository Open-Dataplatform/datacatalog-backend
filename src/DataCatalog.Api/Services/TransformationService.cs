using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Api.Data.Model;
using DataCatalog.Api.Repositories;

namespace DataCatalog.Api.Services
{
    public class TransformationService : ITransformationService
    {
        private readonly ITransformationRepository _transformationRepository;
        private readonly IUnitIOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransformationService(ITransformationRepository transformationRepository, IMapper mapper, IUnitIOfWork unitIOfWork)
        {
            _transformationRepository = transformationRepository;
            _unitOfWork = unitIOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Data.Domain.Transformation>> ListAsync()
        {
            var transformations = await _transformationRepository.ListAsync();
            return transformations.Select(x => _mapper.Map<Data.Domain.Transformation>(x));
        }

        public async Task<Data.Domain.Transformation> FindByIdAsync(Guid id)
        {
            var transformation = await _transformationRepository.FindByIdAsync(id);

            if (transformation != null)
                return _mapper.Map<Data.Domain.Transformation>(transformation);

            return null;
        }

        public async Task<IEnumerable<Data.Domain.Transformation>> GetByDatasetIdsAsync(Guid[] datasetIds)
        {
            var transformations = await _transformationRepository.GetByDatasetIdsAsync(datasetIds);
            return transformations.Select(x => _mapper.Map<Data.Domain.Transformation>(x));
        }

        public async Task SaveAsync(Data.Domain.Transformation transformation)
        {
            var transformationEntity = new Transformation
            {
                Id = transformation.Id,
                ShortDescription = transformation.ShortDescription,
                Description = transformation.Description,
                CreatedDate = transformation.CreatedDate,
                ModifiedDate = transformation.ModifiedDate
            };

            await _transformationRepository.AddAsync(transformationEntity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(Data.Domain.Transformation transformation)
        {
            var existingTransformation = await _transformationRepository.FindByIdAsync(transformation.Id);

            if (existingTransformation == null)
                return;

            existingTransformation.ShortDescription = transformation.ShortDescription;
            existingTransformation.Description = transformation.Description;
            existingTransformation.ModifiedDate = DateTime.UtcNow;

            _transformationRepository.Update(existingTransformation);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingTransformation = await _transformationRepository.FindByIdAsync(id);

            if (existingTransformation == null)
                return;

            await _transformationRepository.RemoveAsync(existingTransformation);
            await _unitOfWork.CompleteAsync();
        }
    }
}