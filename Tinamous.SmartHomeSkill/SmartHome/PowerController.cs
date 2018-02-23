using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Models.PropertyModels;
using Tinamous.SmartHome.Tinamous;
using Tinamous.SmartHome.Tinamous.Dtos;
using Tinamous.SmartHome.Tinamous.Interfaces;

namespace Tinamous.SmartHome.SmartHome
{
    /// <summary>
    /// Handled Alexa Power Control 
    /// </summary>
    /// <see cref="https://developer.amazon.com/docs/device-apis/alexa-powercontroller.html"/>
    public class PowerController : AlexaSmartHomeInterfaceControllerBase
    {
        public PowerController(IDevicesClient devicesClient, IMeasurementsClient measurementsClient, IStatusClient statusClient)
            : base(devicesClient, measurementsClient, statusClient)
        { }

        public override Task<object> HandleAlexaRequest(SmartHomeRequest request, ILambdaContext context)
        {
            switch (request.Directive.Header.Name)
            {
                case "TurnOn":
                    return HandleTurnOn(request, context);
                case "TurnOff":
                    return HandleTurnOff(request, context);
                default:
                    return NotSupportedDirective(request.Directive);
            }
        }

        /// <summary>
        /// Create the properties for the StateReport
        /// </summary>
        /// <param name="token"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public override async Task<List<Property>> CreateProperties(string token, DeviceDto device, string port)
        {
            LambdaLogger.Log("Power StateReport");

            List<Property> properties = new List<Property>();

            FieldValueDto value = await GetFieldValue(token, device, "powertState", port);

            if (value != null)
            {
                string stringValue = GetOnOff(value);

                var temperatureProperty = new StringValueProperty
                {
                    Namespace = "Alexa.PowerController",
                    Name = "powerState",
                    Value = stringValue,
                    TimeOfSample = DateTime.UtcNow,
                    UncertaintyInMilliseconds = 600
                };
                properties.Add(temperatureProperty);
            }

            return properties;
        }

        private async Task<object> HandleTurnOn(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Turn On");
            string token = request.Directive.Endpoint.Scope.Token;

            // Bit for debugging for a second...

            if (request.Directive.Endpoint.Cookie != null)
            {
                LambdaLogger.Log("Endpoint cookie set");
                LambdaLogger.Log("Endpoint cookie Username: " + request.Directive.Endpoint.Cookie.Username);
            }
            else
            {
                LambdaLogger.Log("Endpoint cookie was null");
            }

            await SendDeviceStatusMessage(request, token, "Turn On");

            // TODO: Can (shouldn't) these come from the state report

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

        private async Task<object> HandleTurnOff(SmartHomeRequest request, ILambdaContext context)
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


        private static string GetOnOff(FieldValueDto value)
        {
            if (value.bv.HasValue)
            {
                return value.bv.Value ? "ON" : "OFF";
            }

            if (string.IsNullOrWhiteSpace(value.sv))
            {
                return value.sv;
            }

            if (value.v.HasValue)
            {
                return value.v.Value > 0 ? "ON" : "OFF";
            }

            return null;
        }
    }
}