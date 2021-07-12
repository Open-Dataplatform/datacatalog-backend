using DataCatalog.Api.Controllers;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Common.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace DataCatalog.Api.UnitTests.Controllers
{
    public class UserControllerTests
    {
        [Fact]
        public void GetUserInfo_ShouldReturnNameAndRole()
        {
            // ARRANGE
            var name = Guid.NewGuid().ToString();
            var userController = new UserController()
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(
                            new[]
                            {
                                new Claim(ClaimsUtility.ClaimName, name),
                                new Claim(ClaimTypes.Role, Role.DataSteward.ToString())
                            }, "auth"))
                    }
                }
            };

            // ACT
            var result = userController.GetUserInfo();

            // ASSERT
            result.Should().NotBeNull("because the endpoint must return user info");
            var user = result.Value;
            user.Should().NotBeNull("because the endpoint must return a object of type Data.Domain.User");
            user.Name.Should().Be(name, "because that was the users name");
            user.Roles.Should().NotBeNull("because the endpoint should always return a list");
            user.Roles.Count().Should().Be(1, "because the user has one role");
            user.Roles.First().Should().Be("DataSteward", "because the users role is DataSteward");
        }
    }
}
