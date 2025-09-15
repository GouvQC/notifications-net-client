using System;
using System.Text;
using System.Text.Json;

namespace PgnNotifications.Utils
{
    public static class JwtUtils
    {
        /// <summary>
        /// Extrait le champ 'iat' (issued-at) d’un JWT HMACSHA256.
        /// </summary>
        /// <param name="token">Le JWT en format string (xxxx.yyyy.zzzz)</param>
        /// <returns>DateTime UTC de l’iat si trouvé, sinon null</returns>
        public static DateTime? GetIatFromToken(string token)
        {
            try
            {
                var parts = token.Split('.');
                if (parts.Length < 2) return null;

                var payload = parts[1];
                var jsonBytes = Base64UrlDecode(payload);
                var json = Encoding.UTF8.GetString(jsonBytes);

                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("iat", out var iatElement))
                {
                    // Certains iat sont double (ex: 1757972643.0)
                    if (iatElement.ValueKind == JsonValueKind.Number)
                    {
                        long seconds = 0;
                        if (iatElement.TryGetInt64(out var i64)) seconds = i64;
                        else if (iatElement.TryGetDouble(out var d)) seconds = (long)d;

                        return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Decode Base64URL (JWT) en bytes
        /// </summary>
        private static byte[] Base64UrlDecode(string base64Url)
        {
            string padded = base64Url
                .Replace('-', '+')
                .Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            return Convert.FromBase64String(padded);
        }
    }
}
