using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;

namespace Tinamous.SmartHome.SmartHome.Cooking
{
    /// <summary>
    /// https://developer.amazon.com/docs/device-apis/alexa-timeholdcontroller.html
    /// </summary>
    public class CookingTimeHoldController : AlexaInterfaceControllerBase
    {
        public Task<object> HandleCookingTimeHoldController(SmartHomeRequest request, ILambdaContext context)
        {
            // Hold
            // Resume
            throw new NotImplementedException();
            // CookingErrorResponse
        }
    }
}