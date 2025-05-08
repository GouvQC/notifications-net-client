using Newtonsoft.Json;

namespace PgnNotifications.Models
{
    public class NotifyHTTPError
    {
#pragma warning disable 169
        [JsonProperty("error")]
        private string error;

        [JsonProperty("message")]
        private string message;
    }
}
