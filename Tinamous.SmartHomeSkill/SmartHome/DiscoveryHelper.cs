using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Tinamous.SmartHome.Tinamous.Extension;
using Tinamous.SmartHome.Models;
using Tinamous.SmartHome.Tinamous;
using Tinamous.SmartHome.Tinamous.Dtos;

namespace Tinamous.SmartHome.SmartHome
{
    public class DiscoveryHelper
    {
        /// <summary>
        /// These are the smart home Alexa Interface's supported, Tinamous devices should be tagged with these
        /// if they support them (in addition to the Alexa.SmartHome tag).
        /// </summary>
        private static List<string> _supportedInterfaces = new List<string>
        {
            "Alexa.TemperatureSensor",
            "Alexa.PercentageController",
            "Alexa.BrightnessController",
            "Alexa.ColorController",
            "Alexa.PowerController",
            "Alexa.PowerLevelController",
            "Alexa.LockController",
            "Alexa.SceneController",
            "Alexa.ThermostatController",
            "Alexa.Cooking.TimeController"
        };

        private static List<string> _displayCategoriesTags = new List<string>
        {
            "ActivityTrigger",
            "Camera",
            "Door",
            "Light",
            "Other",
            "SceneTrigger",
            "SmartLock",
            "SmartPlug",
            "Speaker",
            "Switch",
            "TemperatureSensor",
            "Thermostat",
            "TV",
        };

        public async Task<DiscoverResponse> HandleDiscovery(SmartHomeRequest request, ILambdaContext context)
        {
            LambdaLogger.Log("Building discovery list.");

            DiscoverResponse response = new DiscoverResponse
            {
                Event = new Event
                {
                    Header = new Header
                    {
                        MessageId = request.Directive.Header.MessageId,
                        Namespace = "Alexa.Discovery",
                        Name = "Discover.Response",
                        PayloadVersion = "3"
                    },
                    Payload = new EventPayload
                    {
                        Endpoints = new List<Endpoint>()
                    }
                }
            };

            await AddDeviceEndpoints(request, response);

            LambdaLogger.Log("Discovery complete");
            LambdaLogger.Log("***********************************************" + Environment.NewLine);

            return response;
        }

        /// <summary>
        /// Add Tinamous devices tagged with "Alexa.SmartDevice" 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task AddDeviceEndpoints(SmartHomeRequest request, DiscoverResponse response)
        {
            // Get a list of devices filtered by the Alexa.SmartDevice Tag from Tinamous.
            var helper = new DevicesClient();
            List<DeviceDto> devices = await helper.GetDevicesAsync(request.Directive.Payload.Scope.Token);
            LambdaLogger.Log("Found " + devices.Count + " devices from Tinamous.");

            // Add each of the devices as an endpoint.
            foreach (DeviceDto device in devices)
            {
                LambdaLogger.Log("Adding device " + device.DisplayName);

                // Add the base device, regardless of if it's a 2 port, 4 port or whatever outlet device.
                response.Event.Payload.Endpoints.Add(CreateDeviceEndpoint(device));

                if (device.Tags.Contains("4Port"))
                {
                    LambdaLogger.Log("Adding 4 Port Device as multiple-devices.");
                    response.Event.Payload.Endpoints.AddRange(CreateMultiDeviceEndpoint(device, 4));
                }
                else if (device.Tags.Contains("2Port"))
                {
                    LambdaLogger.Log("Adding 2 Port Device as multiple-devices.");
                    response.Event.Payload.Endpoints.AddRange(CreateMultiDeviceEndpoint(device, 2));
                }
            }
        }

        // If a meta tag is set with the nane "Port x" then use the value 
        // as the device name, otherwise use the port number.
        private string GetPortName(DeviceDto device, int port)
        {
            if (device.MetaTags != null)
            {
                string portKey = string.Format("Port {0}", port + 1);
                var metaTag = device
                    .MetaTags
                    .FirstOrDefault(x => portKey.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase));

                if (metaTag != null)
                {
                    return metaTag.Value;
                }
            }
            return string.Format("{0} Port {1}", device.DisplayName, port + 1);
        }

        /// <summary>
        /// Add in n devices for the one device that has multiple outlets.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="portCount"></param>
        /// <returns></returns>
        private List<Endpoint> CreateMultiDeviceEndpoint(DeviceDto device, int portCount)
        {
            List<Endpoint> endpoints = new List<Endpoint>();
            for (int port = 0; port < portCount; port++)
            {
                var endpoint = CreateDeviceEndpoint(device);
                endpoint.EndpointId += "-port" + port.ToString() + "*";
                endpoint.FriendlyName = GetPortName(device, port);
                
                endpoints.Add(endpoint);
            }
            return endpoints;
        }

