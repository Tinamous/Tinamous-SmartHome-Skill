using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Tinamous.Extension;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Models.PropertyModels;
using Tinamous.SmartHome.Tinamous;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.SmartHome
{
    public class StateReportHelper
    {
        private static List<string> _supportedInterfaces = new List<string>
        {
            "Alexa.TemperatureSensor",
            //"Alexa.PercentageController",
            //"Alexa.BrightnessController",
            //"/Alexa.ColorController",
            //"Alexa.PowerController",
            //"Alexa.PowerLevelController",
            //"Alexa.LockController",
            //"Alexa.SceneController",
            //"Alexa.ThermostatController"
        };

        public async Task<StateReportResponse> HandleAlexaRequest(SmartHomeRequest request, ILambdaContext context)
        {
            // Not available for ReportState
            if (request.Directive.Endpoint == null)
            {
                throw new NullReferenceException("Directive.Endpoint is null");
            }

            if (request.Directive.Endpoint.Scope != null)
            {
                //LambdaLogger.Log("Token Name: " + request.Directive.Endpoint.Scope.Token);
            }

            switch (request.Directive.Header.Name)
            {
                case "ReportState":
                    LambdaLogger.Log("Alexa ReportState request for " + request.Directive.Endpoint.EndpointId);
                    return await BuildDeviceStateReport(request.Directive);
                default:
                    LambdaLogger.Log("Unknown header name:" + request.Directive.Header.Name);
                    throw new NotSupportedException("Not supported directive name: " + request.Directive.Header.Name);
            }
        }

        private async Task<StateReportResponse> BuildDeviceStateReport(Directive requestDirective)
        {
            string token = requestDirective.Endpoint.Scope.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Missing token");
            }

            // See https://github.com/alexa/alexa-smarthome/blob/master/sample_messages/StateReport/StateReport.json
            var reponse = new StateReportResponse
            {
                Context = new Context
                {
                    Properties = new List<Property>(),
                },
                Event = ConstructReponseEvent(requestDirective, "StateReport"),
            };

            DevicesClient devicesClient = new DevicesClient();
            var device = await devicesClient.GetDeviceAsync(token, requestDirective.Endpoint.EndpointId);

            if (device == null)
            {
                return reponse;
            }

            foreach (var supportedInterface in _supportedInterfaces)
            {
                if (device.Tags.Contains(supportedInterface))
                {
                    List<Property> properties = await CreateInterfaceProperties(token, device, supportedInterface);
                    reponse.Context.Properties.AddRange(properties);
                }
            }

            return reponse;
        }

        private Task<List<Property>> CreateInterfaceProperties(string token, DeviceDto device, string supportedInterface)
        {
            switch (supportedInterface)
            {
                case "Alexa.TemperatureSensor":
                    return CreateTemperatureSensorProperties(supportedInterface, token, device);
                //case "Alexa.PercentageController":
                case "Alexa.BrightnessController":
                    return CreateBrightnessProperties(supportedInterface, token, device);
                //case "Alexa.ColorController":
                //case "Alexa.PowerController":
                //case "Alexa.PowerLevelController":
                //case "Alexa.LockController":
                //case "Alexa.SceneController":
                //case "Alexa.ThermostatController":
                default:
                    return Task.FromResult(new List<Property>());
            }
        }

        private async Task<List<Property>> CreateTemperatureSensorProperties(string supportedInterface, string token, DeviceDto device)
        {
            LambdaLogger.Log("Temperature StateReport");

            List<Property> properties = new List<Property>();

            FieldValueDto value = await GetFieldValue(token, device, "temperature");

            if (value != null)
            {
                var temperatureProperty = new NumericValueWithUnitsProperty
                {
                    Namespace = supportedInterface,
                    Name = "temperature",
                    Value = new NumericPropertyValue
                    {
                        // TODO: Support °F
                        Scale = "CELSIUS",
                        Value = value.v,
                    },
                    TimeOfSample = DateTime.UtcNow,
                    UncertaintyInMilliseconds = 600
                };
                properties.Add(temperatureProperty);
            }

            return properties;
        }

        private async Task<List<Property>> CreateBrightnessProperties(string supportedInterface, string token, DeviceDto device)
        {
            LambdaLogger.Log("Brightness StateReport");

            List<Property> properties = new List<Property>();

            FieldValueDto value = await GetFieldValue(token, device, "brightness");

            if (value != null)
            {
                var temperatureProperty = new IntValueProperty
                {
                    Namespace = supportedInterface,
                    Name = "brightness",
                    Value = Convert.ToInt32(value.v),
                    TimeOfSample = DateTime.UtcNow,
                    UncertaintyInMilliseconds = 600
                };
                properties.Add(temperatureProperty);
            }

            return properties;
        }

        private static async Task<FieldValueDto> GetFieldValue(string token, DeviceDto device, string fieldName)
        {
            FieldDescriptorDto field = device.GetField(fieldName);
            if (field == null)
            {
                LambdaLogger.Log("Did not find required field '" + fieldName + "' for device: " + device.DisplayName);
                return null;
            }

            LambdaLogger.Log("Getting field '" + fieldName + "' value from device: " + device.DisplayName);
            MeasurementsClient measurementsClient = new MeasurementsClient();
            var value = await measurementsClient.GetFieldValueAsync(token, device.Id, field);
            if (value == null)
            {
                LambdaLogger.Log("Did not find field '" + fieldName + "' value for device: " + device.DisplayName);
                return null;
            }

            return value;
        }

        private Event ConstructReponseEvent(Directive directive, string name)
        {
            return new Event
            {
                Header = new Header
                {
                    MessageId = directive.Header.MessageId,
                    CorrelationToken = directive.Header.CorrelationToken,
                    Namespace = "Alexa",
                    Name = name,
                    PayloadVersion = "3",
                },
                Endpoint = new StateReportEndpoint
                {
                    EndpointId =directive.Endpoint.EndpointId,
                    Scope = new Scope
                    {
                        Type = directive.Endpoint.Scope.Type,
                        Token = directive.Endpoint.Scope.Token,
                    }
                },
            };
        }
    }
}