using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Repositories;
using DataCatalog.Api.Services.AD;
using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using DataCatalog.Common.Utils;

namespace DataCatalog.Api.Services.Local
{
    /// <summary>
    /// Dummy Group service implementation used ONLY for the local environment runtime.
    /// DO NOT use this in any other context! 
    /// </summary>
    public class LocalGroupService : BaseGroupService, IGroupService
    {
        private readonly AccessMember _localAccessMember;
        private readonly AdSearchResult _localSearchResult;
        private readonly IUnitOfWork _unitOfWork;

        public LocalGroupService(IDatasetChangeLogRepository datasetChangeLogRepository, Current current, IUnitOfWork unitOfWork) : base(datasetChangeLogRepository, current)
        {
            if (!EnvironmentUtil.IsDevelopment())
            {
                throw new InvalidOperationException("This class cannot be used unless the environment is local");
            }

            _localAccessMember = new AccessMember()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "LocalMember",
                Mail = "local@test.dk",
                Type = AccessMemberType.User
            };
            _localSearchResult = new AdSearchResult()
            {
                DisplayName = "LocalSearchResult",
                Id = Guid.NewGuid().ToString(),
                Mail = "local@test.dk",
                Type = "LocalSearchResultType"
            };
            _unitOfWork = unitOfWork;
        }

        public override Task<IEnumerable<AccessMember>> GetGroupMembersAsync(string groupId)
        {
            return Task.FromResult(new List<AccessMember> {_localAccessMember}.AsEnumerable());
        }

        public override Task<AccessMember> GetAccessMemberAsync(string groupId)
        {
            return Task.FromResult(_localAccessMember);
        }

        public override async Task RemoveGroupMemberAsync(Guid datasetId, string groupId, string memberId, AccessType accessType)
        {
            var accessMember = await GetAccessMemberAsync(memberId);
            AddChangeLog(datasetId, accessType, PermissionChangeType.Removed, accessMember, groupId);

            await _unitOfWork.CompleteAsync();
        }

        public async override Task<AccessMember> AddGroupMemberAsync(Guid datasetId, string groupId, string memberId, AccessType accessType)
        {
            var accessMember = await GetAccessMemberAsync(memberId);
            AddChangeLog(datasetId, accessType, PermissionChangeType.Added, accessMember, groupId);
            
            await _unitOfWork.CompleteAsync();

            return accessMember;
        }

        public override Task<IEnumerable<AdSearchResult>> SearchAsync(string searchString)
        {
            return Task.FromResult(new List<AdSearchResult> {_localSearchResult}.AsEnumerable());
        }
    }
}