using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models
{
    public class DiscoverResponse
    {
        [JsonProperty("event")]
        public Event Event { get; set; }
    }
}