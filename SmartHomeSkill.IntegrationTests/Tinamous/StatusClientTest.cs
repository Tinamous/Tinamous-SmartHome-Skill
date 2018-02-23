using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Tinamous.SmartHome.Tinamous;

namespace SmartHomeSkill.IntegrationTests.Tinamous
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
            var client = new StatusClient(new RestClient());

            // Act
            await client.PostStatusMessageAsync(_authToken, "Hello from Alexa Unit Tests...");

            // Assert
            // shouldn't blow up....
        }
    }
}