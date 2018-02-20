using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models
{
    public class EventPayload
    {
        [JsonProperty("endpoints")]
        public List<Endpoint> Endpoints { get; set; }

        /// <summary>
        /// Used for error messages (e.g. ENDPOINT_UNREACHABLE)
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Used for error messages
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}