using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using PgnNotifications.Client.Handlers;

namespace PgnNotifications.Client.Tests.UnitTests
{
    [TestFixture]
    public class RetryHandlerTests
    {
        [Test]
        public async Task RetryHandler_ReturnsOnFirstSuccess()
        {
            // Arrange
            var innerHandlerMock = new Mock<HttpMessageHandler>();
            innerHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();

            var handler = new RetryHandler(innerHandlerMock.Object, maxRetries: 3);
            var invoker = new HttpMessageInvoker(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "http://test.local/api");

            // Act
            var response = await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            innerHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task RetryHandler_RetriesOnFailure_AndReturnsLastResponse()
        {
            // Arrange
            var callCount = 0;
            var innerHandlerMock = new Mock<HttpMessageHandler>();
            innerHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(() =>
                {
                    callCount++;
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                });

            var handler = new RetryHandler(innerHandlerMock.Object, maxRetries: 3);
            var invoker = new HttpMessageInvoker(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "http://test.local/api");

            // Act
            var response = await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual(3, callCount);
        }

        [Test]
        public async Task RetryHandler_StopsOnSuccessWithinRetries()
        {
            // Arrange
            var callCount = 0;
            var innerHandlerMock = new Mock<HttpMessageHandler>();
            innerHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(() =>
                {
                    callCount++;
                    if (callCount < 2)
                        return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            var handler = new RetryHandler(innerHandlerMock.Object, maxRetries: 5);
            var invoker = new HttpMessageInvoker(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "http://test.local/api");

            // Act
            var response = await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(2, callCount);
        }
    }
}
