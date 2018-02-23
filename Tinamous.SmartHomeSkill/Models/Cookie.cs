using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models
{
    /// <summary>
    /// String name/value pairs that provide additional information about a device for use by the skill. The contents of this property cannot exceed 5000 bytes. The API doesn't use or understand this data.
    /// </summary>
    public class Cookie
    {
        /// <summary>
        /// The devices @Username in Tinamous
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        // TODO: Add port?, actual deviceId?, Display name, other???
    }
}