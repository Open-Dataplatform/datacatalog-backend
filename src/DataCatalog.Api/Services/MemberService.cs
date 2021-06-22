using AutoMapper;
using DataCatalog.Data.Model;
using DataCatalog.Api.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataCatalog.Api.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberService(IMemberRepository memberRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _memberRepository = memberRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Data.Domain.Member> GetOrCreateAsync(string externalId, Guid identityProviderId)
        {
            var member = await _memberRepository.FindByExternalIdAsync(externalId, identityProviderId);
            if (member == null)
            {
                var zulu = DateTime.Now;
                member = new Member
                {
                    CreatedDate = zulu,
                    ModifiedDate = zulu,
                    IdentityProviderId = identityProviderId,
                    ExternalId = externalId
                };

                try
                {
                    await _memberRepository.AddAsync(member);
                    await _unitOfWork.CompleteAsync();
                }
                catch (DbUpdateException)
                {
                    member = await _memberRepository.FindByExternalIdAsync(externalId, identityProviderId);
                }
            }

            return _mapper.Map<Data.Domain.Member>(member);
        }
    }
}
