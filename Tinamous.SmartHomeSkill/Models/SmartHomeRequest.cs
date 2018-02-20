using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models
{
    public class SmartHomeRequest
    {
        [JsonProperty("directive")]
        public Directive Directive { get; set; }
    }
}