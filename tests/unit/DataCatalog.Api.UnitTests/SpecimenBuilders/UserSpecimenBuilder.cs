﻿using AutoFixture;
using AutoFixture.Kernel;
using Energinet.DataPlatform.Shared.Testing.AutoFixture;
using Microsoft.Graph;

namespace DataCatalog.Api.UnitTests.SpecimenBuilders
{
    public class UserSpecimenBuilder : ISpecimenBuilder<User>
    {
        public User Create(ISpecimenContext context)
        {
            IFixture fixture = new Fixture();

            return new User
            {
                Id = fixture.Create<string>(),
                DisplayName = fixture.Create<string>()
            };
        }
    }
}
