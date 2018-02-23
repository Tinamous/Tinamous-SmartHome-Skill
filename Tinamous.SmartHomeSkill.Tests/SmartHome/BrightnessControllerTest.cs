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
    public class CookingTimeControllerTest
    {
        [Test]
        public async Task SetBrightness_ForSimpleDevice_SendsTurnsOn()
        {
            // Arrange
            string messageId = "Message12";
            var request = BuildRequest(messageId, "SetBrightness", "Endpoint1");

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

            var controller = new BrightnessController(devicesClient, measurementsClient, statusClient);

            // Act
            BrightnessControlResponse response = (BrightnessControlResponse)await controller.HandleAlexaRequest(request, null);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual("@Switch Set brightness 50", statusClient.SentMessage);
        }

        [Test]
        public async Task SetBrightness_ForMultiPortDevice_TurnOn_Port1_SendsTurnOnPort1()
        {
            // Arrange
            string messageId = "Message12";
            var device = new DeviceDto { Id = "Endpoint1" };
            var deviceAndPort = new DeviceAndPort(device, 1);
            var request = BuildRequest(messageId, "SetBrightness", deviceAndPort.ToString());

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

            var controller = new BrightnessController(devicesClient, measurementsClient, statusClient);

            // Act
            BrightnessControlResponse response = (BrightnessControlResponse)await controller.HandleAlexaRequest(request, null);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual("@Switch Set brightness 50 port-1", statusClient.SentMessage);
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
                        Brightness = 50,
                        BrightnessDelta = 10,
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