using AutoFixture.Kernel;

namespace DataCatalog.Common.UnitTests.SpecimenBuilders
{
    /// <summary>
    /// Builds or helps building anonymous variables (specimens) that are typed.
    /// </summary>
    public interface ISpecimenBuilder<out T>
    {
        /// <summary>Creates a new typed specimen.</summary>
        /// <param name="context">A context that can be used to create other specimens.</param>
        /// <returns>The requested specimen if possible; otherwise a <see cref="T:AutoFixture.Kernel.NoSpecimen" /> instance.</returns>
        T Create(ISpecimenContext context);
    }
}