using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models.PropertyModels
{
    public class ValueValueProperty : Property
    {
        [JsonProperty("value")]
        public ValuePropertyValue Value { get; set; }
    }
}