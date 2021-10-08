using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Api.Messages;
using DataCatalog.Common.Interfaces;
using DataCatalog.Common.Rebus;
using DataCatalog.Common.Utils;
using DataCatalog.DatasetResourceManagement.Commands.AccessControlList;
using DataCatalog.DatasetResourceManagement.Common;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.Storage;
using DataCatalog.DatasetResourceManagement.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rebus.Bus;
using Rebus.Retry.Simple;
using IAllUsersGroupProvider = DataCatalog.Common.Interfaces.IAllUsersGroupProvider;

namespace DataCatalog.DatasetResourceManagement.MessageHandlers
{
    public class DatasetCreatedHandler : AbstractMessageHandler<DatasetCreatedMessage>
    {
        private readonly ILogger<DatasetCreatedHandler> _logger;
        private readonly IStorageService _storageService;
        private readonly IActiveDirectoryGroupService _activeDirectoryGroupService;
        private readonly IAccessControlListService _accessControlListService;
        private readonly IActiveDirectoryGroupProvider _activeDirectoryGroupProvider;
        private readonly IActiveDirectoryRootGroupProvider _activeDirectoryRootGroupProvider;
        private readonly IAllUsersGroupProvider _allUsersGroupProvider;
        private readonly ICorrelationIdResolver _correlationIdResolver;
        private readonly IBus _bus;

        public DatasetCreatedHandler(ILogger<DatasetCreatedHandler> logger, 
            IStorageService storageService, 
            IActiveDirectoryGroupService activeDirectoryGroupService, 
            IAccessControlListService accessControlListService, 
            IActiveDirectoryGroupProvider activeDirectoryGroupProvider, 
            IActiveDirectoryRootGroupProvider activeDirectoryRootGroupProvider, 
            IAllUsersGroupProvider allUsersGroupProvider, 
            IBus bus,
            ICorrelationIdResolver correlationIdResolver,
            IOptions<RebusOptions> rebusOptions) : base(logger, rebusOptions, bus)
        {
            _logger = logger;
            _storageService = storageService;
            _activeDirectoryGroupService = activeDirectoryGroupService;
            _accessControlListService = accessControlListService;
            _activeDirectoryGroupProvider = activeDirectoryGroupProvider;
            _activeDirectoryRootGroupProvider = activeDirectoryRootGroupProvider;
            _allUsersGroupProvider = allUsersGroupProvider;
            _correlationIdResolver = correlationIdResolver;
            _bus = bus;
        }

