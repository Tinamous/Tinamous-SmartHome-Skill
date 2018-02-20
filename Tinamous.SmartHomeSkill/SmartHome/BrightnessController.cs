using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Models.PropertyModels;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.SmartHome
{
    /// <summary>
    /// See https://developer.amazon.com/docs/device-apis/alexa-brightnesscontroller.html
    /// </summary>
    public class BrightnessController : AlexaInterfaceControllerBase
    {
        // / AdjustBrightness, SetBrightness, 
        public Task<BrightnessControlResponse> HandleBrightnessControl(SmartHomeRequest request, ILambdaContext context)
        {
            switch (request.Directive.Header.Name)
            {
                case "SetBrightness":
                    return HandleSetBrightness(request, context);
                case "AdjustBrightness":
                    return HandleAdjustBrightness(request, context);
                default:
                    return null;
            }
        }

        private async Task<BrightnessControlResponse> HandleSetBrightness(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Set brightness");
            string token = request.Directive.Endpoint.Scope.Token;

            var level = request.Directive.Payload.Brightness;

            string message = string.Format("Set brightness {0}", level);
            await SendDeviceStatusMessage(request, token, message);

            // Assume it worked...
            return new BrightnessControlResponse
            {
                Context = new Context
                {
                    Properties = new List<Property>
                    {
                        new IntValueProperty
                        {
                            Namespace = "Alexa.BrightnessController",
                            Name = "brightness",
                            Value = level, // TODO: Get the actual value back!
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        },
                        new ValueValueProperty
                        {
                            Namespace = "Alexa.BrightnessController",
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

        private async Task<BrightnessControlResponse> HandleAdjustBrightness(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Adjust brightness");
            string token = request.Directive.Endpoint.Scope.Token;

            var level = request.Directive.Payload.PowerLevelDelta;

            string message = string.Format("Adjust brightness {0}", level);
            await SendDeviceStatusMessage(request, token, message);

            // Give the device some time to update.
            Thread.Sleep(2000);

            FieldValueDto fieldValue = await GetFieldValue(request, token, "brightness");
            fieldValue = fieldValue ?? new FieldValueDto {v = 50};

            // Assume it worked...
            return new BrightnessControlResponse
            {
                Context = new Context
                {
                    Properties = new List<Property>
                    {
                        new IntValueProperty
                        {
                            Namespace = "Alexa.BrightnessController",
                            Name = "brightness",
                            Value = Convert.ToInt32(fieldValue.v),
                            TimeOfSample = DateTime.UtcNow,
                            UncertaintyInMilliseconds = 600,
                        },
                        new ValueValueProperty
                        {
                            Namespace = "Alexa.BrightnessController",
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