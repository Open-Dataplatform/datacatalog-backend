using System.Linq;
using AutoFixture;
using DataCatalog.Api.UnitTests.SpecimenBuilders;
using Energinet.DataPlatform.Shared.Testing.AutoFixture;

namespace DataCatalog.Api.UnitTests.AutoMoqAttribute
{
    public class MapperAutoMoqAttribute : MoqAutoDataAttribute
    {
        public MapperAutoMoqAttribute()
            : base(() =>
            {
                var fixture = new Fixture()
                    .Customize(new MapperSpecimenBuilder());

                fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                    .ForEach(b => fixture.Behaviors.Remove(b));
                fixture.Behaviors.Add(new OmitOnRecursionBehavior());

                return fixture;
            })
        {
        }
    }
}
