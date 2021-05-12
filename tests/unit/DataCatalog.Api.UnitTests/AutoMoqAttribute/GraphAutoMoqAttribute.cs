﻿using System.Linq;
using AutoFixture;
using DataCatalog.Api.UnitTests.SpecimenBuilders;
using Energinet.DataPlatform.Shared.Testing.AutoFixture;

namespace DataCatalog.Api.UnitTests.AutoMoqAttribute
{
    public class GraphAutoMoqAttribute : MoqAutoDataAttribute
    {
        public GraphAutoMoqAttribute()
            : base(() =>
            {
                var fixture = new Fixture()
                    .Customize(new GroupSpecimenBuilder())
                    .Customize(new UserSpecimenBuilder())
                    .Customize(new ServicePrincipalSpecimenBuilder());

                fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                    .ForEach(b => fixture.Behaviors.Remove(b));
                fixture.Behaviors.Add(new OmitOnRecursionBehavior());

                return fixture;
            })
        {
        }
    }
}
