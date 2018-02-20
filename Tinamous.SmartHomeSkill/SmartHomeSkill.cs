using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.SmartHome;
using Tinamous.SmartHome.SmartHome.Cooking;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Tinamous.SmartHome
{
    // See: https://github.com/DamianMehers/AlexaSmartHomeDemo

    // https://optimalbi.com/blog/2017/02/17/tips-and-tricks-for-aws-lambda-functions-in-c-dotnet-core/

    // https://developer.amazon.com/docs/smarthome/steps-to-build-a-smart-home-skill.html#create-a-lambda-function

    // NB: To add Amazon.Lambda.Tools use... dotnet add package Amazon.Lambda.Tools (fails with NuGet)


    /// <summary>
    /// Main entry point of the Lambda expression, called
    /// by Alexa to process a message.
    /// </summary>
    /// <remarks>
    /// Function handler in Lambda: 
    /// Tinamous.SmartHome::Tinamous.SmartHome.SmartHomeSkill::FunctionHandler
    /// needs to be assembly::namespace.class::function
    /// </remarks>
    public class SmartHomeSkill
    {
        public async Task<object> FunctionHandler(SmartHomeRequest request, ILambdaContext context)
        {
            try
            {
                LambdaLogger.Log("***********************************************" + Environment.NewLine);
                LambdaLogger.Log("SmartHomeSkill - Test #11" + Environment.NewLine);
                LambdaLogger.Log(request.ToString() + Environment.NewLine);
                LambdaLogger.Log("Request Namespace: " + request.Directive.Header.Namespace);
                LambdaLogger.Log("Request Name: " + request.Directive.Header.Name);


                switch (request.Directive.Header.Namespace)
                {
                    case "Alexa":
                        var stateReportHelper = new StateReportHelper();
                        return await stateReportHelper.HandleAlexaRequest(request, context);
                    case "Alexa.Discovery":
                        var discoveryHelper = new DiscoveryHelper();
                        return await discoveryHelper.HandleDiscovery(request, context);
                    case "Alexa.PowerController":
                        var powerController = new PowerController();
                        return await powerController.HandlePowerControl(request, context);
                    case "Alexa.PowerLevelController":
                        // SetPowerLevel, AdjustPowerLevel
                        var powerLevelController = new PowerLevelController();
                        return await powerLevelController.HandleLevelPowerControl(request, context);
                    case "Alexa.SceneController":
                        // Activate, Deactivate
                        // TODO: Implement Scene Controller
                    case "Alexa.PercentageController":
                        var percentageController = new PercentageController();
                        return await percentageController.HandlePercentageControl(request, context);
                    case "Alexa.BrightnessController":
                        // AdjustBrightness, SetBrightness, 
                        var brightnessController = new BrightnessController();
                        return await brightnessController.HandleBrightnessControl(request, context);
                    case "Alexa.ColorController":
                        var colorController = new ColorController();
                        return await colorController.HandleColorControl(request, context);
                    case "Alexa.LockController":
                        // Lock, Unlock
                        // TODO: Implement Locl Controller
                    case "Alexa.ThermostatController":
                        // SetTargetTemperature
                        // AdjustTargetTemperature
                        // SetThermostatMode
                        // TODO: Implement ThermostatController

                    case "Alexa.Authorization":
                        // https://developer.amazon.com/docs/device-apis/alexa-authorization.html
                        // AcceptGrant
                        // TODO: Implement Authorization Controller
                        LambdaLogger.Log("Alexa.Authorization not implemented");
                        throw new NotImplementedException(request.Directive.Header.Namespace);


                    case "Alexa.Cooking":
                        var cookingController = new CookingController();
                        return await cookingController.HandleCooking(request, context);
                    case "Alexa.Cooking.TimeController":
                        var cookingTimeController = new CookinTimeController();
                        return await cookingTimeController.HandleCookingTimeController(request, context);
                    case "Alexa.Cooking.PresetController":
                        var cookingPresetController = new CookingPresetController();
                        return await cookingPresetController.HandleCookingPresetController(request, context);
                    case "Alexa.TimeHoldController":
                        var timeHoldController = new CookingTimeHoldController();
                        return await timeHoldController.HandleCookingTimeHoldController(request, context);
                    default:
                        LambdaLogger.Log("Unknown namespace: " + request.Directive.Header.Namespace);
                        break;
                }
            }
            catch (Exception ex)
            {
                LambdaLogger.Log("Exception processing function: " + ex + Environment.NewLine);
                return null;
            }
            return Task.FromResult("");
        }
    }
}
