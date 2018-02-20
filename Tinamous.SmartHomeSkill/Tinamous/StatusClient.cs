using System.Threading.Tasks;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.Tinamous
{
    public class StatusClient : RestClientBase
    {
        private string baseUri = "api/v1/status";

        public Task PostStatusMessageAsync(string token, string message)
        {
            StatusMessageRequestDto status = new StatusMessageRequestDto
            {
                Message = message,
            };

            return Post(token, baseUri, status);
        }
    }
}