using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Data.Model;
using DataCatalog.Api.Repositories;

namespace DataCatalog.Api.Services
{
    public class HierarchyService : IHierarchyService
    {
        private readonly IHierarchyRepository _hierarchyRepository;
        private readonly IUnitIOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HierarchyService(IHierarchyRepository hierarchyRepository, IMapper mapper, IUnitIOfWork unitIOfWork)
        {
            _hierarchyRepository = hierarchyRepository;
            _unitOfWork = unitIOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Data.Domain.Hierarchy>> ListAsync()
        {
            var hierarchies = await _hierarchyRepository.ListAsync();
            var result = hierarchies.Select(x => _mapper.Map<Data.Domain.Hierarchy>(x));

            return result;
        }

        public async Task<Data.Domain.Hierarchy> FindByIdAsync(Guid id)
        {
            var hierarchy = await _hierarchyRepository.FindByIdAsync(id);

            if (hierarchy != null)
                return _mapper.Map<Data.Domain.Hierarchy>(hierarchy);

            return null;
        }

        public async Task SaveAsync(Data.Domain.Hierarchy hierarchy)
        {
            var hierarchyEntity = new Hierarchy
            {
                Id = hierarchy.Id,
                Name = hierarchy.Name,
                Description = hierarchy.Description,
                ParentHierarchyId = hierarchy.ParentHierarchyId,
                CreatedDate = hierarchy.CreatedDate,
                ModifiedDate = hierarchy.ModifiedDate
            };

            await _hierarchyRepository.AddAsync(hierarchyEntity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(Data.Domain.Hierarchy hierarchy)
        {
            var existingHierarchy = await _hierarchyRepository.FindByIdAsync(hierarchy.Id);

            if (existingHierarchy == null)
                return;

            existingHierarchy.Name = hierarchy.Name;
            existingHierarchy.Description = hierarchy.Description;
            existingHierarchy.ParentHierarchyId = hierarchy.ParentHierarchyId;
            existingHierarchy.ModifiedDate = DateTime.UtcNow;

            await CheckForCyclicHierarchies(hierarchy.Id);

            _hierarchyRepository.Update(existingHierarchy);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingHierarchy = await _hierarchyRepository.FindByIdAsync(id);

            if (existingHierarchy == null)
                return;

            _hierarchyRepository.Remove(existingHierarchy);
            await _unitOfWork.CompleteAsync();
        }

        public async Task CheckForCyclicHierarchies(Guid hierarchyId)
        {
            var current = await _hierarchyRepository.FindByIdAsync(hierarchyId);
            while (current.ParentHierarchyId.HasValue)
            {
                var parent = await _hierarchyRepository.FindByIdAsync(current.ParentHierarchyId.Value);
                if (!parent.ParentHierarchyId.HasValue)
                    break;
                if (parent.Id == hierarchyId)
                    throw new Exception("Cyclic hierarchies are not allowed");
                current = parent;
            }
        }
    }
}