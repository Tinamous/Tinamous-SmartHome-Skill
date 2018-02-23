using System;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.SmartHome.Models
{
    public class DeviceAndPort
    {
        public DeviceAndPort(string endpointId)
        {
            string[] idAndPort = endpointId.Split("#", StringSplitOptions.RemoveEmptyEntries);
            Id = idAndPort[0];
            
            if (idAndPort.Length > 1)
            {
                Port = idAndPort[1];
            }
        }

        public DeviceAndPort(DeviceDto device, int? portId = null)
        {
            Id = device.Id;

            if (portId.HasValue)
            {
                // Port names are 1 based.
                Port = string.Format("port-{0}", portId);
            }
        }

        public string Id { get; private set; }
        public string Port { get; private set; } = "";

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Port))
            {
                return string.Format("{0}#{1}#", Id, Port);
            }
            return Id;
        }
    }
}