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
    /// https://developer.amazon.com/docs/device-apis/alexa-powerlevelcontroller.html
    /// </summary>
    public class PowerLevelController : AlexaSmartHomeInterfaceControllerBase
    {
        private const string InterfaceNamespace = "Alexa.PowerLevelController";
        public PowerLevelController(IDevicesClient devicesClient, IMeasurementsClient measurementsClient, IStatusClient statusClient) 
            : base(devicesClient, measurementsClient, statusClient)
        { }

        public override Task<object> HandleAlexaRequest(SmartHomeRequest request, ILambdaContext context)
        {
            switch (request.Directive.Header.Name)
            {
                case "SetPowerLevel":
                    return HandleSetPowerLevel(request, context);
                case "AdjustPowerLevel":
                    return HandleAdjustPowerLevel(request, context);
                default:
                    return NotSupportedDirective(request.Directive);
            }
        }

        public override async Task<List<Property>> CreateProperties(string token, DeviceDto device, string port)
        {
            LambdaLogger.Log("Percentage PowerLevel");

            List<Property> properties = new List<Property>();

            FieldValueDto value = await GetFieldValue(token, device, "powerLevel", port);

            if (value != null && value.v.HasValue)
            {
                var temperatureProperty = new FloatValueProperty
                {
                    Namespace = InterfaceNamespace,
                    Name = "powerLevel",
                    Value = value.v.Value,
                    TimeOfSample = DateTime.UtcNow,
                    UncertaintyInMilliseconds = 600
                };
                properties.Add(temperatureProperty);
            }

            return properties;
        }

        private async Task<object> HandleSetPowerLevel(SmartHomeRequest request, ILambdaContext context)
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
                            Namespace = InterfaceNamespace,
                            Name = "powerLevel",
                            Value = level, // TODO: Get the actual value back!
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        }
                    },
                },
                Event = ConstructReponseEvent(request.Directive, "Response"),
            };
        }

        ///
        private async Task<object> HandleAdjustPowerLevel(SmartHomeRequest request, ILambdaContext context)
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
                            Namespace = InterfaceNamespace,
                            Name = "powerLevel",
                            Value = 50, // TODO: Get the actual value back!
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        }
                    },
                },
                Event = ConstructReponseEvent(request.Directive, "Response"),
            };
        }
    }
}