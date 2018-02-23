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
    public class ColorControllerTest
    {
        [Test]
        public async Task SetColor_ForSimpleDevice_SendsTurnsOn()
        {
            // Arrange
            string messageId = "Message12";
            var request = BuildRequest(messageId, "SetColor", "Endpoint1");

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

            var controller = new ColorController(devicesClient, measurementsClient, statusClient);

            // Act
            ColorControlResponse response = (ColorControlResponse)await controller.HandleAlexaRequest(request, null);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual("@Switch Set color HSV 23.5,0.3,0.2", statusClient.SentMessage);
        }

        [Test]
        public async Task SetColor_ForMultiPortDevice_TurnOn_Port1_SendsTurnOnPort1()
        {
            // Arrange
            string messageId = "Message12";
            var device = new DeviceDto { Id = "Endpoint1" };
            var deviceAndPort = new DeviceAndPort(device, 1);
            var request = BuildRequest(messageId, "SetColor", deviceAndPort.ToString());

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

            var controller = new ColorController(devicesClient, measurementsClient, statusClient);

            // Act
            ColorControlResponse response = (ColorControlResponse)await controller.HandleAlexaRequest(request, null);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual("@Switch Set color HSV 23.5,0.3,0.2 port-1", statusClient.SentMessage);
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
                    Payload = new Payload
                    {
                        Color = new HsvColor
                        {
                            Brightness = 0.2f,
                            Hue = 23.5f,
                            Saturation = 0.3f
                        }
                    },
                    Endpoint = new RequestEndpoint
                    {
                        // Port 1 on the device endpoint
                        EndpointId = endpointId,
                        Scope = new Scope
                        {
                            Token = "Tokenx"
                        }
                    }
                }
            };
        }
    }
}