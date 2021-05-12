using DataCatalog.Api.Data;
using DataCatalog.Api.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public class DataFieldRepository : BaseRepository, IDataFieldRepository
    {
        public DataFieldRepository(DataCatalogContext context) : base(context)
        { }

        public async Task<IEnumerable<DataField>> ListAsync()
        {
            return await _context.DataFields.ToListAsync();
        }

        public async Task AddAsync(DataField dataField)
        {
            await _context.DataFields.AddAsync(dataField);
        }

        public async Task<DataField> FindByIdAsync(Guid id)
        {
            return await _context.DataFields.FirstOrDefaultAsync(a => a.Id == id);
        }

        public void Update(DataField dataField)
        {
            _context.DataFields.Update(dataField);
        }

        public void Remove(DataField dataField)
        {
            _context.DataFields.Remove(dataField);
        }
    }
}
