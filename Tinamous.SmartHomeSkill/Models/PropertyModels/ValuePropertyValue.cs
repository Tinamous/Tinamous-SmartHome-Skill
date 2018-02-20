using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models.PropertyModels
{
    public class ValuePropertyValue
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}