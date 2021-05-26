using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using DataCatalog.Data.Model;
using FluentAssertions;
using Xunit;

namespace DataCatalog.Api.IntegrationTests.Controllers
{
    public class CategoryEndpointTests : BaseTest
    {
		[Fact(Skip="Unable to run integration tests in pipeline")]
		public void Get_NoAuthorization_ShouldFail()
		{
			// ARRANGE

			// ACT
			var response = SendRequest(HttpVerb.GET, Controller.Category, "", null, false);

			// ASSERT
			response.HttpWebResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

        [Fact(Skip = "Unable to run integration tests in pipeline")]
		public void Get_ValidAuthorization_ShouldReturnCategories()
		{
			// ARRANGE

			// ACT
			var response = SendRequest(HttpVerb.GET, Controller.Category, "", null, true);
			var categories = JsonSerializer.Deserialize<List<Category>>(response.Content);

			// ASSERT
			response.HttpWebResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			categories.Should().NotBeNull("because the response object should be a valid json array");
			categories.Count.Should().BeGreaterOrEqualTo(1, "because at least category exists");
		}
	}
}
