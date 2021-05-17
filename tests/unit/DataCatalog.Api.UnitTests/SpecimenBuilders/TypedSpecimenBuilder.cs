using AutoFixture.Kernel;
using System;
using System.Reflection;

namespace DataCatalog.Api.UnitTests.SpecimenBuilders
{
    internal class TypedSpecimenBuilder<T> : ISpecimenBuilder
    {
        private readonly ISpecimenBuilder<T> _specimenBuilder;

        public TypedSpecimenBuilder(ISpecimenBuilder<T> specimenBuilder) => _specimenBuilder = specimenBuilder;

        public object Create(object request, ISpecimenContext context)
        {
            var type = request as Type;
            if (((object) type == null || type != typeof (T)) && (request is not ParameterInfo parameterInfo || parameterInfo.ParameterType != typeof (T)))
            {
                var propertyInfo = request as PropertyInfo;
                if ((object) propertyInfo == null || propertyInfo.PropertyType != typeof (T))
                    return new NoSpecimen();
            }
            return _specimenBuilder.Create(context);
        }
    }
}