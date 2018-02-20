using System.Collections.Generic;
using Newtonsoft.Json;
using Tinamous.SmartHome.Models.PropertyModels;

namespace Tinamous.SmartHome.Models
{
    public class Context
    {
        [JsonProperty("properties")]
        public List<Property> Properties { get; set; }
    }
}