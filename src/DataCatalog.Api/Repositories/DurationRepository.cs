using DataCatalog.Api.Data;
using DataCatalog.Api.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public class DurationRepository : BaseRepository, IDurationRepository
    {
        public DurationRepository(DataCatalogContext context) : base(context)
        { }

        public async Task<IEnumerable<Duration>> ListAsync()
        {
            return await _context.Durations.ToListAsync();
        }

        public async Task<Duration> FindByIdAsync(Guid id)
        {
            return await _context.Durations.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Duration> FindByCodeAsync(string code)
        {
            return await _context.Durations.FirstOrDefaultAsync(a => a.Code == code);
        }

        public async Task AddAsync(Duration duration)
        {
            await _context.Durations.AddAsync(duration);
        }

        public void Update(Duration duration)
        {
            _context.Durations.Update(duration);
        }

        public void Remove(Duration duration)
        {
            _context.Durations.Remove(duration);
        }
    }
}
