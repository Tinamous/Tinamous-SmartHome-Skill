using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models.PropertyModels
{
    public class FloatValueProperty : Property
    {
        [JsonProperty("value")]
        public float Value { get; set; }
    }
}