using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Models.PropertyModels;
using Tinamous.SmartHome.Tinamous;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.SmartHome
{
    /// <summary>
    /// See: https://developer.amazon.com/docs/device-apis/alexa-temperaturesensor.html
    /// </summary>
    public class TemperatureSensorController : AlexaSmartHomeInterfaceControllerBase
    {
        private const string InterfaceNamespace = "Alexa.TemperatureSensor";

        public TemperatureSensorController(DevicesClient devicesClient, MeasurementsClient measurementsClient, StatusClient statusClient)
            : base(devicesClient, measurementsClient, statusClient)
        { }

        public override Task<object> HandleAlexaRequest(SmartHomeRequest request, ILambdaContext context)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public override async Task<List<Property>> CreateProperties(string token, DeviceDto device, string port)
        {
            LambdaLogger.Log("Temperature StateReport");

            List<Property> properties = new List<Property>();

            FieldValueDto value = await GetFieldValue(token, device, "temperature", port);

            if (value != null && value.v.HasValue)
            {
                var temperatureProperty = new NumericValueWithUnitsProperty
                {
                    Namespace = InterfaceNamespace,
                    Name = "temperature",
                    Value = new TemperaturePropertyValue
                    {
                        // TODO: Support °F
                        Scale = "CELSIUS",
                        Value = value.v.Value,
                    },
                    TimeOfSample = DateTime.UtcNow,
                    UncertaintyInMilliseconds = 600
                };
                properties.Add(temperatureProperty);
            }

            return properties;
        }
    }
}