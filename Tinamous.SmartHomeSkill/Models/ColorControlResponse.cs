using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models
{
    public class ColorControlResponse
    {
        [JsonProperty("context")]
        public Context Context { get; set; }

        [JsonProperty("event")]
        public Event Event { get; set; }
    }
}