using System;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.SmartHome.Cooking;
using Tinamous.SmartHome.Tinamous;

namespace Tinamous.SmartHome.SmartHome
{
    public static class ControllerFactory
    {
        /// <summary>
        /// Get the controller class responsible for handing the request namespace
        /// </summary>
        /// <param name="requestNamespace"></param>
        /// <param name="restClient"></param>
        /// <returns></returns>
        public static IAlexaSmartHomeController GetController(string requestNamespace, ITinamousRestClient restClient)
        {
            var devicesClient = new DevicesClient(restClient);
            var measurementsClient = new MeasurementsClient(restClient);
            var statusClient = new StatusClient(restClient);

            switch (requestNamespace)
            {
                case "Alexa":
                    return new StateReportHelper(devicesClient);
                case "Alexa.Discovery":
                    return new DiscoveryHelper(devicesClient);
                case "Alexa.PowerController":
                    return new PowerController(devicesClient, measurementsClient, statusClient);
                case "Alexa.PowerLevelController":
                    return new PowerLevelController(devicesClient, measurementsClient, statusClient);
                case "Alexa.SceneController":
                    return new SceneController(devicesClient, measurementsClient, statusClient);
                case "Alexa.PercentageController":
                    return new PercentageController(devicesClient, measurementsClient, statusClient);
                case "Alexa.BrightnessController":
                    return new BrightnessController(devicesClient, measurementsClient, statusClient);
                case "Alexa.ColorController":
                    return new ColorController(devicesClient, measurementsClient, statusClient);
                case "Alexa.LockController":
                    return new LockController(devicesClient, measurementsClient, statusClient);
                case "Alexa.ThermostatController":
                    return new ThermostatController(devicesClient, measurementsClient, statusClient);
                case "Alexa.Authorization":
                    // https://developer.amazon.com/docs/device-apis/alexa-authorization.html
                    // AcceptGrant
                    // TODO: Implement Authorization Controller
                    LambdaLogger.Log("Alexa.Authorization not implemented");
                    throw new NotImplementedException(requestNamespace);
                    return new NotSupportedYetController(devicesClient, measurementsClient, statusClient);

                case "Alexa.TemperatureSensor":
                    return new TemperatureSensorController(devicesClient, measurementsClient, statusClient);


                case "Alexa.Cooking":
                    return new CookingController(devicesClient, measurementsClient, statusClient);
                case "Alexa.Cooking.TimeController":
                    return new CookingTimeController(devicesClient, measurementsClient, statusClient);
                case "Alexa.Cooking.PresetController":
                    return new CookingPresetController(devicesClient, measurementsClient, statusClient);
                case "Alexa.TimeHoldController":
                    return new CookingTimeHoldController(devicesClient, measurementsClient, statusClient);
                default:
                    LambdaLogger.Log("*** Unknown namespace: " + requestNamespace);
                    return new NotSupportedYetController(devicesClient, measurementsClient, statusClient);
            }
        }
    }
}