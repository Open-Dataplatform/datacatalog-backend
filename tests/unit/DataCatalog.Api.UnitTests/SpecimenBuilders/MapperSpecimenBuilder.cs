using AutoFixture.Kernel;
using AutoMapper;
using DataCatalog.Api.Data;
using DataCatalog.Common.UnitTests.SpecimenBuilders;

namespace DataCatalog.Api.UnitTests.SpecimenBuilders
{
    public class MapperSpecimenBuilder : ISpecimenBuilder<IMapper>
    {
        public IMapper Create(ISpecimenContext context)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperProfile>();
            });

            return config.CreateMapper();
        }
    }
}
