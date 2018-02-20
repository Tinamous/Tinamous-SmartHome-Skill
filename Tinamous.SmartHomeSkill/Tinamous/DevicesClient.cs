using System.Collections.Generic;
using System.Threading.Tasks;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.Tinamous
{
    public class DevicesClient : RestClientBase
    {
        private const string BaseUri = "api/v1/Devices/{0}";

        /// <summary>
        /// Get a list of the devices that are tagged with "Alexa.SmartDevice"
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public async Task<List<DeviceDto>> GetDevicesAsync(string authToken)
        {
            var uri = string.Format(BaseUri, "");
            List<DeviceDto> devices = await GetAsJsonAsync<List<DeviceDto>>(authToken, uri);

            List<DeviceDto> filteredDevices = new List<DeviceDto>();

            foreach (var deviceDto in devices)
            {
                if (deviceDto.Tags != null)
                {
                    if (deviceDto.Tags.Contains("Alexa.SmartDevice"))
                    {
                        filteredDevices.Add(deviceDto);
                    }
                }
            }

            return filteredDevices;
        }


        /// <summary>
        /// Get a list of the devices that are tagged with "Alexa.SmartDevice"
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public Task<DeviceDto> GetDeviceAsync(string authToken, string id)
        {
            // Handle Multi-port device.
            if (id.EndsWith("*"))
            {
                // Remove the port identifier.
                id = id.Replace("-port1*", "");
                id = id.Replace("-port2*", "");
                id = id.Replace("-port3*", "");
                id = id.Replace("-port4*", "");
            }

            var uri = string.Format(BaseUri, id);
            return GetAsJsonAsync<DeviceDto>(authToken, uri);
        }

    }
}