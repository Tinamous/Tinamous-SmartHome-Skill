using System;
using System.Collections.Generic;
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
    public class StateReportHelper : IAlexaSmartHomeController
    {
        private readonly IDevicesClient _devicesClient;

        /// <summary>
        /// Interfaces that are supported by this skill at presnet.
        /// </summary>
        private static readonly List<string> SupportedInterfaces = new List<string>
        {
            "Alexa.TemperatureSensor",
            "Alexa.PercentageController",
            "Alexa.BrightnessController",
            "Alexa.ColorController",
            "Alexa.PowerController",
            "Alexa.PowerLevelController",
            // Not implemented...
            //"Alexa.LockController",
            //"Alexa.SceneController",
            //"Alexa.ThermostatController"
        };

        public StateReportHelper(IDevicesClient devicesClient)
        {
            _devicesClient = devicesClient;
        }

        public async Task<object> HandleAlexaRequest(SmartHomeRequest request, ILambdaContext context)
        {
            // Not available for ReportState
            if (request.Directive.Endpoint == null)
            {
                throw new NullReferenceException("Directive.Endpoint is null");
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

        public Task<List<Property>> CreateProperties(string token, DeviceDto device, string port)
        {
            throw new NotSupportedException();
        }

        public Task<List<Property>> CreateProperties(string supportedInterface, string token, DeviceDto device, string port)
        {
            IAlexaSmartHomeController controller = ControllerFactory.GetController(supportedInterface, new RestClient());
            return controller.CreateProperties(token, device, port);
        }

        /// <summary>
        ///  See https://github.com/alexa/alexa-smarthome/blob/master/sample_messages/StateReport/StateReport.json
        /// </summary>
        /// <param name="requestDirective"></param>
        /// <returns></returns>
        private async Task<StateReportResponse> BuildDeviceStateReport(Directive requestDirective)
        {
            string token = requestDirective.Endpoint.Scope.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Missing token");
            }

            var reponse = new StateReportResponse
            {
                Context = new Context
                {
                    Properties = new List<Property>(),
                },
                Event = ConstructReponseEvent(requestDirective, "StateReport"),
            };

            var deviceAndPort = new DeviceAndPort(requestDirective.Endpoint.EndpointId);
            var device = await _devicesClient.GetDeviceAsync(token, deviceAndPort.Id);

            if (device == null)
            {
                return reponse;
            }

            foreach (var supportedInterface in SupportedInterfaces)
            {
                if (device.Tags.Contains(supportedInterface))
                {
                    // This will itterate through the appropriate controllers
                    // and get them to create the required properties for the interface
                    // they support.
                    List<Property> properties = await CreateProperties(supportedInterface, token, device, deviceAndPort.Port);
                    reponse.Context.Properties.AddRange(properties);
                }
            }

            return reponse;
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