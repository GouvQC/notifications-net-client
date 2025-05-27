using Newtonsoft.Json;
using System.Collections.Generic;

namespace PgnNotifications.Models.Responses
{
    public class ReceivedTextListResponse
    {
        [JsonProperty("received_text_messages")]
        public List<ReceivedTextResponse> receivedTexts;
        [JsonProperty("links")]
        public Link links;
    }
}
