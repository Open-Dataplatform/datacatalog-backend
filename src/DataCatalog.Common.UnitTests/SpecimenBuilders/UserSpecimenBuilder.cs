using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.Graph;

namespace DataCatalog.Common.UnitTests.SpecimenBuilders
{
    public class UserSpecimenBuilder : ISpecimenBuilder<User>
    {
        public User Create(ISpecimenContext context)
        {
            IFixture fixture = new Fixture();

            return new User
            {
                Id = fixture.Create<string>(),
                DisplayName = fixture.Create<string>()
            };
        }
    }
}
