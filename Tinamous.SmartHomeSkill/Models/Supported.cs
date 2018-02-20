using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models
{
    public class Supported
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}