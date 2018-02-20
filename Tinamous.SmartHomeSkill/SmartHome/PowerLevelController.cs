using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Models.PropertyModels;

namespace Tinamous.SmartHome.SmartHome
{
    /// <summary>
    /// https://developer.amazon.com/docs/device-apis/alexa-brightnesscontroller.html
    /// </summary>
    public class PowerLevelController : AlexaInterfaceControllerBase
    {
        public Task<PowerControlResponse> HandleLevelPowerControl(SmartHomeRequest request, ILambdaContext context)
        {
            switch (request.Directive.Header.Name)
            {
                case "SetPowerLevel":
                    return HandleSetPowerLevel(request, context);
                case "AdjustPowerLevel":
                    return HandleAdjustPowerLevel(request, context);
                default:
                    return null;
            }
        }

        private async Task<PowerControlResponse> HandleSetPowerLevel(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Set powerlevel");
            string token = request.Directive.Endpoint.Scope.Token;

            var level = request.Directive.Payload.PowerLevel;

            string message = string.Format("Set powerlevel {0}", level);
            await SendDeviceStatusMessage(request, token, message);

            // Assume it worked...
            return new PowerControlResponse
            {
                Context = new Context
                {
                    Properties = new List<Property>
                    {
                        new IntValueProperty
                        {
                            Namespace = "Alexa.PowerLevelController",
                            Name = "powerLevel",
                            Value = level, // TODO: Get the actual value back!
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        },
                        new ValueValueProperty
                        {
                            Namespace = "Alexa.EndpointHealth",
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

        ///
        private async Task<PowerControlResponse> HandleAdjustPowerLevel(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Adjust powerlevel");
            string token = request.Directive.Endpoint.Scope.Token;

            var level = request.Directive.Payload.PowerLevelDelta;

            string message = string.Format("Adjust powerlevel {0}", level);
            await SendDeviceStatusMessage(request, token, message);

            // Assume it worked...
            return new PowerControlResponse
            {
                Context = new Context
                {
                    Properties = new List<Property>
                    {
                        new IntValueProperty
                        {
                            Namespace = "Alexa.PowerLevelController",
                            Name = "powerLevel",
                            Value = 50, // TODO: Get the actual value back!
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        },
                        new ValueValueProperty
                        {
                            Namespace = "Alexa.EndpointHealth",
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