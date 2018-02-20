using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models.PropertyModels
{
    public class ColorValueProperty : Property
    {
        [JsonProperty("value")]
        public HsvColor Value { get; set; }
    }
}