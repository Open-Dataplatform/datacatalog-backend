using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Repositories;
using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Services.AD
{
    public abstract class AbstractGroupService : IGroupService
    {
        protected readonly IDatasetChangeLogRepository _datasetChangeLogRepository;
        protected readonly Current _current;

        protected AbstractGroupService(IDatasetChangeLogRepository datasetChangeLogRepository, Current current)
        {
            _datasetChangeLogRepository = datasetChangeLogRepository;
            _current = current;
        }

        protected void AddChangeLog(Guid datasetId, Common.Enums.AccessType accessType, PermissionChangeType permissionChangeType, AccessMember accessMember, string accessGroupId)
        {
            _datasetChangeLogRepository.Add(
                new DataCatalog.Data.Model.DatasetChangeLog
                {
                    DatasetId = datasetId,
                    MemberId = _current.MemberId,
                    Name = _current.Name,
                    Email = _current.Email,
                    DatasetChangeType = DatasetChangeType.PermissionChange,
                    DatasetPermissionChange = new DataCatalog.Data.Model.DatasetPermissionChange
                    {
                        PermissionChangeType = permissionChangeType,
                        AccessType = accessType,
                        AccessMemberType = accessMember.Type,
                        DirectoryObjectId = accessMember.Id,
                        DisplayName = accessMember.Name,
                        Mail = accessMember.Mail,
                        AccessGroupId = accessGroupId
                    }
                }
            );
        }

        public abstract Task<AccessMember> AddGroupMemberAsync(Guid datasetId, string groupId, string memberId, AccessType accessType);
        public abstract Task<AccessMember> GetAccessMemberAsync(string groupId);
        public abstract Task<IEnumerable<AccessMember>> GetGroupMembersAsync(string groupId);
        public abstract Task RemoveGroupMemberAsync(Guid datasetId, string groupId, string memberId, AccessType accessType);
        public abstract Task<IEnumerable<AdSearchResult>> SearchAsync(string searchString);
    }
}