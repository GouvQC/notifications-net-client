using System;
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
    public class LoggingHandlerTests
    {
        [Test]
        public async Task LoggingHandler_LogsRequestAndResponse()
        {
            // Arrange
            var logOutput = string.Empty;
            Action<string> logAction = msg => logOutput += msg + "\n";

            var innerHandlerMock = new Mock<HttpMessageHandler>();
            innerHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    ReasonPhrase = "OK"
                })
                .Verifiable();

            var handler = new LoggingHandler(innerHandlerMock.Object, logAction);
            var invoker = new HttpMessageInvoker(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "http://test.local/api");

            // Act
            var response = await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            StringAssert.Contains("[LOG] Request: GET http://test.local/api", logOutput);
            StringAssert.Contains("[LOG] Response: 200 OK", logOutput);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            innerHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task LoggingHandler_UsesDefaultConsoleWriteLine_WhenNoLogActionProvided()
        {
            // Arrange
            var innerHandlerMock = new Mock<HttpMessageHandler>();
            innerHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    ReasonPhrase = "OK"
                });

            var handler = new LoggingHandler(innerHandlerMock.Object);
            var invoker = new HttpMessageInvoker(handler);
            var request = new HttpRequestMessage(HttpMethod.Post, "http://test.local/api");

            // Act & Assert (no exception should be thrown)
            var response = await invoker.SendAsync(request, CancellationToken.None);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
