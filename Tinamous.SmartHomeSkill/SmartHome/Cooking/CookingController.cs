using System;
using System.Data;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;

namespace Tinamous.SmartHome.SmartHome.Cooking
{
    /// <summary>
    /// https://developer.amazon.com/docs/device-apis/alexa-cooking.html
    /// </summary>
    public class CookingController : AlexaInterfaceControllerBase
    {
        /// <summary>
        /// User: Alexa, stop the microwave
        /// User: Alexa, defrost three pounds of meat in my microwave
        /// </summary>
        /// <returns></returns>
        public Task<object> HandleCooking(SmartHomeRequest request, ILambdaContext context)
        {
            // SetCookingMode
            throw new NotImplementedException();

            // CookingErrorResponse
        }
    }
}