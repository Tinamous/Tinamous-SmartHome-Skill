using System.Threading.Tasks;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.Tinamous
{
    public interface IMeasurementsClient
    {
        Task<FieldValueDto> GetFieldValueAsync(string authToken, string deviceId, FieldDescriptorDto field);
    }
}