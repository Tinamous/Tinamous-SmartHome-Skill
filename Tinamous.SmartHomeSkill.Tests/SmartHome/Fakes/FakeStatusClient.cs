using System.Threading.Tasks;
using Tinamous.SmartHome.Tinamous;

namespace Tinamous.SmartHome.Tests.SmartHome.Fakes
{
    public class FakeStatusClient : IStatusClient
    {
        public Task PostStatusMessageAsync(string token, string message)
        {
            SentMessage = message;
            return Task.CompletedTask;
        }

        public string SentMessage { get; set; }
    }
}