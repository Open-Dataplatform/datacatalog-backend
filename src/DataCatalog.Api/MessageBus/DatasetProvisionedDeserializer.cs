using System;
using System.Text.Json;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;
using DataCatalog.Api.Data.Messages;

namespace DataCatalog.Api.MessageBus
{
    public class DatasetProvisionedDeserializer
    {
        public static DatasetProvisioned Deserialize(string body)
        {
            var parsedMessage = JsonSerializer.Deserialize<DatasetProvisionedMessage>(body);

            return new DatasetProvisioned
            {
                DatasetId = Guid.Parse(parsedMessage.DatasetId),
                Status = (ProvisionDatasetStatusEnum)Enum.Parse(typeof(ProvisionDatasetStatusEnum), parsedMessage.Status, true),
                Error = parsedMessage.Error
            };
        }
    }
}
