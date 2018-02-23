using System.Collections.Generic;
using System.Threading.Tasks;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.Tinamous.Interfaces
{
    public interface IDevicesClient
    {
        Task<List<DeviceDto>> GetDevicesAsync(string authToken);
        Task<DeviceDto> GetDeviceAsync(string authToken, string id);
    }
}