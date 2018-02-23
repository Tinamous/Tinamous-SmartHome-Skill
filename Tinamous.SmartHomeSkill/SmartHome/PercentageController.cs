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
    /// Alexa, increase device name by number percent
    /// 
    /// See: https://developer.amazon.com/docs/device-apis/alexa-percentagecontroller.html
    /// </summary>
    public class PercentageController : AlexaSmartHomeInterfaceControllerBase
    {
        private const string InterfaceNamespace = "Alexa.PercentageController";

        public PercentageController(IDevicesClient devicesClient, IMeasurementsClient measurementsClient, IStatusClient statusClient) 
            : base(devicesClient, measurementsClient, statusClient)
        { }

        public override Task<object> HandleAlexaRequest(SmartHomeRequest request, ILambdaContext context)
        {
            switch (request.Directive.Header.Name)
            {
                case "SetPercentage":
                    return HandleSetPercentage(request, context);
                case "AdjustPercentage":
                    return AdjustPercentage(request, context);
                default:
                    return NotSupportedDirective(request.Directive);
            }
        }

        public override async Task<List<Property>> CreateProperties(string token, DeviceDto device, string port)
        {
            LambdaLogger.Log("Percentage StateReport");

            List<Property> properties = new List<Property>();

            FieldValueDto value = await GetFieldValue(token, device, "percentage", port);

            if (value != null && value.v.HasValue)
            {
                var temperatureProperty = new FloatValueProperty
                {
                    Namespace = InterfaceNamespace,
                    Name = "percentage",
                    Value = value.v.Value,
                    TimeOfSample = DateTime.UtcNow,
                    UncertaintyInMilliseconds = 600
                };
                properties.Add(temperatureProperty);
            }

            return properties;
        }

        /// <summary>
        /// Alexa, set name to number percen
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<object> HandleSetPercentage(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Set percentage");
            string token = request.Directive.Endpoint.Scope.Token;

            var level = request.Directive.Payload.Percentage;

            string message = string.Format("Set percentage {0}", level);
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
                            Name = "percentage",
                            Value = level, // TODO: Get the actual value back!
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        }
                    },
                },
                Event = ConstructReponseEvent(request.Directive, "Response"),
            };
        }

        private async Task<object> AdjustPercentage(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Adjust Percentage");
            string token = request.Directive.Endpoint.Scope.Token;

            var delta = request.Directive.Payload.PercentageDelta;

            string message = string.Format("Adjust percentage {0}", delta);
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
                            Name = "percentage",
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