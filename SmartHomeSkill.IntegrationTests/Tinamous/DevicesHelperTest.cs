using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Tinamous.SmartHome.Tinamous;

namespace SmartHomeSkill.IntegrationTests.Tinamous
{
    [TestFixture]
    public class DevicesHelperTest
    {
        private string _authToken;

        [SetUp]
        public void Setup()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("secrets.json")
                .Build();

            _authToken = config["BearerToken"];

            if (string.IsNullOrEmpty(_authToken))
            {
                throw new Exception("No auth token");
            }
        }

        [Test]
        public async Task GetDevices()
        {
            // Arrange
            var restClient = new RestClient();
            DevicesClient helper = new DevicesClient(restClient);

            // Act
            var devices = await helper.GetDevicesAsync(_authToken);

            // Assert
            Assert.IsNotNull(devices);
            Assert.AreEqual(4, devices.Count);
        }
    }
}