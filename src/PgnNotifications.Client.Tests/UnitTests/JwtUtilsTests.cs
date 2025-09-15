using System;
using NUnit.Framework;
using PgnNotifications.Utils;
using System.Text;

namespace PgnNotifications.Client.Tests.UnitTests
{
    [TestFixture]
    public class JwtUtilsTests
    {
        private static string Base64UrlEncode(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var base64 = Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
            return base64;
        }

        [Test]
        public void GetIatFromToken_ReturnsDateTime_WhenValidToken()
        {
            // Arrange : payload avec iat = 1700000000
            var payload = "{\"iat\":1700000000}";
            var base64Payload = Base64UrlEncode(payload);
            var token = $"header.{base64Payload}.signature";

            // Act
            var result = JwtUtils.GetIatFromToken(token);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(DateTimeOffset.FromUnixTimeSeconds(1700000000).UtcDateTime, result.Value);
        }

        [Test]
        public void GetIatFromToken_ReturnsNull_WhenNoIat()
        {
            var payload = "{\"sub\":\"1234567890\"}";
            var base64Payload = Base64UrlEncode(payload);
            var token = $"header.{base64Payload}.signature";

            var result = JwtUtils.GetIatFromToken(token);

            Assert.IsNull(result);
        }

        [Test]
        public void GetIatFromToken_ReturnsNull_WhenInvalidToken()
        {
            var token = "invalid.token";

            var result = JwtUtils.GetIatFromToken(token);

            Assert.IsNull(result);
        }
    }
}
