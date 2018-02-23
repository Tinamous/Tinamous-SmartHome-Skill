using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Tinamous.Extension;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Models.Payloads;
using Tinamous.SmartHome.Models.PropertyModels;
using Tinamous.SmartHome.SmartHome.Extensions;
using Tinamous.SmartHome.SmartHome.Models;
using Tinamous.SmartHome.Tinamous;
using Tinamous.SmartHome.Tinamous.Dtos;
using Tinamous.SmartHome.Tinamous.Interfaces;

namespace Tinamous.SmartHome.SmartHome
{
    public abstract class AlexaSmartHomeInterfaceControllerBase : IAlexaSmartHomeController
    {
        private readonly IDevicesClient _devicesClient;
        private readonly IMeasurementsClient _measurementsClient;
        private readonly IStatusClient _statusClient;

        public AlexaSmartHomeInterfaceControllerBase(IDevicesClient devicesClient, IMeasurementsClient measurementsClient, IStatusClient statusClient)
        {
            _devicesClient = devicesClient;
            _measurementsClient = measurementsClient;
            _statusClient = statusClient;
        }

        public abstract Task<object> HandleAlexaRequest(SmartHomeRequest request, ILambdaContext context);

        public abstract Task<List<Property>> CreateProperties(string token, DeviceDto device, string port);

        #region Helpers

        protected Event ConstructReponseEvent(Directive directive, string name)
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
                    EndpointId = directive.Endpoint.EndpointId,
                    Scope = new Scope
                    {
                        Type = directive.Endpoint.Scope.Type,
                        Token = directive.Endpoint.Scope.Token,
                    }
                },
            };
        }

        protected Task<object> NotSupportedDirective(Directive directive)
        {
            var responseEvent = ConstructReponseEvent(directive, "ErrorResponse");
            responseEvent.Endpoint = new StateReportEndpoint();

            if (directive.Endpoint != null)
            {
                responseEvent.Endpoint.EndpointId = directive.Endpoint.EndpointId;
            }

            directive.Payload = new ErrorPayload
            {
                Type = "INVALID_DIRECTIVE",
                Message = "Directive not supported or implemented."
            };

            return Task.FromResult((object)new ErrorResponse
            {
                Event = responseEvent
            });
        }

        protected async Task SendDeviceStatusMessage(SmartHomeRequest request, string token, string messageFragment)
        {
            // Get the device we are targetting.
            DeviceAndPort deviceAndPort = request.Directive.Endpoint.GetDeviceAndPort();
            //DeviceDto device = await _devicesClient.GetDeviceAsync(token, deviceAndPort.Id);

            string username = request.Directive.Endpoint.Cookie.Username;

            // Ideall we would just send a named command "Turn On"
            // once the commands interface in inplace......

            // For now, use a status message.
            string message = string.Format("@{0} {1} {2}", username, messageFragment, deviceAndPort.Port);
            message = message.Trim();
            await _statusClient.PostStatusMessageAsync(token, message);
        }

        protected Task<DeviceDto> GetDevice(string token, DeviceAndPort deviceAndPort)
        {
            return _devicesClient.GetDeviceAsync(token, deviceAndPort.Id);
        }

        private FieldDescriptorDto GetFieldDescriptor(DeviceDto device, string fieldName, string port)
        {
            if (string.IsNullOrEmpty(port))
            {
                return device.GetField(fieldName);
            }

            // Port has a space in it.
            // e.g. powerState and powerState-port-1
            string fieldAndPortName = string.Format("{0}-{1}", fieldName, port);
            FieldDescriptorDto field = device.GetField(fieldAndPortName);

            if (field != null)
            {
                return field;
            }
            return GetFieldDescriptor(device, fieldName, null);
        }

        protected async Task<FieldValueDto> GetFieldValue(string token, DeviceDto device, string fieldName, string port)
        {
            FieldDescriptorDto field = GetFieldDescriptor(device, fieldName, port);
            if (field == null)
            {
                LambdaLogger.Log("Did not find required field '" + fieldName + "' for device: " + device.DisplayName);
                return null;
            }

            LambdaLogger.Log("Getting field '" + fieldName + "' value from device: " + device.DisplayName);
            var value = await _measurementsClient.GetFieldValueAsync(token, device.Id, field);
            if (value == null)
            {
                LambdaLogger.Log("Did not find field '" + fieldName + "' value for device: " + device.DisplayName);
                return null;
            }

            return value;
        }

        #endregion
    }
}