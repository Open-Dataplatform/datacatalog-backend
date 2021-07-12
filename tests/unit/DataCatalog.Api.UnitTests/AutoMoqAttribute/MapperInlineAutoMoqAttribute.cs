using System.Linq;
using AutoFixture;
using DataCatalog.Api.UnitTests.SpecimenBuilders;
using DataCatalog.Common.UnitTests.AutoMoqAttribute;
using DataCatalog.Common.UnitTests.Extensions;

namespace DataCatalog.Api.UnitTests.AutoMoqAttribute
{
    public class MapperInlineAutoMoqAttribute : MoqInlineAutoDataAttribute
    {
        public MapperInlineAutoMoqAttribute(params object[] values)
            : base(() =>
            {
                var fixture = new Fixture()
                    .Customize(new MapperSpecimenBuilder());

                fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                    .ForEach(b => fixture.Behaviors.Remove(b));
                fixture.Behaviors.Add(new OmitOnRecursionBehavior());

                return fixture;
            }, values)
        {
        }
    }
}
