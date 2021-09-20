using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Repositories;

namespace DataCatalog.Api.Services
{
    public class ServiceLevelAgreementService : IServiceLevelAgreementService
    {
        private readonly IMapper _mapper;
        private readonly IServiceLevelAgreementRepository _serviceLevelAgreementRepository;

        public ServiceLevelAgreementService(IMapper mapper, IServiceLevelAgreementRepository serviceLevelAgreementRepository)
        {
            _mapper = mapper;
            _serviceLevelAgreementRepository = serviceLevelAgreementRepository;
        }

        public async Task<IEnumerable<ServiceLevelAgreement>> ListAsync()
        {

            var agreements = await _serviceLevelAgreementRepository.ListAsync();
            var result = agreements.Select(x => _mapper.Map<Data.Domain.ServiceLevelAgreement>(x));

            return result;
        }
    }
}