using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Models.PropertyModels;
using Tinamous.SmartHome.Tinamous;
using Tinamous.SmartHome.Tinamous.Dtos;
using Tinamous.SmartHome.Tinamous.Interfaces;

namespace Tinamous.SmartHome.SmartHome
{
    public class ThermostatController : AlexaSmartHomeInterfaceControllerBase
    {
        public ThermostatController(IDevicesClient devicesClient, IMeasurementsClient measurementsClient, IStatusClient statusClient) 
            : base(devicesClient, measurementsClient, statusClient)
        { }

        public override Task<object> HandleAlexaRequest(SmartHomeRequest request, ILambdaContext context)
        {
            // SetTargetTemperature
            // AdjustTargetTemperature
            // SetThermostatMode

            // TODO: Some code, implementation of something...
            return NotSupportedDirective(request.Directive);
        }

        public override Task<List<Property>> CreateProperties(string token, DeviceDto device, string port)
        {
            throw new System.NotImplementedException();
        }
    }
}