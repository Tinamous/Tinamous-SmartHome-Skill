using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Models.PropertyModels;
using Tinamous.SmartHome.SmartHome.Models;
using Tinamous.SmartHome.Tinamous;
using Tinamous.SmartHome.Tinamous.Dtos;
using Tinamous.SmartHome.Tinamous.Interfaces;

namespace Tinamous.SmartHome.SmartHome
{
    /// <summary>
    /// See https://developer.amazon.com/docs/device-apis/alexa-brightnesscontroller.html
    /// </summary>
    public class BrightnessController : AlexaSmartHomeInterfaceControllerBase
    {
        private const string InterfaceNamespace = "Alexa.BrightnessController";

        public BrightnessController(IDevicesClient devicesClient, IMeasurementsClient measurementsClient, IStatusClient statusClient)
            : base(devicesClient, measurementsClient, statusClient)
        { }

        // / AdjustBrightness, SetBrightness, 
        public override Task<object> HandleAlexaRequest(SmartHomeRequest request, ILambdaContext context)
        {
            switch (request.Directive.Header.Name)
            {
                case "SetBrightness":
                    return HandleSetBrightness(request, context);
                case "AdjustBrightness":
                    return HandleAdjustBrightness(request, context);
                default:
                    return NotSupportedDirective(request.Directive);
            }
        }


        /// <summary>
        /// Create the properties for the StateReport
        /// </summary>
        /// <param name="token"></param>
        /// <param name="device"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public override async Task<List<Property>> CreateProperties(string token, DeviceDto device, string port)
        {
            LambdaLogger.Log("Brightness StateReport");

            List<Property> properties = new List<Property>();

            FieldValueDto value = await GetFieldValue(token, device, "brightness", port);

            if (value != null && value.v.HasValue)
            {
                var temperatureProperty = new IntValueProperty
                {
                    Namespace = InterfaceNamespace,
                    Name = "brightness",
                    Value = Convert.ToInt32(value.v.Value),
                    TimeOfSample = DateTime.UtcNow,
                    UncertaintyInMilliseconds = 600
                };
                properties.Add(temperatureProperty);
            }

            return properties;
        }

        private async Task<object> HandleSetBrightness(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Set brightness");
            string token = request.Directive.Endpoint.Scope.Token;

            var level = request.Directive.Payload.Brightness;

            string message = string.Format("Set brightness {0}", level);
            await SendDeviceStatusMessage(request, token, message);

            // Assume it worked...
            return new BrightnessControlResponse
            {
                Context = new Context
                {
                    Properties = new List<Property>
                    {
                        new IntValueProperty
                        {
                            Namespace = "Alexa.BrightnessController",
                            Name = "brightness",
                            Value = level, // TODO: Get the actual value back!
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        }
                    },
                },
                Event = ConstructReponseEvent(request.Directive, "Response"),
            };
        }

        private async Task<object> HandleAdjustBrightness(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Adjust brightness");
            string token = request.Directive.Endpoint.Scope.Token;

            var level = request.Directive.Payload.PowerLevelDelta;

            string message = string.Format("Adjust brightness {0}", level);
            await SendDeviceStatusMessage(request, token, message);

            // Give the device some time to update.
            // or can we just return empty properties?
            Thread.Sleep(2000);

            return await CreateResponse(request, token);
        }

        private async Task<object> CreateResponse(SmartHomeRequest request, string token)
        {
            var deviceAndPort = new DeviceAndPort(request.Directive.Endpoint.EndpointId);
            var device = await GetDevice(token, deviceAndPort);

            var properties = await CreateProperties(token, device, deviceAndPort.Port);

            return new BrightnessControlResponse
            {
                Context = new Context
                {
                    Properties = properties,
                },
                Event = ConstructReponseEvent(request.Directive, "Response"),
            };
        }
    }
}