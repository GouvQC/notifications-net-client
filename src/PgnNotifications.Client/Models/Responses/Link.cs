using Newtonsoft.Json;

namespace PgnNotifications.Models.Responses
{
    public class Link
    {
        [JsonProperty("current")]
        public string current;
        [JsonProperty("next")]
        public string next;
    }
}