        public override async Task Handle(DatasetCreatedMessage datasetCreatedMessage)
        {
            try
            {
                var path = datasetCreatedMessage.DatasetId.ToString();
                _logger.LogInformation("Received Dataset created message for Dataset Id {DatasetId}. Starting provisioning process", datasetCreatedMessage.DatasetId);

                var rootGroupId = await _activeDirectoryRootGroupProvider.ProvideGroupAsync(
                    $"DataPlatform-{Constants.Container}-Zone-{EnvironmentUtil.GetCurrentEnvironment()}-Reader",
                    $"Root reader group for the {Constants.Container} zone", Constants.Container);

                await _storageService.CreateDirectoryIfNeeded(Constants.Container, path);

                // Create reader and writer groups in parallel as they are both very slow and independent of each other
                var readerGroupIdTask = _activeDirectoryGroupProvider.ProvideGroupAsync(
                    GenerateGroupDisplayName(datasetCreatedMessage.DatasetId, ReadWriteGroup.Read),
                    GenerateGroupDescription(datasetCreatedMessage, ReadWriteGroup.Read), 
                    new[] { datasetCreatedMessage.Owner });

                var writerGroupIdTask = _activeDirectoryGroupProvider.ProvideGroupAsync(
                    GenerateGroupDisplayName(datasetCreatedMessage.DatasetId, ReadWriteGroup.Write),
                    GenerateGroupDescription(datasetCreatedMessage, ReadWriteGroup.Write));

                var readerGroupId = await readerGroupIdTask;
                var writerGroupId = await writerGroupIdTask;

                await using (var lease = await _storageService.AcquireLeaseAsync(Constants.Container, path))
                {
                    await _accessControlListService.SetGroupsInAccessControlListAsync(new CreateGroupsInAccessControlList
                    {
                        StorageContainer = Constants.Container,
                        Path = path,
                        GroupEntries = new List<AccessControlListGroupEntry>
                    {
                        new() {Id = readerGroupId, Permissions = "r-x"},
                        new() {Id = readerGroupId, Permissions = "r-x", IsDefault = true},
                        new() {Id = writerGroupId, Permissions = "rwx"}, 
                        new() {Id = writerGroupId, Permissions = "rwx", IsDefault = true}
                    }
                    }, lease.LeaseId);

                    await _storageService.SetDirectoryMetadata(Constants.Container, path,
                        new Dictionary<string, string>
                        {
                        { "ReaderGroupId", readerGroupId },
                        { "WriterGroupId", writerGroupId },
                        { "RootDirectoryDatasetId", datasetCreatedMessage.DatasetId.ToString() }
                        }, lease.LeaseId);

                    await _accessControlListService.RemoveGroupFromAccessControlListAsync(Constants.Container, path, rootGroupId, lease.LeaseId);
                }

                _logger.LogDebug("Adding reader and writer groups with ids {ReaderGroupId}, {WriterGroupId}", readerGroupId, writerGroupId);
                await _activeDirectoryGroupService.AddGroupMember(rootGroupId, readerGroupId);
                await _activeDirectoryGroupService.AddGroupMember(rootGroupId, writerGroupId);

                if (datasetCreatedMessage.AddAllUsersGroup)
                {
                    _logger.LogInformation("New dataset is set to public, so we add the all users group to the reader group");
                    await _activeDirectoryGroupService.AddGroupMember(readerGroupId,
                        _allUsersGroupProvider.GetAllUsersGroup());
                }

                _logger.LogInformation("Successfully provisioned the dataset with Id {Id}", datasetCreatedMessage.DatasetId);
                await _bus.Publish(new DatasetProvisionedMessage
                    {
                        CorrelationId = _correlationIdResolver.GetCorrelationId(),
                        DatasetId = datasetCreatedMessage.DatasetId, 
                        Status = "Succeeded"
                    }
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred during processing of dataset created event");
                throw;
            }
        }

        protected override async void ActionExecutedUponMessageDeadLettered(IFailed<DatasetCreatedMessage> datasetCreatedMessage)
        {
            _logger.LogError("Failed to provision the dataset. Informing the world about this via a failed DatasetProvisionedMessage. Error description: {ErrorDescription}", datasetCreatedMessage.ErrorDescription);
            await _bus.Publish(new DatasetProvisionedMessage
                {
                    DatasetId = datasetCreatedMessage.Message.DatasetId,
                    Status = "Failed"
                }
            );
        }

        private static string GenerateGroupDisplayName(Guid datasetId, ReadWriteGroup groupType)
        {
            return groupType switch
            {
                ReadWriteGroup.Read =>
                    $"DataPlatform-DataSet_{datasetId}-{EnvironmentUtil.GetCurrentEnvironment()}-Reader",
                ReadWriteGroup.Write =>
                    $"DataPlatform-DataSet_{datasetId}-{EnvironmentUtil.GetCurrentEnvironment()}-Writer",
                _ => throw new ArgumentOutOfRangeException(nameof(groupType), groupType, "group type must be reader or writer")
            };
        }
        
        private static string GenerateGroupDescription(DatasetCreatedMessage datasetCreatedMessage, ReadWriteGroup groupType)
        {
            return groupType switch
            {
                ReadWriteGroup.Read =>
                    $"Reader group with id {datasetCreatedMessage.DatasetId} and name at creation {datasetCreatedMessage.DatasetName}",
                ReadWriteGroup.Write =>
                    $"Writer group with id {datasetCreatedMessage.DatasetId} and name at creation {datasetCreatedMessage.DatasetName}",
                _ => throw new ArgumentOutOfRangeException(nameof(groupType), groupType, "group type must be reader or writer")
            };
        }
    }

    internal enum ReadWriteGroup
    {
        Read,
        Write
    }
}