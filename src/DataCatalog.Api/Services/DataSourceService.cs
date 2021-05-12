using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Api.Data.Model;
using DataCatalog.Api.Repositories;
using Energinet.DataPlatform.Shared.Environments;

namespace DataCatalog.Api.Services
{
    public class DataSourceService : IDataSourceService
    {
        private readonly IDataSourceRepository _dataSourceRepository;
        private readonly IUnitIOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private static string _environment;

        public DataSourceService(IEnvironment environment, IDataSourceRepository dataSourceRepository, IMapper mapper, IUnitIOfWork unitIOfWork)
        {
            _dataSourceRepository = dataSourceRepository;
            _unitOfWork = unitIOfWork;
            _mapper = mapper;
            _environment = environment.Name.ToLower();
        }

        public async Task<IEnumerable<Data.Domain.DataSource>> ListAsync()
        {
            var dataSources = await _dataSourceRepository.ListAsync();
            var result = dataSources.Select(x => _mapper.Map<Data.Domain.DataSource>(x));

            return result;
        }

        public async Task<Data.Domain.DataSource> FindByIdAsync(Guid id)
        {
            var dataSource = await _dataSourceRepository.FindByIdAsync(id);

            if (dataSource != null)
                return _mapper.Map<Data.Domain.DataSource>(dataSource);

            return null;
        }

        public async Task SaveAsync(Data.Domain.DataSource dataSource)
        {
            var dataSourceEntity = new DataSource
            {
                Id = dataSource.Id,
                Name = dataSource.Name,
                ContactInfo = dataSource.ContactInfo,
                Description = dataSource.Description,
                SourceType = dataSource.SourceType,
                CreatedDate = dataSource.CreatedDate,
                ModifiedDate = dataSource.ModifiedDate,
                OriginEnvironment = _environment
            };

            await _dataSourceRepository.AddAsync(dataSourceEntity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(Data.Domain.DataSource dataSource)
        {
            var existingDataSource = await _dataSourceRepository.FindByIdAsync(dataSource.Id);

            if (existingDataSource == null)
                return;

            existingDataSource.Name = dataSource.Name;
            existingDataSource.ContactInfo = dataSource.ContactInfo;
            existingDataSource.Description = dataSource.Description;
            existingDataSource.SourceType = dataSource.SourceType;
            existingDataSource.ModifiedDate = DateTime.UtcNow;

            _dataSourceRepository.Update(existingDataSource);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingDataSource = await _dataSourceRepository.FindByIdAsync(id);

            if (existingDataSource == null)
                return;

            _dataSourceRepository.Remove(existingDataSource);
            await _unitOfWork.CompleteAsync();
        }
    }
}