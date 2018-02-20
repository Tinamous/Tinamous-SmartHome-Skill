using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models
{
    public class BrightnessControlResponse
    {
        [JsonProperty("context")]
        public Context Context { get; set; }

        [JsonProperty("event")]
        public Event Event { get; set; }
    }
}