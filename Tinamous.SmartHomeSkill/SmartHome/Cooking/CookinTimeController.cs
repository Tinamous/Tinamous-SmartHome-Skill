using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;

namespace Tinamous.SmartHome.SmartHome.Cooking
{
    /// <summary>
    /// User: Alexa, 2 minutes on the microwave
    /// https://developer.amazon.com/docs/device-apis/alexa-cooking-timecontroller.html
    /// </summary>
    public class CookinTimeController : AlexaInterfaceControllerBase
    {
        public Task<object> HandleCookingTimeController(SmartHomeRequest request, ILambdaContext context)
        {
            // CookByTime
            // AdjustCookTime
            throw new NotImplementedException();
            // CookingErrorResponse
            switch (request.Directive.Header.Name)
            {
                case "CookByTime":
                    return HandleCookByTime(request, context);
                case "AdjustCookTime":
                    return HandleAdjustCookTime(request, context);
                default:
                    return null;
            }
        }

        private async Task<object> HandleCookByTime(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Cook by time");
            string token = request.Directive.Endpoint.Scope.Token;

            await SendDeviceStatusMessage(request, token, "Cook by time...");
            throw new NotImplementedException();
        }

        private async Task<object> HandleAdjustCookTime(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Adjust cook time");
            string token = request.Directive.Endpoint.Scope.Token;

            await SendDeviceStatusMessage(request, token, "Adjust cook time...");
            throw new NotImplementedException();
        }        
    }
}