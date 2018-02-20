using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models
{
    public class NumericPropertyValue
    {
        [JsonProperty("value")]
        public float Value { get; set; }

        [JsonProperty("scale")]
        public string Scale { get; set; }
    }
}