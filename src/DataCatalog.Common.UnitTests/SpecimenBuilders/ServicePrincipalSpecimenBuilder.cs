using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.Graph;

namespace DataCatalog.Common.UnitTests.SpecimenBuilders
{
    public class ServicePrincipalSpecimenBuilder : ISpecimenBuilder<ServicePrincipal>
    {
        public ServicePrincipal Create(ISpecimenContext context)
        {
            IFixture fixture = new Fixture();

            return new ServicePrincipal
            {
                Id = fixture.Create<string>(),
                DisplayName = fixture.Create<string>()
            };
        }
    }
}