        private Endpoint CreateDeviceEndpoint(DeviceDto device)
        {
            var endpoint = new Endpoint
            {
                EndpointId = device.Id,
                FriendlyName = device.DisplayName,
                Description = device.Description,
                ManufacturerName = "TinamousSteve",
                DisplayCategories = new List<string>(),
                Cookie = new Cookie
                {
                    Username = device.UserName,
                },
                Capabilities = new List<Capability>
                {
                    new Capability
                    {
                        Type = "AlexaInterface",
                        Interface = "Alexa",
                        Version = "3",
                    },
                }
            };

            AddDisplayCategories(device, endpoint);

            AddDeviceCapabilities(device, endpoint);

            return endpoint;
        }

        private void AddDisplayCategories(DeviceDto device, Endpoint endpoint)
        {
            // Populate the Display Categoris based on tags found on the device.
            foreach (string displayCategoryTag in _displayCategoriesTags)
            {
                if (device.Tags.Contains(displayCategoryTag))
                {
                    DisplayCategories category = GetDisplayCategory(displayCategoryTag);
                    endpoint.DisplayCategories.Add(category.ToString());
                }
            }
        }

        private void AddDeviceCapabilities(DeviceDto device, Endpoint endpoint)
        {
            // Populate the Supported Interfaces based on the tags on the device.
            foreach (string supportedInterface in _supportedInterfaces)
            {
                if (device.Tags.Contains(supportedInterface))
                {
                    endpoint.Capabilities.Add(CreateInterfaceCapability(supportedInterface, device));
                }
            }

            // Always add the endpoint health category.
            endpoint.Capabilities.Add(CreateInterfaceCapability("Alexa.EndpointHealth", device));
        }

        private Capability CreateInterfaceCapability(string supportedInterface, DeviceDto device)
        {
            var capability = CreateCapability(supportedInterface);

            switch (supportedInterface)
            {
                case "Alexa.TemperatureSensor":
                    PopulateTemperatureSensorCapability(capability, device);
                    break;
                case "Alexa.PercentageController":
                    PopulatePercentageControllerCapability(capability, device);
                    break;
                case "Alexa.BrightnessController":
                    PopulateBrightnessControllerCapability(capability, device);
                    break;
                case "Alexa.ColorController":
                    PopulateColorControllerCapability(capability, device);
                    break;
                case "Alexa.PowerController":
                    PopulatePowerControllerCapability(capability, device);
                    break;
                case "Alexa.PowerLevelController":
                    PopulatePowerLevelControllerCapability(capability, device);
                    break;
                case "Alexa.LockController":
                    PopulateLockControllerCapability(capability, device);
                    break;
                case "Alexa.EndpointHealth":
                    PopulateEndpointHealthCapability(capability, device);
                    break;
                case "Alexa.SceneController":
                    PopulateSceneControllerCapability(capability, device);
                    break;
                case "Alexa.ThermostatController":
                    PopulateThermostatControllerCapability(capability, device);
                    break;
                case "Alexa.Cooking.TimeController":
                    PopulateCookingTimeControllerCapability(capability, device);
                    break;
                default:
                    LambdaLogger.Log("Missing support for supported capability: " + supportedInterface);
                    break;
            }

            return capability;
        }

        private Capability CreateCapability(string supportedInterface)
        {
            return new Capability
            {
                Type = "AlexaInterface",
                Interface = supportedInterface,
                Version = "3",
                Properties = new Properties
                {
                    Supported = new List<Supported>(),
                    // If true, we need to send update to Alexa.
                    ProactivelyReported = false,
                    Retrievable = true,
                }
            };
        }

        /// <summary>
        /// https://developer.amazon.com/docs/device-apis/alexa-powercontroller.html
        /// </summary>
        /// <param name="capability"></param>
        /// <param name="device"></param>
        private void PopulatePowerControllerCapability(Capability capability, DeviceDto device)
        {
            PopulatePropertyIfSupported(capability, device, "powerState");
        }

        /// <summary>
        /// https://developer.amazon.com/docs/device-apis/alexa-brightnesscontroller.html
        /// </summary>
        /// <param name="capability"></param>
        /// <param name="device"></param>
        private void PopulateBrightnessControllerCapability(Capability capability, DeviceDto device)
        {
            PopulatePropertyIfSupported(capability, device, "brightness");
        }

        /// <summary>
        /// https://developer.amazon.com/docs/device-apis/alexa-percentagecontroller.html
        /// </summary>
        /// <param name="capability"></param>
        /// <param name="device"></param>
        private void PopulatePercentageControllerCapability(Capability capability, DeviceDto device)
        {
            PopulatePropertyIfSupported(capability, device, "percentage");
        }

