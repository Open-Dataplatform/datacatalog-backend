using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.Graph;

namespace DataCatalog.Common.UnitTests.SpecimenBuilders
{
    public class GroupSpecimenBuilder : ISpecimenBuilder<Group>
    {
        public Group Create(ISpecimenContext context)
        {
            IFixture fixture = new Fixture();

            return new Group
            {
                Id = fixture.Create<string>(), 
                DisplayName = fixture.Create<string>(),
                GroupTypes = new List<string>()
            };
        }
    }
}
