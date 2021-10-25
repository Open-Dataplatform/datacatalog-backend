using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Repositories;
using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Serilog.Context;

namespace DataCatalog.Api.Services.AD
{
    public class AzureGroupService : BaseGroupService, IGroupService
    {
        private readonly IGraphServiceClient _graphServiceClient;
        private readonly IUnitOfWork _unitOfWork;
        
        private readonly ILogger<AzureGroupService> _logger;

        public AzureGroupService(
            IGraphServiceClient graphServiceClient,
            ILogger<AzureGroupService> logger,
            IDatasetChangeLogRepository datasetChangeLogRepository,
            Current current, 
            IUnitOfWork unitOfWork) : base(datasetChangeLogRepository, current)
        {
            _graphServiceClient = graphServiceClient;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public override async Task<IEnumerable<AccessMember>> GetGroupMembersAsync(string groupId)
        {
            try
            {
                var members = await _graphServiceClient.Groups[groupId].Members.Request().GetAsync();
                using(LogContext.PushProperty("GroupMembers", members, true))
                {
                    _logger.LogDebug("Fetched group members from groupId {GroupId}", groupId);
                }
                return members.Select(GetMemberInfo);
            }

            catch (ServiceException se)
            {
                _logger.LogInformation(se, "Service exception caught from the graph api when trying to get members for group {GroupId}", groupId);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception caught when retrieving group members for group {GroupId}", groupId);
                throw;
            }
        }

        public override async Task<AccessMember> GetAccessMemberAsync(string groupId)
        {
            try
            {
                var directoryObject = await _graphServiceClient.DirectoryObjects[groupId].Request().GetAsync();
                var accessMember = GetMemberInfo(directoryObject);
                using(LogContext.PushProperty("AccessMembers", accessMember, true))
                {
                    _logger.LogDebug("Fetched access members from groupId {GroupId}", groupId);
                }
                return accessMember;
            }
            catch (ServiceException se)
            {
                _logger.LogInformation(se, "Service exception caught from the graph api when trying to get member with id {GroupId}", groupId);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception caught when retrieving member with id {GroupId}", groupId);
                throw;
            }
        }

        public override async Task RemoveGroupMemberAsync(Guid datasetId, string groupId, string memberId, Common.Enums.AccessType accessType)
        {
            try
            {
                var accessMember = await GetAccessMemberAsync(memberId);
                AddChangeLog(datasetId, accessType, PermissionChangeType.Removed, accessMember, groupId);

                await _graphServiceClient.Groups[groupId].Members[memberId].Reference.Request().DeleteAsync();
                _logger.LogInformation("Removed the member with Id {MemberId} from the group with Id {GroupId}", memberId, groupId);

                await _unitOfWork.CompleteAsync(); // Save the new changelog entry once we know the operation did not trow an error
            }
            catch (ServiceException se)
            {
                _logger.LogWarning(se, "Service exception caught from the graph api when trying to remove member {MemberId} from group {GroupId}", memberId, groupId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception caught when removing member {MemberId} from group {GroupId}", memberId, groupId);
                throw;
            }
        }

        public override async Task<AccessMember> AddGroupMemberAsync(Guid datasetId, string groupId, string memberId, Common.Enums.AccessType accessType)
        {
            try
            {
                var members = await _graphServiceClient.Groups[groupId].Members.Request().GetAsync();
                var accessMember = await GetAccessMemberAsync(memberId);

                // Just return if member is already present in the group
                if (members.Any(x => Equals(x.Id, memberId)))
                {
                    _logger.LogInformation("Member with Id {MemberId} is already part of the group with Id {GroupId}", memberId, groupId);
                    return accessMember;
                }

                AddChangeLog(datasetId, accessType, PermissionChangeType.Added, accessMember, groupId);

                await _graphServiceClient.Groups[groupId].Members.References.Request()
                    .AddAsync(new DirectoryObject { Id = memberId });

                _logger.LogInformation("Added the member with Id {MemberId} to the group with Id {GroupId}", memberId, groupId);

                await _unitOfWork.CompleteAsync(); // Save the new changelog entry once we know the operation did not trow an error

                return accessMember;
            }
            catch (ServiceException se)
            {
                _logger.LogWarning(se, "Service exception caught from the graph api when trying to add member {MemberId} from group {GroupId}", memberId, groupId);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception caught when adding member {MemberId} to group {GroupId}", memberId, groupId);
                throw;
            }
        }

        public override async Task<IEnumerable<AdSearchResult>> SearchAsync(string searchString)
        {
            var displaySearchQueryOption = new QueryOption("$search", $"\"displayName:{searchString}\"");
            var mailSearchQueryOption = new QueryOption("$search", $"\"mail:{searchString}\"");
            var consistencyLevelHeader = new HeaderOption("ConsistencyLevel", "eventual");

            var groupsTask = _graphServiceClient.Groups.Request(new List<Option> { displaySearchQueryOption, consistencyLevelHeader }).GetAsync();
            var usersTask = _graphServiceClient.Users.Request(new List<Option> { displaySearchQueryOption, consistencyLevelHeader }).GetAsync();
            var mailTask = _graphServiceClient.Users.Request(new List<Option> { mailSearchQueryOption, consistencyLevelHeader }).GetAsync();
            var servicePrincipalsTask = _graphServiceClient.ServicePrincipals.Request(new List<Option> { displaySearchQueryOption, consistencyLevelHeader }).GetAsync();

            await Task.WhenAll(groupsTask, usersTask, mailTask, servicePrincipalsTask);

            var result = groupsTask.Result.Where(x => !x.GroupTypes.Contains("Unified")).Select(x => new AdSearchResult {Id = x.Id, DisplayName = x.DisplayName, Type = AdSearchResultType.Group})
                .Union(usersTask.Result.Select(x => new AdSearchResult { Id = x.Id, DisplayName = x.DisplayName, Mail = x.Mail, Type = AdSearchResultType.User }))
                .Union(mailTask.Result.Select(x => new AdSearchResult { Id = x.Id, DisplayName = x.DisplayName, Mail = x.Mail, Type = AdSearchResultType.User }))
                .Union(servicePrincipalsTask.Result.Select(x => new AdSearchResult { Id = x.Id, DisplayName = x.DisplayName, Type = AdSearchResultType.ServicePrincipal }));

            return result;
        }

        private AccessMember GetMemberInfo(DirectoryObject memberDirectoryObject)
        {
            switch (memberDirectoryObject)
            {
                case Group groupMember:
                    return new AccessMember {Id = groupMember.Id, Name = groupMember.DisplayName, Type = AccessMemberType.Group};
                case Microsoft.Graph.User userMember:
                    return new AccessMember { Id = userMember.Id, Name = userMember.DisplayName, Mail = userMember.Mail, Type = AccessMemberType.User };
                case ServicePrincipal servicePrincipalMember:
                    return new AccessMember { Id = servicePrincipalMember.Id, Name = servicePrincipalMember.DisplayName, Type = AccessMemberType.ServicePrincipal };
                default:
                    return new AccessMember {Id = memberDirectoryObject.Id, Name = memberDirectoryObject.Id, Type = AccessMemberType.Other};
            }
        }
    }
}
