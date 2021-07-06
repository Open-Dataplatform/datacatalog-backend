using AutoFixture;
using AutoFixture.Kernel;
using DataCatalog.Common.UnitTests.SpecimenBuilders;
using DataCatalog.DatasetResourceManagement.Commands.AccessControlList;

namespace DataCatalog.DatasetResourceManagement.UnitTests.SpecimenBuilders
{
    public class CreateGroupsInAccessControlListSpecimenBuilder : ISpecimenBuilder<CreateGroupsInAccessControlList>
    {
        private readonly IFixture _fixture = new Fixture();
        
        public CreateGroupsInAccessControlList Create(ISpecimenContext context)
        {
            var entries = _fixture.Build<AccessControlListGroupEntry>().With(x => x.Permissions, "rwx").CreateMany();
            return _fixture.Build<CreateGroupsInAccessControlList>().With(x => x.GroupEntries, entries).Create();
        }
    }
}
