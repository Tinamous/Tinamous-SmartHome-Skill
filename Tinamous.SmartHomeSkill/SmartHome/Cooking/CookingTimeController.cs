using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Models.PropertyModels;
using Tinamous.SmartHome.Tinamous;
using Tinamous.SmartHome.Tinamous.Dtos;
using Tinamous.SmartHome.Tinamous.Interfaces;

namespace Tinamous.SmartHome.SmartHome.Cooking
{
    /// <summary>
    /// User: Alexa, 2 minutes on the microwave
    /// https://developer.amazon.com/docs/device-apis/alexa-cooking-timecontroller.html
    /// </summary>
    public class CookingTimeController : AlexaSmartHomeInterfaceControllerBase
    {
        public CookingTimeController(IDevicesClient devicesClient, IMeasurementsClient measurementsClient, IStatusClient statusClient) 
            : base(devicesClient, measurementsClient, statusClient)
        { }

        public override Task<object> HandleAlexaRequest(SmartHomeRequest request, ILambdaContext context)
        {
            // CookByTime
            // AdjustCookTime
            
            // CookingErrorResponse
            switch (request.Directive.Header.Name)
            {
                case "CookByTime":
                    return HandleCookByTime(request, context);
                case "AdjustCookTime":
                    return HandleAdjustCookTime(request, context);
                default:
                    return NotSupportedDirective(request.Directive);
            }
        }

        public override Task<List<Property>> CreateProperties(string token, DeviceDto device, string port)
        {
            throw new NotImplementedException();
        }

        private async Task<object> HandleCookByTime(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Cook by time");
            string token = request.Directive.Endpoint.Scope.Token;

            string message = string.Format("Cook by time {0}", request.Directive.Payload.CookTime);
            await SendDeviceStatusMessage(request, token, message);

            return CreateResponse(request);
        }

        private async Task<object> HandleAdjustCookTime(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Adjust cook time");
            string token = request.Directive.Endpoint.Scope.Token;

            string message = string.Format("Adjust cook time {0}", request.Directive.Payload.CookTime);
            await SendDeviceStatusMessage(request, token, message);

            return CreateResponse(request);
        }

        private object CreateResponse(SmartHomeRequest request)
        {
            // Assume it worked...
            var completionTime = DateTime.UtcNow.AddMinutes(3);
            return new CookTimeControlResponse
            {
                Context = new Context
                {
                    Properties = new List<Property>
                    {
                        new StringValueProperty
                        {
                            Namespace = "Alexa.Cooking",
                            Name = "cookingMode",
                            Value = "TIMECOOK",
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        },
                        new DateTimeValueProperty
                        {
                            Namespace = "Alexa.Cooking",
                            Name = "cookCompletionTime",
                            Value = completionTime, // TODO: Get the actual value back!
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        },
                        new BooleanValueProperty
                        {
                            Namespace = "Alexa.Cooking",
                            Name = "isCookCompletionTimeEstimated",
                            Value = false, // TODO: Get the actual value back!
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        },
                        new BooleanValueProperty
                        {
                            Namespace = "Alexa.Cooking",
                            Name = "isHolding",
                            Value = false, // TODO: Get the actual value back!
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        },
                        new ValueValueProperty
                        {
                            Namespace = "Alexa.BrightnessController",
                            Name = "connectivity",
                            Value = new ValuePropertyValue {Value = "ON"},
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        },
                    },
                },
                Event = ConstructReponseEvent(request.Directive, "Response"),
            };
        }
    }
}