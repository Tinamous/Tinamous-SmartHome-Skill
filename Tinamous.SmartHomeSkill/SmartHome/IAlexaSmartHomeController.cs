using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Models.PropertyModels;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.SmartHome
{
    public interface IAlexaSmartHomeController
    {
        Task<object> HandleAlexaRequest(SmartHomeRequest request, ILambdaContext context);
        Task<List<Property>> CreateProperties(string token, DeviceDto device, string port);
    }
}