using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PgnNotifications.Client;
using PgnNotifications.Client.Builders;
using PgnNotifications.Utils;
using System.Collections.Generic;

namespace PgnNotifications.Client.Tests.UnitTests
{
    public class NotificationClientUserAgentTests
    {
        private class FakeHandler : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;

                // On retourne une réponse factice
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                };

                return Task.FromResult(response);
            }
        }

        [Test]
        public async Task NotificationClient_AddsIatToUserAgent()
        {
            // Arrange
            var handler = new FakeHandler();
            var client = new NotificationClientBuilder()
                .WithApiKey(Constants.fakeApiKey)
                .WithClientId(Constants.fakeClientId)
                .WithBaseUrl("https://fake-api.test")
                .WithHandlerBuilder(hb => hb.WithTestHandler(handler))
                .Build();

            // Act
            await client.SendSmsAsync(
                Constants.fakePhoneNumber,
                Constants.fakeTemplateId,
                new Dictionary<string, dynamic>()
            );

            // Assert
            Assert.IsNotNull(handler.LastRequest, "La requête n'a pas été interceptée.");

            var userAgent = handler.LastRequest!.Headers.UserAgent?.ToString() ?? string.Empty;

            Assert.That(userAgent, Does.Contain("NOTIFY-API-NET-CLIENT"), "Le User-Agent ne contient pas le nom du client.");

            // Récupérer le token réel depuis l'en-tête Authorization
            var authHeader = handler.LastRequest.Headers.Authorization?.Parameter;
            Assert.IsNotNull(authHeader, "Authorization header is missing.");

            var expectedIat = JwtUtils.GetIatFromToken(authHeader!);
            Assert.IsTrue(expectedIat.HasValue, "Impossible de récupérer l'iat depuis le token.");
            Assert.That(userAgent, Does.Contain(expectedIat.Value.ToString("o")), "Le User-Agent ne contient pas l'iat attendu.");
        }

    }
}
