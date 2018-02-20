using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Tinamous.SmartHome.Tinamous;

namespace Tinamous.SmartHome.Tests.Tinamous
{
    [TestFixture]
    public class StatusClientTest
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
        public async Task PostStatus()
        {
            // Arrange
            var client = new StatusClient();

            // Act
            await client.PostStatusMessageAsync(_authToken, "Hello from Alexa Unit Tests...");

            // Assert
            // shouldn't blow up....
        }
    }
}