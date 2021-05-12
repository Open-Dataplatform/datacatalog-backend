using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Api.Repositories;

namespace DataCatalog.Api.Services
{
    public class IdentityProviderService : IIdentityProviderService
    {
        private readonly IIdentityProviderRepository _identityProviderRepository;
        private readonly IMapper _mapper;

        public IdentityProviderService(IIdentityProviderRepository identityProviderRepository, IMapper mapper)
        {
            _identityProviderRepository = identityProviderRepository;
            _mapper = mapper;
        }

        public async Task<Data.Domain.IdentityProvider> FindByTenantIdAsync(string id)
        {
            var identityProvider = await _identityProviderRepository.FindByTenantIdAsync(id);

            if (identityProvider != null)
                return _mapper.Map<Data.Domain.IdentityProvider>(identityProvider);

            return null;
        }
    }
}