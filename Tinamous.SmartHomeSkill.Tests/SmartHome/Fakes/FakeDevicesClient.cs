using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tinamous.SmartHome.Tinamous.Dtos;
using Tinamous.SmartHome.Tinamous.Interfaces;

namespace Tinamous.SmartHome.Tests.SmartHome.Fakes
{
    public class FakeDevicesClient : IDevicesClient
    {
        private List<DeviceDto> _devices;

        public FakeDevicesClient(List<DeviceDto> devices)
        {
            _devices = devices;
        }

        public Task<List<DeviceDto>> GetDevicesAsync(string authToken)
        {
            return Task.FromResult(_devices);
        }

        public Task<DeviceDto> GetDeviceAsync(string authToken, string id)
        {
            if (id == "Endpoint1")
            {
                return Task.FromResult(new DeviceDto
                {
                    Id = "Endpoint1",
                    DisplayName = "USB Switch",
                    UserName = "Switch"
                });
            }
            throw new NotSupportedException();
        }
    }
}