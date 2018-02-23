using System;
using System.Collections.Generic;
using System.Linq;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.Tinamous.Extension
{
    public static class DeviceDtoExtension
    {
        public static bool HasField(this DeviceDto device, string fieldNameOrTag)
        {
            if (device.FieldDescriptors == null)
            {
                return false;
            }

            return device.GetField(fieldNameOrTag) != null;
        }

        public static FieldDescriptorDto GetField(this DeviceDto device, string fieldNameOrTag)
        {
            if (device.FieldDescriptors == null)
            {
                return null;
            }

            foreach (var deviceFieldDescriptor in device.FieldDescriptors)
            {
                // Check tags.
                deviceFieldDescriptor.Tags = deviceFieldDescriptor.Tags ?? new List<string>();
                if (deviceFieldDescriptor.Tags.Any(tag => fieldNameOrTag.Equals(tag, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return deviceFieldDescriptor;
                }

                // Check field name
                if (fieldNameOrTag.Equals(deviceFieldDescriptor.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return deviceFieldDescriptor;
                }

                // Check field label
                if (fieldNameOrTag.Equals(deviceFieldDescriptor.Label, StringComparison.InvariantCultureIgnoreCase))
                {
                    return deviceFieldDescriptor;
                }
            }

            return null;
        }
    }
}