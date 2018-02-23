using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.SmartHome;
using Tinamous.SmartHome.SmartHome.Models;
using Tinamous.SmartHome.Tests.SmartHome.Fakes;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.Tests.SmartHome
{
    [TestFixture]
    public class PowerControllerTest
    {
        public class TurnOn
        {
            [Test]
            public async Task ForSimpleDevice_SendsTurnsOn()
            {
                // Arrange
                string messageId = "Message12";
                var request = BuildRequest(messageId, "TurnOn", "Endpoint1");

                List<DeviceDto> devices = new List<DeviceDto>
                {
                    new DeviceDto
                    {
                        DisplayName = "Device 1"
                    }
                };

                var devicesClient = new FakeDevicesClient(devices);
                var measurementsClient = new FakeMeasurementsClient();
                var statusClient = new FakeStatusClient();

                var controller = new PowerController(devicesClient, measurementsClient, statusClient);

                // Act
                PowerControlResponse response =
                    (PowerControlResponse)await controller.HandleAlexaRequest(request, null);

                // Assert
                Assert.IsNotNull(response);
                Assert.AreEqual("@Switch Turn On", statusClient.SentMessage);
            }

            [Test]
            public async Task ForMultiPortDevice_TurnOn_Port1_SendsTurnOnPort1()
            {
                // Arrange
                string messageId = "Message12";
                var device = new DeviceDto { Id = "Endpoint1" };
                var deviceAndPort = new DeviceAndPort(device, 1);
                var request = BuildRequest(messageId, "TurnOn", deviceAndPort.ToString());

                List<DeviceDto> devices = new List<DeviceDto>
                {
                    new DeviceDto
                    {
                        DisplayName = "Device 1"
                    }
                };

                var devicesClient = new FakeDevicesClient(devices);
                var measurementsClient = new FakeMeasurementsClient();
                var statusClient = new FakeStatusClient();

                var controller = new PowerController(devicesClient, measurementsClient, statusClient);

                // Act
                PowerControlResponse response =
                    (PowerControlResponse)await controller.HandleAlexaRequest(request, null);

                // Assert
                Assert.IsNotNull(response);
                Assert.AreEqual("@Switch Turn On port-1", statusClient.SentMessage);
            }
        }

        public class TurnOff
        {
            [Test]
            public async Task ForSimpleDevice_SendsTurnsOn()
            {
                // Arrange
                string messageId = "Message12";
                var request = BuildRequest(messageId, "TurnOff", "Endpoint1");

                List<DeviceDto> devices = new List<DeviceDto>
                {
                    new DeviceDto
                    {
                        DisplayName = "Device 1"
                    }
                };

                var devicesClient = new FakeDevicesClient(devices);
                var measurementsClient = new FakeMeasurementsClient();
                var statusClient = new FakeStatusClient();

                var controller = new PowerController(devicesClient, measurementsClient, statusClient);

                // Act
                PowerControlResponse response =
                    (PowerControlResponse)await controller.HandleAlexaRequest(request, null);

                // Assert
                Assert.IsNotNull(response);
                Assert.AreEqual("@Switch Turn Off", statusClient.SentMessage);
            }

            [Test]
            public async Task ForMultiPortDevice_TurnOn_Port1_SendsTurnOnPort1()
            {
                // Arrange
                string messageId = "Message12";
                var device = new DeviceDto { Id = "Endpoint1" };
                var deviceAndPort = new DeviceAndPort(device, 1);
                var request = BuildRequest(messageId, "TurnOff", deviceAndPort.ToString());

                List<DeviceDto> devices = new List<DeviceDto>
                {
                    new DeviceDto
                    {
                        DisplayName = "Device 1"
                    }
                };

                var devicesClient = new FakeDevicesClient(devices);
                var measurementsClient = new FakeMeasurementsClient();
                var statusClient = new FakeStatusClient();

                var controller = new PowerController(devicesClient, measurementsClient, statusClient);

                // Act
                PowerControlResponse response =
                    (PowerControlResponse)await controller.HandleAlexaRequest(request, null);

                // Assert
                Assert.IsNotNull(response);
                Assert.AreEqual("@Switch Turn Off port-1", statusClient.SentMessage);
            }
        }

        private static SmartHomeRequest BuildRequest(string messageId, string name, string endpointId)
        {
            return new SmartHomeRequest
            {
                Directive = new Directive
                {
                    Header = new Header
                    {
                        MessageId = messageId,
                        Name = name
                    },
                    Payload = new Payload { },
                    Endpoint = new RequestEndpoint
                    {
                        // Port 1 on the device endpoint
                        EndpointId = endpointId,
                        Scope = new Scope
                        {
                            Token = "Tokenx"
                        },
                        Cookie = new Cookie
                        {
                            Username = "Switch",
                        }
                    }
                }
            };
        }
    }
}