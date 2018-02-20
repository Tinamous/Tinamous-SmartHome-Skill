using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models
{
    public class Payload
    {
        [JsonProperty("scope")]
        public Scope Scope { get; set; }

        /// <summary>
        /// SetPowerLevel:  https://developer.amazon.com/docs/device-apis/alexa-powerlevelcontroller.html
        /// </summary>
        /// <remarks>0..100</remarks>
        [JsonProperty("powerLevel")]
        public int PowerLevel { get; set; }

        /// <summary>
        /// AdjustPowerLevel: https://developer.amazon.com/docs/device-apis/alexa-powerlevelcontroller.html
        /// </summary>
        /// <remarks>-100..100</remarks>
        [JsonProperty("powerLevelDelta")]
        public int PowerLevelDelta { get; set; }

        /// <summary>
        /// Set Brightness: https://developer.amazon.com/docs/device-apis/alexa-brightnesscontroller.html
        /// </summary>
        [JsonProperty("brightness")]
        public int Brightness { get; set; }

        /// <summary>
        /// Adjust Brightness: https://developer.amazon.com/docs/device-apis/alexa-brightnesscontroller.html
        /// </summary>
        [JsonProperty("brightnessDelta")]
        public int BrightnessDelta { get; set; }

        /// <summary>
        /// Set percentage: https://developer.amazon.com/docs/device-apis/alexa-percentagecontroller.html
        /// </summary>
        [JsonProperty("percentage")]
        public int Percentage { get; set; }

        /// <summary>
        /// Adjust percentage: https://developer.amazon.com/docs/device-apis/alexa-percentagecontroller.html
        /// </summary>
        [JsonProperty("percentageDelta")]
        public int PercentageDelta { get; set; }

        /// <summary>
        /// Set Color: https://developer.amazon.com/docs/device-apis/alexa-colorcontroller.html
        /// </summary>
        [JsonProperty("color")]
        public HsvColor Color { get; set; }

        //"cookTime": "PT3M"
        [JsonProperty("cookTime")]
        public string CookTime { get; set; }

        [JsonProperty("cookingPowerLevel")]
        public CookingPowerLevel CookingPowerLevel { get; set; }
    }
}