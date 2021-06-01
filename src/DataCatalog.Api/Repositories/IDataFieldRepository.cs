using DataCatalog.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public interface IDataFieldRepository
    {
        Task<IEnumerable<DataField>> ListAsync();
        Task AddAsync(DataField dataField);
        Task<DataField> FindByIdAsync(Guid id);
        void Update(DataField dataField);
        void Remove(DataField dataField);
    }
}
