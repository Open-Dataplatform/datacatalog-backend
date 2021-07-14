using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.Services.ActiveDirectory.Middleware;
using DataCatalog.DatasetResourceManagement.UnitTests.AutoMoqData;
using Microsoft.Identity.Client;
using Moq;
using Shouldly;
using Xunit;

namespace DataCatalog.DatasetResourceManagement.UnitTests.Services.ActiveDirectory.Middleware
{
    // ReSharper disable once InconsistentNaming
    public class GroupAuthenticationMiddleware_Should
    {
        [Theory]
        [MoqAutoData]
        public async Task Append_Authorization_Header(
            Mock<DelegatingHandler> innerHandlerMock,
            [Frozen]Mock<ITokenProvider> tokenProviderMock,
            CancellationToken ct,
            AuthenticationResult authResult,
            GroupAuthenticationMiddleware sut)
        {   
            // Arrange
            tokenProviderMock.Setup(x => x.GetTokenAsync(It.IsAny<IEnumerable<string>>(), ct))
                .ReturnsAsync(authResult);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "");
            sut.InnerHandler = innerHandlerMock.Object;
            var invoker = new HttpMessageInvoker(sut);

            // Act
            await invoker.SendAsync(httpRequestMessage, ct);

            // Assert
            httpRequestMessage.Headers.Authorization.Parameter.ShouldBe(authResult.AccessToken);
        }

        [Theory]
        [MoqAutoData]
        public async Task Call_TokenProvider_With_Correct_Scope(
            Mock<DelegatingHandler> innerHandlerMock,
            [Frozen] Mock<ITokenProvider> tokenProviderMock,
            CancellationToken ct,
            AuthenticationResult authResult,
            GroupAuthenticationMiddleware sut)
        {
            // Arrange
            tokenProviderMock.Setup(x => x.GetTokenAsync(It.IsAny<IEnumerable<string>>(), ct))
                .ReturnsAsync(authResult);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "");
            sut.InnerHandler = innerHandlerMock.Object;
            var invoker = new HttpMessageInvoker(sut);

            // Act
            await invoker.SendAsync(httpRequestMessage, ct);

            // Assert
            tokenProviderMock.Verify(
                x => x.GetTokenAsync(
                    It.Is<IEnumerable<string>>(a => a.Contains("https://aadprovisioner.energinet.dk/.default")), ct),
                Times.Once);
        }

        [Theory]
        [MoqAutoData]
        public async Task Throw_When_Authentication_Fails(
            Mock<DelegatingHandler> innerHandlerMock,
            [Frozen] Mock<ITokenProvider> tokenProviderMock,
            CancellationToken ct,
            Exception e,
            GroupAuthenticationMiddleware sut)
        {
            // Arrange
            tokenProviderMock.Setup(x => x.GetTokenAsync(It.IsAny<IEnumerable<string>>(), ct))
                .Throws(e);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "");
            sut.InnerHandler = innerHandlerMock.Object;
            var invoker = new HttpMessageInvoker(sut);

            // Act
            var exThrown = await Assert.ThrowsAsync<Exception>(() => invoker.SendAsync(httpRequestMessage, ct));

            // Assert
            exThrown.ShouldBe(e);
        }
    }
}
