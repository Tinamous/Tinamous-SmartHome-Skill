using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Models.PropertyModels;

namespace Tinamous.SmartHome.SmartHome
{
    /// <summary>
    /// Alexa, increase device name by number percent
    /// </summary>
    public class PercentageController : AlexaInterfaceControllerBase
    {
        public Task<PowerControlResponse> HandlePercentageControl(SmartHomeRequest request, ILambdaContext context)
        {
            switch (request.Directive.Header.Name)
            {
                case "SetPercentage":
                    return HandleSetPercentage(request, context);
                case "AdjustPercentage":
                    return AdjustPercentage(request, context);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Alexa, set name to number percen
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<PowerControlResponse> HandleSetPercentage(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Set powerlevel");
            string token = request.Directive.Endpoint.Scope.Token;

            var level = request.Directive.Payload.Percentage;

            string message = string.Format("Set Percentage {0}", level);
            await SendDeviceStatusMessage(request, token, message);

            // Assume it worked...
            return new PowerControlResponse
            {
                Context = new Context
                {
                    Properties = new List<Property>
                    {
                        new IntValueProperty
                        {
                            Namespace = "Alexa.PercentageController",
                            Name = "powerLevel",
                            Value = level, // TODO: Get the actual value back!
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        },
                        new ValueValueProperty
                        {
                            Namespace = "Alexa.EndpointHealth",
                            Name = "connectivity",
                            Value = new ValuePropertyValue {Value = "ON"},
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        },
                    },
                },
                Event = ConstructReponseEvent(request.Directive, "Response"),
            };
        }

        private async Task<PowerControlResponse> AdjustPercentage(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Adjust Percentage");
            string token = request.Directive.Endpoint.Scope.Token;

            var delta = request.Directive.Payload.PercentageDelta;

            string message = string.Format("Adjust percentage {0}", delta);
            await SendDeviceStatusMessage(request, token, message);

            // Assume it worked...
            return new PowerControlResponse
            {
                Context = new Context
                {
                    Properties = new List<Property>
                    {
                        new IntValueProperty
                        {
                            Namespace = "Alexa.PercentageController",
                            Name = "powerLevel",
                            Value = 50, // TODO: Get the actual value back!
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        },
                        new ValueValueProperty
                        {
                            Namespace = "Alexa.EndpointHealth",
                            Name = "connectivity",
                            Value = new ValuePropertyValue {Value = "ON"},
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        },
                    },
                },
                Event = ConstructReponseEvent(request.Directive, "Response"),
            };
        }
    }
}