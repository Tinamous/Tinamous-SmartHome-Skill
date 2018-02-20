using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models
{
    public class StateReportEndpoint
    {
        [JsonProperty("scope")]
        public Scope Scope { get; set; }

        [JsonProperty("endpointId")]
        public string EndpointId { get; set; }
    }
}