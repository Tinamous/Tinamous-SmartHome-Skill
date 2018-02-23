﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Models.PropertyModels;
using Tinamous.SmartHome.Tinamous;
using Tinamous.SmartHome.Tinamous.Dtos;
using Tinamous.SmartHome.Tinamous.Interfaces;

namespace Tinamous.SmartHome.SmartHome.Cooking
{
    /// <summary>
    /// https://developer.amazon.com/docs/device-apis/alexa-timeholdcontroller.html
    /// </summary>
    public class CookingTimeHoldController : AlexaSmartHomeInterfaceControllerBase
    {
        public CookingTimeHoldController(IDevicesClient devicesClient, IMeasurementsClient measurementsClient, IStatusClient statusClient) 
            : base(devicesClient, measurementsClient, statusClient)
        { }

        public override Task<object> HandleAlexaRequest(SmartHomeRequest request, ILambdaContext context)
        {
            // Hold
            // Resume
            return NotSupportedDirective(request.Directive);
            // CookingErrorResponse
        }

        public override Task<List<Property>> CreateProperties(string token, DeviceDto device, string port)
        {
            throw new NotImplementedException();
        }
    }
}