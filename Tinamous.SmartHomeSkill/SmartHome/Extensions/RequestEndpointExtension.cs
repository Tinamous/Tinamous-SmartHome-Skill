using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.SmartHome.Models;

namespace Tinamous.SmartHome.SmartHome.Extensions
{
    public static class RequestEndpointExtension
    {
        public static DeviceAndPort GetDeviceAndPort(this RequestEndpoint requestEndpoint)
        {
            return new DeviceAndPort(requestEndpoint.EndpointId);
        }
    }
}