using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Tinamous.Extension;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Tinamous;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.SmartHome
{
    public abstract class AlexaInterfaceControllerBase
    {
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

        protected static async Task SendDeviceStatusMessage(SmartHomeRequest request, string token, string messageFragment)
        {
            // Get the device we are targetting.
            string deviceId = request.Directive.Endpoint.EndpointId;
            DevicesClient devicesClient = new DevicesClient();
            DeviceDto device = await devicesClient.GetDeviceAsync(token, deviceId);

            // Ideall we would just send a named command "Turn On"
            // once the commands interface in inplace......

            // For now, use a status message.
            StatusClient statusClient = new StatusClient();
            string message = string.Format("@{0} {1}", device.UserName, messageFragment);
            await statusClient.PostStatusMessageAsync(token, message);
        }

        protected async Task<FieldValueDto> GetFieldValue(SmartHomeRequest request, string token, string field)
        {
            DevicesClient devicesClient = new DevicesClient();
            DeviceDto device = await devicesClient.GetDeviceAsync(token, request.Directive.Endpoint.EndpointId);
            return await GetFieldValue(token, device, field);
        }

        private async Task<FieldValueDto> GetFieldValue(string token, DeviceDto device, string fieldName)
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
    }
}