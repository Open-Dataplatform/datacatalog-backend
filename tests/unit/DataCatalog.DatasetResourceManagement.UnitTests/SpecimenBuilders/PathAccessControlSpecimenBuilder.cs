using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Kernel;
using Azure.Storage.Files.DataLake.Models;
using DataCatalog.Common.UnitTests.Extensions;
using DataCatalog.Common.UnitTests.SpecimenBuilders;

namespace DataCatalog.DatasetResourceManagement.UnitTests.SpecimenBuilders
{
    public class PathAccessControlSpecimenBuilder : ISpecimenBuilder<PathAccessControl>
    {
        private readonly IFixture _fixture = new Fixture();
        
        public PathAccessControl Create(ISpecimenContext context)
        {
            var instance = ReflectionExtensions.CreateInstance<PathAccessControl>();
            instance.SetInternalProperty(x => x.Permissions, _fixture.Build<PathPermissions>().Create());
            instance.SetInternalProperty(x => x.AccessControlList, _fixture.Create<IEnumerable<PathAccessControlItem>>());
            instance.SetInternalProperty(x => x.Group, _fixture.Create<string>());
            instance.SetInternalProperty(x => x.Owner, _fixture.Create<string>());
            
            return instance;
        }
    }
}
