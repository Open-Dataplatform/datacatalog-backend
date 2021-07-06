using System.Linq;
using AutoFixture;
using DataCatalog.Common.UnitTests.Extensions;
using DataCatalog.DatasetResourceManagement.UnitTests.SpecimenBuilders;

namespace DataCatalog.DatasetResourceManagement.UnitTests.AutoMoqData
{
    public class AutoMoqInfrastructureAttribute : MoqAutoDataAttribute
    {
        public AutoMoqInfrastructureAttribute()
            : base(() =>
            {
                var fixture = new Fixture()
                    .Customize(new CreateGroupsInAccessControlListSpecimenBuilder())
                    .Customize(new PathAccessControlSpecimenBuilder());

                fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                    .ForEach(b => fixture.Behaviors.Remove(b));
                fixture.Behaviors.Add(new OmitOnRecursionBehavior());
                
                return fixture;
            })
        {
        }
    }
}