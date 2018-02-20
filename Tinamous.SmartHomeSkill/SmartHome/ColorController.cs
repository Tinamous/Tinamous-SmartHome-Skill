using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Models.PropertyModels;

namespace Tinamous.SmartHome.SmartHome
{
    /// <summary>
    /// See: https://developer.amazon.com/docs/device-apis/alexa-colorcontroller.html
    /// </summary>
    public class ColorController : AlexaInterfaceControllerBase
    {
        public Task<ColorControlResponse> HandleColorControl(SmartHomeRequest request, ILambdaContext context)
        {
            switch (request.Directive.Header.Name)
            {
                case "SetColor":
                    return HandleSetColor(request, context);
                default:
                    return null;
            }
        }

        private async Task<ColorControlResponse> HandleSetColor(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Set color");
            string token = request.Directive.Endpoint.Scope.Token;

            HsvColor color = request.Directive.Payload.Color;
            if (color == null)
            {
                LambdaLogger.Log("No color!");
                throw new NullReferenceException("Color");
            }
            LambdaLogger.Log("Got a color");

            string message = string.Format("Set color HSV {0},{1},{2}",
                color.Hue,
                color.Saturation,
                color.Brightness);

            await SendDeviceStatusMessage(request, token, message);

            // Assume it worked...
            return new ColorControlResponse
            {
                Context = new Context
                {
                    Properties = new List<Property>
                        {
                            new ColorValueProperty
                            {
                                Namespace = "Alexa.ColorController",
                                Name = "color",
                                Value = color, // TODO: Get the actual value back!
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