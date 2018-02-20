using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Tinamous.SmartHome.Tinamous;

namespace Tinamous.SmartHome.Tests.Tinamous
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
            DevicesClient helper = new DevicesClient();

            // Act
            var devices = await helper.GetDevicesAsync(_authToken);

            // Assert
            Assert.IsNotNull(devices);
            Assert.AreEqual(2, devices.Count);
        }
    }
}