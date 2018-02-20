using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models
{
    public class CookingPowerLevel
    {
        [JsonProperty("@type")]
        public string PowerLevel { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}