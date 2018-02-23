using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.SmartHome;
using Tinamous.SmartHome.Tinamous;

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

                ITinamousRestClient restClient = new RestClient();

                IAlexaSmartHomeController smartHomeController = ControllerFactory.GetController(request.Directive.Header.Namespace, restClient);
                return await smartHomeController.HandleAlexaRequest(request, context);                
            }
            catch (Exception ex)
            {
                LambdaLogger.Log("Exception processing function: " + ex + Environment.NewLine);
                throw;
            }
        }
    }
}
