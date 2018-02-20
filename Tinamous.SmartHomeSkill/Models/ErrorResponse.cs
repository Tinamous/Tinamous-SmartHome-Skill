using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models
{
    public class ErrorResponse
    {
        [JsonProperty("event")]
        public Event Event { get; set; }
    }
}