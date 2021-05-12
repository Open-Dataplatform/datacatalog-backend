﻿using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Kernel;
using Energinet.DataPlatform.Shared.Testing.AutoFixture;
using Microsoft.Graph;

namespace DataCatalog.Api.UnitTests.SpecimenBuilders
{
    public class GroupSpecimenBuilder : ISpecimenBuilder<Group>
    {
        public Group Create(ISpecimenContext context)
        {
            IFixture fixture = new Fixture();

            return new Group
            {
                Id = fixture.Create<string>(), 
                DisplayName = fixture.Create<string>(),
                GroupTypes = new List<string>()
            };
        }
    }
}
