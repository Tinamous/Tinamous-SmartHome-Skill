using System.Threading.Tasks;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.Tinamous
{
    public class StatusClient : IStatusClient
    {
        private readonly ITinamousRestClient _restClient;
        private string baseUri = "api/v1/status";

        public StatusClient(ITinamousRestClient restClient)
        {
            _restClient = restClient;
        }

        public Task PostStatusMessageAsync(string token, string message)
        {
            StatusMessageRequestDto status = new StatusMessageRequestDto
            {
                Message = message,
            };

            return _restClient.Post(token, baseUri, status);
        }
    }
}