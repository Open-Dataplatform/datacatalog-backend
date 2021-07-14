using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Data.Model;
using DataCatalog.Api.Repositories;

namespace DataCatalog.Api.Services
{
    public class MemberGroupService : IMemberGroupService
    {
        private readonly IMemberGroupRepository _memberGroupRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberGroupService(IMemberGroupRepository memberGroupRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _memberGroupRepository = memberGroupRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Data.Domain.MemberGroup>> ListAsync()
        {
            var memberGroups = await _memberGroupRepository.ListAsync();

            return memberGroups.Select(x => _mapper.Map<Data.Domain.MemberGroup>(x));
        }

        public async Task<Data.Domain.MemberGroup> FindByIdAsync(Guid id)
        {
            var memberGroup = await _memberGroupRepository.FindByIdAsync(id);

            if (memberGroup != null)
                return _mapper.Map<Data.Domain.MemberGroup>(memberGroup);

            return null;
        }

        public async Task<IEnumerable<Data.Domain.MemberGroup>> GetMemberGroups(Guid memberId)
        {
            var memberGroups = await _memberGroupRepository.GetMemberGroups(memberId);

            return memberGroups.Select(x => _mapper.Map<Data.Domain.MemberGroup>(x));
        }

        public async Task SaveAsync(Data.Domain.MemberGroup memberGroup)
        {
            var memberGroupEntity = new MemberGroup
            {
                Id = memberGroup.Id,
                Name = memberGroup.Name,
                Description = memberGroup.Description,
                Email = memberGroup.Email,
                CreatedDate = memberGroup.CreatedDate,
                ModifiedDate = memberGroup.ModifiedDate
            };

            await _memberGroupRepository.AddAsync(memberGroupEntity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(Data.Domain.MemberGroup memberGroup)
        {
            var existingMemberGroup = await _memberGroupRepository.FindByIdAsync(memberGroup.Id);

            if (existingMemberGroup == null)
                return;

            existingMemberGroup.Name = memberGroup.Name;
            existingMemberGroup.Description = memberGroup.Description;
            existingMemberGroup.Email = memberGroup.Email;
            existingMemberGroup.ModifiedDate = DateTime.UtcNow;

            _memberGroupRepository.Update(existingMemberGroup);
            await _unitOfWork.CompleteAsync();
        }

        public async Task AddMemberAsync(Guid memberGroupId, Guid memberId)
        {
            await _memberGroupRepository.AddMemberAsync(memberGroupId, memberId);
            await _unitOfWork.CompleteAsync();
        }

        public async Task RemoveMemberAsync(Guid memberGroupId, Guid memberId)
        {
            await _memberGroupRepository.RemoveMemberAsync(memberGroupId, memberId);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingMemberGroup = await _memberGroupRepository.FindByIdAsync(id);

            if (existingMemberGroup == null)
                return;

            _memberGroupRepository.Remove(existingMemberGroup);
            await _unitOfWork.CompleteAsync();
        }
    }
}