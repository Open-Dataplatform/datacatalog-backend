using AutoFixture;
using DataCatalog.Api.UnitTests.SpecimenBuilders;

namespace DataCatalog.Api.UnitTests.Extensions
{
    /// <summary>
    /// Extends the <see cref="T:AutoFixture.IFixture" /> interface to make it possible to add specimen builders directly to a fixture without the need for an <see cref="T:AutoFixture.ICustomization" /> instance.
    /// </summary>
    public static class FixtureExtensions
    {
        /// <summary>
        /// Adds a typed specimen builder to the <see cref="T:AutoFixture.IFixture" />.
        /// </summary>
        /// <typeparam name="T">Type of object that is created by the specimen builder.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="specimenBuilder">A typed specimen builder</param>
        /// <returns>The same fixture for chaining calls</returns>
        public static IFixture Customize<T>(
            this IFixture fixture,
            ISpecimenBuilder<T> specimenBuilder)
        {
            fixture.Customizations.Add(new TypedSpecimenBuilder<T>(specimenBuilder));
            return fixture;
        }
    }
}