using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models.PropertyModels
{
    public class BooleanValueProperty : Property
    {
        [JsonProperty("value")]
        public bool Value { get; set; }
    }
}