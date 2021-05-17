using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.Graph;

namespace DataCatalog.Api.UnitTests.SpecimenBuilders
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
