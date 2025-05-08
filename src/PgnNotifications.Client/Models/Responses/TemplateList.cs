using Newtonsoft.Json;
using System.Collections.Generic;

namespace PgnNotifications.Models.Responses
{
    public class TemplateList
    {
        [JsonProperty("templates")]
        public List<TemplateResponse> templates;
    }
}
