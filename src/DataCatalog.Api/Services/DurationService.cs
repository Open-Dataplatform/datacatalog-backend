using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Data.Model;
using DataCatalog.Api.Repositories;

namespace DataCatalog.Api.Services
{
    public class DurationService : IDurationService
    {
        private readonly IDurationRepository _durationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DurationService(IDurationRepository durationRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _durationRepository = durationRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Data.Domain.Duration>> ListAsync()
        {
            var categories = await _durationRepository.ListAsync();
            var result = categories.Select(x => _mapper.Map<Data.Domain.Duration>(x));

            return result;
        }

        public async Task<Data.Domain.Duration> FindByIdAsync(Guid id)
        {
            var duration = await _durationRepository.FindByIdAsync(id);

            if (duration != null)
                return _mapper.Map<Data.Domain.Duration>(duration);

            return null;
        }

        public async Task SaveAsync(Data.Domain.Duration duration)
        {
            var durationEntity = new Duration
            {
                Id = duration.Id,
                Code = duration.Code,
                Description = duration.Description,
                CreatedDate = duration.CreatedDate,
                ModifiedDate = duration.ModifiedDate
            };

            await _durationRepository.AddAsync(durationEntity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(Data.Domain.Duration duration)
        {
            var existingDuration = await _durationRepository.FindByIdAsync(duration.Id);

            if (existingDuration == null)
                return;

            existingDuration.Code = duration.Code;
            existingDuration.Description = duration.Description;
            existingDuration.ModifiedDate = DateTime.UtcNow;

            _durationRepository.Update(existingDuration);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingDuration = await _durationRepository.FindByIdAsync(id);

            if (existingDuration == null)
                return;

            _durationRepository.Remove(existingDuration);
            await _unitOfWork.CompleteAsync();
        }
    }
}