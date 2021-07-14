using System.Linq;
using AutoFixture;

namespace DataCatalog.DatasetResourceManagement.UnitTests.AutoMoqData
{
    public class AutoMoqFunctionAttribute : MoqAutoDataAttribute
    {
        public AutoMoqFunctionAttribute() : base(() =>
        {
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        })
        { }
    }
}
