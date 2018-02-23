using System;
using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models.PropertyModels
{
    public class DateTimeValueProperty : Property
    {
        [JsonProperty("value")]
        public DateTime Value { get; set; }
    }
}