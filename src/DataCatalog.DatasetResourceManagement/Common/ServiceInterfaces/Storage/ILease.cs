using System;

namespace DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.Storage
{
    public interface ILease : IAsyncDisposable
    {
        public string LeaseId { get; }
    }
}