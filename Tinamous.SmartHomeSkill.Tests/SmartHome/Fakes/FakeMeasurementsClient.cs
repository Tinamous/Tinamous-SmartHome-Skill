using System.Threading.Tasks;
using Tinamous.SmartHome.Tinamous;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.Tests.SmartHome.Fakes
{
    public class FakeMeasurementsClient : IMeasurementsClient
    {
        public Task<FieldValueDto> GetFieldValueAsync(string authToken, string deviceId, FieldDescriptorDto field)
        {
            throw new System.NotImplementedException();
        }
    }
}