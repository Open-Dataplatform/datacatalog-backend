using AutoMapper;
using DataCatalog.Data.Model;
using DataCatalog.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using DataCatalog.Api.Exceptions;

namespace DataCatalog.Api.Services
{
    public class DatasetGroupService : IDatasetGroupService
    {
        private readonly IDatasetGroupRepository _datasetGroupRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IDatasetRepository _datasetRepository;
        private readonly Current _current;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DatasetGroupService(Current current, IDatasetGroupRepository datasetGroupRepository, IMemberRepository memberRepository, IDatasetRepository datasetRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _datasetGroupRepository = datasetGroupRepository;
            _memberRepository = memberRepository;
            _datasetRepository = datasetRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _current = current;
        }

        public async Task<IEnumerable<Data.Domain.DatasetGroup>> ListAsync()
        {
            var datasetGroups = await _datasetGroupRepository.ListAsync();

            if (!_current.Roles.Contains(Role.Admin))
                datasetGroups = datasetGroups.Where(d => d.MemberId == _current.MemberId);

            return datasetGroups.Select(x => _mapper.Map<Data.Domain.DatasetGroup>(x));
        }

        public async Task<Data.Domain.DatasetGroup> FindByIdAsync(Guid id)
        {
            var datasetGroup = await _datasetGroupRepository.FindByIdAsync(id);

            if (datasetGroup == null)
                return null;

            if (!_current.Roles.Contains(Role.Admin) && datasetGroup.MemberId != _current.MemberId)
                return null;

            return _mapper.Map<Data.Domain.DatasetGroup>(datasetGroup);
        }

        public async Task<Data.Domain.DatasetGroup> SaveAsync(Data.Domain.DatasetGroup datasetGroup)
        {
            await ValidateAsync(datasetGroup);

            var newDatasetGroup = new DatasetGroup
            {
                Id = Guid.NewGuid(),
                MemberId = datasetGroup.MemberId,
                Name = datasetGroup.Name,
                Description = datasetGroup.Description,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            newDatasetGroup.DatasetGroupDatasets = datasetGroup.DatasetGroupDatasets.Select(d => new DatasetGroupDataset
            {
                DatasetId = d.DatasetId,
                DatasetGroup = newDatasetGroup,
                CreatedDate = DateTime.Now
            }).ToList();

            await _datasetGroupRepository.AddAsync(newDatasetGroup);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<Data.Domain.DatasetGroup>(await FindByIdAsync(newDatasetGroup.Id));
        }

        public async Task<Data.Domain.DatasetGroup> UpdateAsync(Data.Domain.DatasetGroup datasetGroup)
        {
            // Validation
            await ValidateAsync(datasetGroup);
            if (datasetGroup.Id == null)
                throw new ValidationException("Id of dataset group to update must not be null");
            var existingDatasetGroup = await _datasetGroupRepository.FindByIdAsync(datasetGroup.Id.Value);
            if (existingDatasetGroup == null)
                throw new ValidationException("DatasetGroup was not found");

            // Update basic properties
            existingDatasetGroup.Name = datasetGroup.Name;
            existingDatasetGroup.Description = datasetGroup.Description;
            existingDatasetGroup.ModifiedDate = DateTime.UtcNow;
            
            // Create new DatasetGroupDataset
            foreach (var datasetGroupDataset in datasetGroup.DatasetGroupDatasets)
            {
                if (existingDatasetGroup.DatasetGroupDatasets.All(d => d.DatasetId != datasetGroupDataset.DatasetId))
                    existingDatasetGroup.DatasetGroupDatasets.Add(new DatasetGroupDataset
                    {
                        DatasetId = datasetGroupDataset.DatasetId,
                        DatasetGroup = existingDatasetGroup,
                        CreatedDate = DateTime.Now
                    });
            }

            // Delete DatasetGroupDatasets
            foreach (var datasetGroupDataset in existingDatasetGroup.DatasetGroupDatasets)
            {
                if (datasetGroup.DatasetGroupDatasets.All(d => d.DatasetId != datasetGroupDataset.DatasetId))
                    existingDatasetGroup.DatasetGroupDatasets.Remove(datasetGroupDataset);
            }

            _datasetGroupRepository.Update(existingDatasetGroup);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<Data.Domain.DatasetGroup>(existingDatasetGroup);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingDatasetGroup = await _datasetGroupRepository.FindByIdAsync(id);

            if (existingDatasetGroup == null)
                throw new NotFoundException();

            _datasetGroupRepository.Remove(existingDatasetGroup);
            await _unitOfWork.CompleteAsync();
        }

        private async Task ValidateAsync(Data.Domain.DatasetGroup datasetGroup)
        {
            var exceptions = new List<ValidationException>();
            var admin = _current.Roles.Contains(Role.Admin);

            var member = await _memberRepository.FindByIdAsync(datasetGroup.MemberId);
            if (member == null)
                exceptions.Add(new ValidationException("Member not found with id"));

            if (!admin && datasetGroup.MemberId != _current.MemberId)
                exceptions.Add(new ValidationException("You can only create DatasetGroups for yourself"));

            foreach (var datasetGroupDataset in datasetGroup.DatasetGroupDatasets)
            {
                var dataset = await _datasetRepository.FindByIdAsync(datasetGroupDataset.DatasetId);
                if (dataset == null)
                    exceptions.Add(new ValidationException($"Dataset with id '{datasetGroupDataset.DatasetId}' not found"));
            }

            if (exceptions.Any())
                throw new ValidationExceptionCollection("DatasetGroup could not be created", exceptions);
        }
    }
}
