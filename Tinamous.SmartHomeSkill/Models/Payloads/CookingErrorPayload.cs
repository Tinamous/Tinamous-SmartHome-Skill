using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models.Payloads
{
    public class CookingErrorPayload : ErrorPayload
    {
        [JsonProperty("maxCookTime")]
        public string MaxCookTime { get; set; }
    }
}