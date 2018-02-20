using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models
{
    public class RequestEndpoint
    {
        /// <summary>
        /// A device identifier. The identifier must be unique across all devices owned by an end user within the domain for the skill. In addition, the identifier needs to be consistent across multiple discovery requests for the same device. An identifier can contain letters or numbers, spaces, and the following special characters: _ - = # ; : ? @ &. The identifier cannot exceed 256 characters.
        /// </summary>
        [JsonProperty("endpointId")]
        public string EndpointId { get; set; }

        // And cookie which we'll ignore now...

        [JsonProperty("scope")]
        public Scope Scope { get; set; }
    }
}