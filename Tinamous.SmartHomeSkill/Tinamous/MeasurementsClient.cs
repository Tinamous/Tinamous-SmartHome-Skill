using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.Tinamous
{
    public class MeasurementsClient : RestClientBase
    {
        // api/v1/devices/{deviceId}/measurements/channel/{channel}/field/{field}/latest
        private string baseUri = "api/v1/devices/{0}/measurements/channel/{1}/field/{2}/latest";
        public async Task<FieldValueDto> GetFieldValueAsync(string authToken, string deviceId, FieldDescriptorDto field)
        {
            var uri = string.Format(baseUri, deviceId, field.Channel, field.Name);
            LambdaLogger.Log("Get field from: " + uri);
            SenMLDto senml = await GetAsJsonAsync<SenMLDto>(authToken, uri);

            return senml.e.FirstOrDefault();
        }
    }
}