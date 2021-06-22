using AutoMapper;
using DataCatalog.Data.Model;
using DataCatalog.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using DataCatalog.Common.Utils;

namespace DataCatalog.Api.Services
{
    public class DataContractService : IDataContractService
    {
        private readonly IDataContractRepository _dataContractRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _environment;

        public DataContractService(IDataContractRepository dataContractRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _dataContractRepository = dataContractRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _environment = EnvironmentUtil.GetCurrentEnvironment().ToLower();
        }

        public async Task<IEnumerable<Data.Domain.DataContract>> ListAsync()
        {
            var categories = await _dataContractRepository.ListAsync();
            return categories.Select(x => _mapper.Map<Data.Domain.DataContract>(x));
        }

        public async Task<Data.Domain.DataContract> FindByIdAsync(Guid id)
        {
            var dataContract = await _dataContractRepository.FindByIdAsync(id);

            if (dataContract != null)
                return _mapper.Map<Data.Domain.DataContract>(dataContract);

            return null;
        }

        public async Task<Data.Domain.DataContract[]> GetByDatasetIdAsync(Guid datasetId)
        {
            var dataContracts = await _dataContractRepository.GetByDatasetIdAsync(datasetId);

            if (dataContracts != null)
                return _mapper.Map<Data.Domain.DataContract[]>(dataContracts);

            return null;
        }

        public async Task<Data.Domain.DataContract[]> GetByDataSourceIdAsync(Guid dataSourceId)
        {
            var dataContracts = await _dataContractRepository.GetByDataSourceIdAsync(dataSourceId);

            if (dataContracts != null)
                return _mapper.Map<Data.Domain.DataContract[]>(dataContracts);

            return null;
        }

        public async Task SaveAsync(Data.Domain.DataContract dataContract)
        {
            var dataContractEntity = new DataContract
            {
                Id = dataContract.Id,
                DatasetId = dataContract.DatasetId,
                DataSourceId = dataContract.DataSourceId,
                CreatedDate = dataContract.CreatedDate,
                ModifiedDate = dataContract.ModifiedDate,
                OriginEnvironment = _environment
            };

            await _dataContractRepository.AddAsync(dataContractEntity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(Data.Domain.DataContract dataContract)
        {
            var existingDataContract = await _dataContractRepository.FindByIdAsync(dataContract.Id);

            if (existingDataContract == null)
                return;

            existingDataContract.DatasetId = dataContract.DatasetId;
            existingDataContract.DataSourceId = dataContract.DataSourceId;
            existingDataContract.ModifiedDate = DateTime.UtcNow;

            _dataContractRepository.Update(existingDataContract);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingDataContract = await _dataContractRepository.FindByIdAsync(id);

            if (existingDataContract == null)
                return;

            _dataContractRepository.Remove(existingDataContract);
            await _unitOfWork.CompleteAsync();
        }
    }
}