using Newtonsoft.Json;
using System.Collections.Generic;
using PgnNotifications.Models.Responses;

namespace PgnNotifications.Models
{
    public class NotificationList
    {
        [JsonProperty("notifications")]
        public List<Notification> notifications;
        [JsonProperty("links")]
        public Link links;
    }
}
