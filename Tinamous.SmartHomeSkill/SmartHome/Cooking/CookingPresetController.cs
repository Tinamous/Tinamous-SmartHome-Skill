using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;

namespace Tinamous.SmartHome.SmartHome.Cooking
{
    /// <summary>
    /// User: Alexa, microwave the popcorn
    /// User: Alexa, cook the pizza in the microwave
    /// User: Alexa, cook chocolate chip cookies in the oven
    /// https://developer.amazon.com/docs/device-apis/alexa-cooking-presetcontroller.html
    /// </summary>
    public class CookingPresetController : AlexaInterfaceControllerBase
    {

        public Task<object> HandleCookingPresetController(SmartHomeRequest request, ILambdaContext context)
        {
            // CookByPreset

            throw new NotImplementedException();
            // CookingErrorResponse
        }
    }
}