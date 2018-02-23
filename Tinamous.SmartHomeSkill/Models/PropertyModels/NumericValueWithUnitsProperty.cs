using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models.PropertyModels
{
    public class NumericValueWithUnitsProperty : Property
    {
        /// <summary>
        /// Might be PropertyValue (25.0, CELSIUS), or string ("HEAT")
        /// </summary>
        [JsonProperty("value")]
        public TemperaturePropertyValue Value { get; set; }
    }
}