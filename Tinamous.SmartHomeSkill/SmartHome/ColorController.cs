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
    /// See: https://developer.amazon.com/docs/device-apis/alexa-colorcontroller.html
    /// </summary>
    public class ColorController : AlexaSmartHomeInterfaceControllerBase
    {
        private const string InterfaceNamespace = "Alexa.ColorController";

        public ColorController(IDevicesClient devicesClient, IMeasurementsClient measurementsClient, IStatusClient statusClient) 
            : base(devicesClient, measurementsClient, statusClient)
        { }

        public override Task<object> HandleAlexaRequest(SmartHomeRequest request, ILambdaContext context)
        {
            switch (request.Directive.Header.Name)
            {
                case "SetColor":
                    return HandleSetColor(request, context);
                default:
                    return NotSupportedDirective(request.Directive);
            }
        }

        public override async Task<List<Property>> CreateProperties(string token, DeviceDto device, string port)
        {
            LambdaLogger.Log("Get Color property");

            List<Property> properties = new List<Property>();

            FieldValueDto value = await GetFieldValue(token, device, "color", port);

            if (value == null || string.IsNullOrWhiteSpace(value.sv))
            {
                return properties;
            }

            string[] hsv = value.sv.Split(",", StringSplitOptions.RemoveEmptyEntries);

            if (hsv.Length == 3)
            {
                var temperatureProperty = new ColorValueProperty
                {
                    Namespace = InterfaceNamespace,
                    Name = "color",
                    Value = new HsvColor
                    {
                        Hue = Convert.ToSingle(hsv[0]),
                        Saturation = Convert.ToSingle(hsv[0]),
                        Brightness = Convert.ToSingle(hsv[0]),
                    },
                    TimeOfSample = DateTime.UtcNow,
                    UncertaintyInMilliseconds = 600
                };
                properties.Add(temperatureProperty);
            }

            return properties;
        }

        private async Task<object> HandleSetColor(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Set color");
            string token = request.Directive.Endpoint.Scope.Token;

            HsvColor color = request.Directive.Payload.Color;
            if (color == null)
            {
                LambdaLogger.Log("No color!");
                throw new NullReferenceException("Color");
            }
            LambdaLogger.Log("Got a color");

            string message = string.Format("Set color HSV {0},{1},{2}",
                color.Hue,
                color.Saturation,
                color.Brightness);

            await SendDeviceStatusMessage(request, token, message);

            // Assume it worked...
            return new ColorControlResponse
            {
                Context = new Context
                {
                    Properties = new List<Property>
                        {
                            new ColorValueProperty
                            {
                                Namespace = InterfaceNamespace,
                                Name = "color",
                                Value = color, // TODO: Get the actual value back!
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