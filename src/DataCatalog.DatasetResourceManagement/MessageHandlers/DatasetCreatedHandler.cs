using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Api.Messages;
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
        private readonly IBus _bus;

        public DatasetCreatedHandler(ILogger<DatasetCreatedHandler> logger, 
            IStorageService storageService, 
            IActiveDirectoryGroupService activeDirectoryGroupService, 
            IAccessControlListService accessControlListService, 
            IActiveDirectoryGroupProvider activeDirectoryGroupProvider, 
            IActiveDirectoryRootGroupProvider activeDirectoryRootGroupProvider, 
            IAllUsersGroupProvider allUsersGroupProvider, 
            IBus bus,
            IOptions<RebusOptions> rebusOptions) : base(logger, rebusOptions, bus)
        {
            _logger = logger;
            _storageService = storageService;
            _activeDirectoryGroupService = activeDirectoryGroupService;
            _accessControlListService = accessControlListService;
            _activeDirectoryGroupProvider = activeDirectoryGroupProvider;
            _activeDirectoryRootGroupProvider = activeDirectoryRootGroupProvider;
            _allUsersGroupProvider = allUsersGroupProvider;
            _bus = bus;
        }

        public override async Task Handle(DatasetCreatedMessage datasetCreatedMessage)
        {
            try
            {
                var path = datasetCreatedMessage.DatasetId.ToString();

                var rootGroupId = await _activeDirectoryRootGroupProvider.ProvideGroupAsync(
                    $"DataPlatform-{Constants.Container}-Zone-{EnvironmentUtil.GetCurrentEnvironment()}-Reader",
                    $"Root reader group for the {Constants.Container} zone", Constants.Container);

                await _storageService.CreateDirectoryIfNeeded(Constants.Container, path);

                string readerGroupId;
                string writerGroupId;

                await using (var lease = await _storageService.AcquireLeaseAsync(Constants.Container, path))
                {
                    readerGroupId = await _activeDirectoryGroupProvider.ProvideGroupAsync(
                        GenerateGroupDisplayName(datasetCreatedMessage.DatasetId, ReadWriteGroup.Read),
                        GenerateGroupDescription(datasetCreatedMessage, ReadWriteGroup.Read), 
                        new[] { datasetCreatedMessage.Owner });

                    writerGroupId = await _activeDirectoryGroupProvider.ProvideGroupAsync(
                        GenerateGroupDisplayName(datasetCreatedMessage.DatasetId, ReadWriteGroup.Write),
                        GenerateGroupDescription(datasetCreatedMessage, ReadWriteGroup.Write));

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

                await _activeDirectoryGroupService.AddGroupMember(rootGroupId, readerGroupId);
                await _activeDirectoryGroupService.AddGroupMember(rootGroupId, writerGroupId);
                
                if (datasetCreatedMessage.Public)
                    await _activeDirectoryGroupService.AddGroupMember(readerGroupId, _allUsersGroupProvider.GetAllUsersGroup());

                await _bus.Publish(new DatasetProvisionedMessage
                    {
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