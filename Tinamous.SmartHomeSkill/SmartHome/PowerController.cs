using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Models.PropertyModels;

namespace Tinamous.SmartHome.SmartHome
{
    /// <summary>
    /// Handled Alexa Power Control 
    /// </summary>
    /// <see cref="https://developer.amazon.com/docs/device-apis/alexa-powercontroller.html"/>
    public class PowerController : AlexaInterfaceControllerBase
    {
        public Task<PowerControlResponse> HandlePowerControl(SmartHomeRequest request, ILambdaContext context)
        {
            try
            {
                switch (request.Directive.Header.Name)
                {
                    case "TurnOn":
                        return HandleTurnOn(request, context);
                    case "TurnOff":
                        return HandleTurnOff(request, context);
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                LambdaLogger.Log("Exception in power control. " + ex.ToString());
                throw;
            }
        }

        private async Task<PowerControlResponse> HandleTurnOn(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Turn On");
            string token = request.Directive.Endpoint.Scope.Token;

            await SendDeviceStatusMessage(request, token, "Turn On");

            // When the power state changes, send a state report with a powerState property.
            // Assume it worked.......
            return new PowerControlResponse
            {
                Context = new Context
                {
                    Properties = new List<Property>
                    {
                        new StringValueProperty
                        {
                            Namespace = "Alexa.PowerController",
                            Name = "powerState",
                            Value = "ON",
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

        private async Task<PowerControlResponse> HandleTurnOff(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Turn Off");

            string token = request.Directive.Endpoint.Scope.Token;

            await SendDeviceStatusMessage(request, token, "Turn Off");

            return new PowerControlResponse
            {
                Context = new Context
                {
                    Properties = new List<Property>
                    {
                        new StringValueProperty
                        {
                            Namespace = "Alexa.PowerController",
                            Name = "powerState",
                            Value = "OFF",
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