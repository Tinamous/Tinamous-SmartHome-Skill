using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models
{
    public class Scope
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}