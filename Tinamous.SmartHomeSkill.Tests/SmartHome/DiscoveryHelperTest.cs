using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.SmartHome;
using Tinamous.SmartHome.Tests.SmartHome.Fakes;
using Tinamous.SmartHome.Tinamous.Dtos;
using Tinamous.SmartHome.Tinamous.Interfaces;

namespace Tinamous.SmartHome.Tests.SmartHome
{
    [TestFixture]
    public class DiscoveryHelperTest
    {
        [Test]
        public async Task DiscoverDevices_WithMultiPorts_ShouldAddMultipleDevices()
        {
            // Arrange
            List<DeviceDto> devices = new List<DeviceDto>
            {
                new DeviceDto
                {
                    MetaTags = new List<MetaTagDto>
                    {
                        new MetaTagDto {Name = "PortCount", NumericValue = 4},
                        new MetaTagDto {Name = "Port 1", Value="USB Light"},
                        new MetaTagDto {Name = "Port 4", Value="ThingyStick"},
                    },
                    Tags = new List<string> {"MultiPort"},
                    DisplayName = "Test Device",
                    Id = "Device-Id"
                }
            };

            
            string messageId = "MessageId1";
            SmartHomeRequest request = BuildRequest(messageId);

            IDevicesClient devicesClient = new FakeDevicesClient(devices);
            DiscoveryHelper discoveryHelper = new DiscoveryHelper(devicesClient);

            // Act
            DiscoverResponse response = (DiscoverResponse) await discoveryHelper.HandleAlexaRequest(request, null);

            // Assert
            Assert.IsNotNull(response, "response nul");
            Assert.IsNotNull(response.Event, "Event null");
            Assert.IsNotNull(response.Event.Header, "Header null");
            Assert.AreEqual("Alexa.Discovery", response.Event.Header.Namespace, "Namespace");
            Assert.AreEqual("Discover.Response", response.Event.Header.Name, "Name");
            Assert.AreEqual("3", response.Event.Header.PayloadVersion, "PayloadVersion");
            Assert.AreEqual(messageId, response.Event.Header.MessageId, "MessageId");

            var endpoints = response.Event.Payload.Endpoints;
            Assert.IsNotNull(endpoints, "response.Event.Payload.Endpoints null");

            // Expect 2 sub-devices as only 2 are names.
            // plus one parent device.
            Assert.AreEqual(3, endpoints.Count, "response.Event.Payload.Endpoints null");

            Assert.AreEqual("Test Device", endpoints[0].FriendlyName, "endpoints[0].FriendlyName");
            Assert.AreEqual("Device-Id", endpoints[0].EndpointId, "endpoints[0].EndpointId");

            Assert.AreEqual("USB Light", endpoints[1].FriendlyName, "endpoints[1].FriendlyName");
            Assert.AreEqual("Device-Id*port-1*", endpoints[1].EndpointId, "endpoints[1].EndpointId");

            Assert.AreEqual("ThingyStick", endpoints[2].FriendlyName, "endpoints[2].FriendlyName");
            Assert.AreEqual("Device-Id*port-4*", endpoints[2].EndpointId, "endpoints[2].EndpointId");
        }

        [Test]
        public async Task DiscoverDevices_ForSingleDevice_ShouldOnlyAddDevice()
        {
            // Arrange
            List<DeviceDto> devices = new List<DeviceDto>
            {
                new DeviceDto
                {
                    MetaTags = new List<MetaTagDto>(),
                    Tags = new List<string> (),
                    DisplayName = "Test Device",
                    Id = "Device-Id"
                }
            };


            string messageId = "MessageId1";
            SmartHomeRequest request = BuildRequest(messageId);

            IDevicesClient devicesClient = new FakeDevicesClient(devices);
            DiscoveryHelper discoveryHelper = new DiscoveryHelper(devicesClient);

            // Act
            DiscoverResponse response = (DiscoverResponse)await discoveryHelper.HandleAlexaRequest(request, null);

            // Assert
            Assert.IsNotNull(response, "response nul");
            Assert.IsNotNull(response.Event, "Event null");
            Assert.IsNotNull(response.Event.Header, "Header null");
            Assert.AreEqual("Alexa.Discovery", response.Event.Header.Namespace, "Namespace");
            Assert.AreEqual("Discover.Response", response.Event.Header.Name, "Name");
            Assert.AreEqual("3", response.Event.Header.PayloadVersion, "PayloadVersion");
            Assert.AreEqual(messageId, response.Event.Header.MessageId, "MessageId");

            var endpoints = response.Event.Payload.Endpoints;
            Assert.IsNotNull(endpoints, "response.Event.Payload.Endpoints null");

            // Expect 2 sub-devices as only 2 are names.
            // plus one parent device.
            Assert.AreEqual(1, endpoints.Count, "response.Event.Payload.Endpoints null");

            Assert.AreEqual("Test Device", endpoints[0].FriendlyName, "endpoints[0].FriendlyName");
            Assert.AreEqual("Device-Id", endpoints[0].EndpointId, "endpoints[0].EndpointId");
        }

        private static SmartHomeRequest BuildRequest(string messageId)
        {            
            return new SmartHomeRequest
            {
                Directive = new Directive
                {
                    Header = new Header
                    {
                        MessageId = messageId
                    },
                    Payload = new Payload
                    {
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