        /// <summary>
        /// https://developer.amazon.com/docs/device-apis/alexa-thermostatcontroller.html
        /// </summary>
        /// <param name="capability"></param>
        /// <param name="device"></param>
        private void PopulateThermostatControllerCapability(Capability capability, DeviceDto device)
        {
            PopulatePropertyIfSupported(capability, device, "lowerSetpoint");
            PopulatePropertyIfSupported(capability, device, "targetSetpoint");
            PopulatePropertyIfSupported(capability, device, "upperSetpoint");
            PopulatePropertyIfSupported(capability, device, "thermostatMode");
        }

        /// <summary>
        /// https://developer.amazon.com/docs/device-apis/alexa-temperaturesensor.html
        /// </summary>
        /// <param name="capability"></param>
        /// <param name="device"></param>
        private void PopulateTemperatureSensorCapability(Capability capability, DeviceDto device)
        {
            PopulatePropertyIfSupported(capability, device, "temperature");
        }

        /// <summary>
        /// https://developer.amazon.com/docs/device-apis/alexa-lockcontroller.html
        /// </summary>
        /// <param name="capability"></param>
        /// <param name="device"></param>
        private void PopulateLockControllerCapability(Capability capability, DeviceDto device)
        {
            PopulatePropertyIfSupported(capability, device, "lockState");
        }

        /// <summary>
        /// https://developer.amazon.com/docs/device-apis/alexa-colorcontroller.html
        /// </summary>
        /// <param name="capability"></param>
        /// <param name="device"></param>
        private void PopulateColorControllerCapability(Capability capability, DeviceDto device)
        {
            PopulatePropertyIfSupported(capability, device, "color");
        }

        /// <summary>
        /// https://developer.amazon.com/docs/device-apis/alexa-scenecontroller.html
        /// </summary>
        /// <param name="capability"></param>
        /// <param name="device"></param>
        private void PopulateSceneControllerCapability(Capability capability, DeviceDto device)
        {
            // No supported properties...
            //PopulatePropertyIfSupported(capability, device, "....");
        }

        // Camera Feed (Alexa Docs broken...).
        // ......


        /// <summary>
        /// https://developer.amazon.com/docs/device-apis/alexa-powerlevelcontroller.html
        /// </summary>
        /// <param name="capability"></param>
        /// <param name="device"></param>
        private void PopulatePowerLevelControllerCapability(Capability capability, DeviceDto device)
        {
            PopulatePropertyIfSupported(capability, device, "powerLevel");
        }

        private void PopulateCookingTimeControllerCapability(Capability capability, DeviceDto device)
        {
            capability.Properties.Supported.Add(new Supported { Name = "cookTime" });
            capability.Properties.Supported.Add(new Supported { Name = "powerLevel" });

            capability.Configuration = new CookingConfiguration
            {
                EnumeratedPowerLevels = new List<string> { "LOW", "MEDIUM", "HIGH" },
                IntegralPowerLevels = new List<int>(),
                SupportedCookingModes = new List<string> { "TIMECOOK" }, // DEFROST, REHEAT,
                SupportsRemoteStart = true,
            };
        }

        private void PopulatePropertyIfSupported(Capability capability, DeviceDto device, string property)
        {
            // Ensire device has a field named, labeled or tagged with the property name
            if (device.HasField(property))
            {
                LambdaLogger.Log("Device '" + device.DisplayName + "' supports property: " + property);
                capability.Properties.Supported.Add(new Supported { Name = property });
            }
            else
            {
                LambdaLogger.Log("Device '" + device.DisplayName + "' missing capability property: " + property);
            }
        }

        private void PopulateEndpointHealthCapability(Capability capability, DeviceDto device)
        {
            capability.Properties.Supported.Add(new Supported { Name = "connectivity" });
        }

        private DisplayCategories GetDisplayCategory(string tag)
        {
            switch (tag)
            {
                case "ActivityTrigger":
                    return DisplayCategories.ACTIVITY_TRIGGER;
                case "Camera":
                    return DisplayCategories.CAMERA;
                case "Door":
                    return DisplayCategories.DOOR;
                case "Light":
                    return DisplayCategories.LIGHT;
                case "Other":
                    return DisplayCategories.OTHER;
                case "SceneTrigger":
                    return DisplayCategories.SCENE_TRIGGER;
                case "SmartLock":
                    return DisplayCategories.SMARTLOCK;
                case "SmartPlug":
                    return DisplayCategories.SMARTPLUG;
                case "Speaker":
                    return DisplayCategories.SPEAKER;
                case "Switch":
                    return DisplayCategories.SWITCH;
                case "TemperatureSensor":
                    return DisplayCategories.TEMPERATURE_SENSOR;
                case "Thermostat":
                    return DisplayCategories.THERMOSTAT;
                case "TV":
                    return DisplayCategories.TV;
                default:
                    return DisplayCategories.OTHER;
            }
        }

    }
}