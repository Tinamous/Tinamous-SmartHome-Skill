using NUnit.Framework;
using Tinamous.SmartHome.SmartHome.Models;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.Tests.SmartHome.Models
{
    public class DeviceAndPortTest
    {
        [Test]
        public void DeviceWithoutPort_HasExpectedProperties()
        {
            // Arrange
            DeviceDto device = new DeviceDto {Id = "C6CBE649-16BD-4A20-A0DB-941E2296818C"};

            // Act
            DeviceAndPort deviceAndPort = new DeviceAndPort(device);

            // Assert
            Assert.AreEqual("C6CBE649-16BD-4A20-A0DB-941E2296818C", deviceAndPort.Id);
            Assert.AreEqual("", deviceAndPort.Port);
            Assert.AreEqual("C6CBE649-16BD-4A20-A0DB-941E2296818C", deviceAndPort.ToString());
        }

        [Test]
        public void DeviceWithPort_HasExpectedId()
        {
            // Arrange
            DeviceDto device = new DeviceDto { Id = "C6CBE649-16BD-4A20-A0DB-941E2296818C" };

            // Act
            DeviceAndPort deviceAndPort = new DeviceAndPort(device, 2);

            // Assert
            Assert.AreEqual("C6CBE649-16BD-4A20-A0DB-941E2296818C", deviceAndPort.Id);
            Assert.AreEqual("port-2", deviceAndPort.Port);
            Assert.AreEqual("C6CBE649-16BD-4A20-A0DB-941E2296818C#port-2#", deviceAndPort.ToString());
        }

        [Test]
        public void EndpointWithoutPort_HasExpectedIdAndPort()
        {
            // Act
            DeviceAndPort deviceAndPort = new DeviceAndPort("C6CBE649-16BD-4A20-A0DB-941E2296818C");

            // Assert
            Assert.AreEqual("C6CBE649-16BD-4A20-A0DB-941E2296818C", deviceAndPort.Id);
            Assert.AreEqual("", deviceAndPort.Port);
            Assert.AreEqual("C6CBE649-16BD-4A20-A0DB-941E2296818C", deviceAndPort.ToString());
        }

        [Test]
        public void EndpointWithPort_HasExpectedIdAndPort()
        {
            // Act
            DeviceAndPort deviceAndPort = new DeviceAndPort("C6CBE649-16BD-4A20-A0DB-941E2296818C#port 2#");

            // Assert
            Assert.AreEqual("C6CBE649-16BD-4A20-A0DB-941E2296818C", deviceAndPort.Id);
            Assert.AreEqual("port 2", deviceAndPort.Port);
            Assert.AreEqual("C6CBE649-16BD-4A20-A0DB-941E2296818C#port 2#", deviceAndPort.ToString());
        }
    }
}