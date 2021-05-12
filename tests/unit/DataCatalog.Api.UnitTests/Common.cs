using Microsoft.EntityFrameworkCore;
using System;

namespace DataCatalog.Api.UnitTests
{
    public static class Common
    {
        public static DbContextOptions<T> GetInMemoryDbOptions<T>()
            where T: DbContext
            => new DbContextOptionsBuilder<T>()
               .UseInMemoryDatabase(databaseName: "DataCatalog_" + Guid.NewGuid())
               .Options;
    }
}