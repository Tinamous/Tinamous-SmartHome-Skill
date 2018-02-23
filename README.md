# Tinamous SmartHomeSkill for Amazon Alexa.

Generic implementation to enable Tinamous devices to interact with Amazon Alexa through the SmartHome skill.

* Create a Lambda function
** Note: This needs to be in an appropriate region for your skill language. i.e. English (UK) needs to be in EU-West (Ireland) region.
* Upload this assembly (Use AWS tools and right click on the project to upload).
* Entry point (Handler) should be
** Tinamous.SmartHome::Tinamous.SmartHome.SmartHomeSkill::FunctionHandler
* Runtime C# (.NET Core 2.0)
* Publish a new version
** Note the ARN (top right in AWS Lambda) - be sure to pick the versioned ARN.
* In the Skill developer portal (https://developer.amazon.com)
* Create a new skill, use the V3 Smart Home format
* Set the AWS Lamdba ARN to the lambda just created.
** Also set the appropriate geographical region (i.e. Europe/India).
* Configure Account Linking
** In Tinamous, add a "Alexa Bot", follow the instructions and endpoints on there for account linking.
* On the front (dashboard) page of skills, copy the Skill ID
* Back in Lambda...
** Add the Alexa Smart Home trigger
** Set the Application ID to the Skill ID from your skill.
** Click to the Monitoring tab, then Jump to Logs.
*** If you get a "Log group not found" be sure you've given the execution role cloud watch permissions.


Head on over to Alexa.Amazon.co.uk (or .com I guess), add the skill:
* Skills
* My Skills
* Hopefully it shows up eventually in the dev section.
* Enable the skill and link to your tinamous account
** Enter the Tinamous account name, that's the <AccountName>.Tinamous.com bit you use when you navigate to Tinamous for your devices.
** Enter your username and password. Alexa will post messages as you so you may prefer to create a new member (called Alexa?) to do them instead.




When you use phrases such as, "Alexa, Turn on the kettle", the Tinamous SmartHome skill will post a status message "@Kettle turn on".
Your device should use an MQTT connection to receive these status message and then process the "turn on" command.

Updates should be pushed back to Tinamous through MQTT in the form of field values, when the device is on, the appropriate field should be set to true.

## Development tips

* If you use different email / amazon accounts for day to day alexa, and development, then you can use "Alexa, switch profile" to switch between normal and dev.

## Configure Tinamous Devices.

* Tag the device with "Alexa.SmartDevice"
* Tag with the Alexa Interface the device supports (e.g. Alexa.PowerController)
* Implement the required status message (or in future named command) processing.
* Expose properties required by the Alexa Interface
** Use tags or named fields to match the required state property.

When Alexa runs device discover the interface will not add supported properties if the fields are not found.

For devices that have multple ports/outlets

* Tag with MultiPort
* Add the state variable (MetaTag) Name = 'PortCount' Value = the number of ports. (e.g. PortCount,4)
* For each port you wish to add to Alexa's devices add a Name = "Port-n", Value = Name of the device on the port. (e.g. Port-1,LED Lights )
* Ports without a name are ignored.
* Properties for the interfaces exposed by the device should be tagged with the suffix '-port-n' (e.g. powerState-port-1)
* When quering for properties if the port specific property is not found, the non-port field will be queries.

Run smart home discovery on Alexa. Your devices should be listed.

### Supported Namepspace Interfaces:

#### Alexa.TemperatureSensor

Directives: None
Properties: temperature

Note: Currently only celcius is supported for temperature.

#### Alexa.PercentageController

Directives: SetPercentage, AdjustPercentage
Properties: percentage

Status Posts: "@Device Set percentage 90"
Status Posts: "@Device Set percentage 90 port-1"

Status Posts: "@Device Adjust percentage 10"
Status Posts: "@Device Adjust percentage 10 port-1"

Utterance: "Alexa, set <> to number percent"

Note: If you also support Alexa.Brightness percentage requests come in as brightness.

#### Alexa.BrightnessController

Directives: SetBrightness, AdjustBrightness
Properties: brightness

Status Posts: "@Device Set brightness 90"
Status Posts: "@Device Set brightness 90 port-1"

Status Posts: "@Device Adjust brightness 10"
Status Posts: "@Device Adjust brightness 10 port-1"

Utterance: "Alexa, set the device to <>"
Utterance: "Alexa, dim device <>"

#### Alexa.ColorController

Directives: SetColor
Properties: color

Status Posts: "@Device Set color HSV 240,0.5,0.1
Status Posts: "@Device Set color HSV 240,0.5,0.1 port-1

Note: color property should be "h,s,v" string value (or h,s,b) - hue, saturation, brightness. (e.g. 200,0.1,0.9)

Note: Report the color property only when the bulb is set to an HSB color

#### Alexa.PowerController

Directives: TurnOn, TurnOff
Properties: powerState (powerState-port-1)

Property can be boolean (on/off), or string "ON", "OFF" or numeric (>0 for on, 0 for off)

Status Posts: "@Device Turn On"
Status Posts: "@Device Turn On port-1"

Status Posts: "@Device Turn Off"
Status Posts: "@Device Turn Of port-1"

#### Alexa.PowerLevelController

Directives: SetPowerLevel, AdjustPowerLevel
Properties: powerLevel

Status Posts: "@Device Set powerlevel 20"
Status Posts: "@Device Set powerlevel 20 port-1"

Status Posts: "@Device Adjust powerlevel 10"
Status Posts: "@Device Adjust powerlevel 10 port-1"

Note: If you also support Alexa.Brightness power level requests come in as brightness.

#### Alexa.LockController - Not Implemented

Directives:
Properties: lockState

Status Posts: "@Device 

#### lexa.SceneController - Not Implemented

Directives:
Properties: None

Status Posts: "@Device 

#### Alexa.ThermostatController - Not Implemented

Directives:
Properties: lowerSetpoint,targetSetpoint,upperSetpoint,thermostatMode

Status Posts: "@Device 


# Use Case:
# BOFF - Box of Four Fans - AWS Lambda functions for fan control using Alexa SmartHome Skill.

## Interactions

### Fan and LED control.

"Alexa, switch on [The Fans]" - Working

"Alexa, switch off [The Fans]" - Working

### Fan Control

"Alexa, set [The Fans] to number percent""

"Alexa, set [The Fans] power to x" ??? (Power level)

### LED control

"Alexa, dim [The Fans] 20%""

"Alexa, set the [The Fans] to 80%" -- this will clash with number percent fan control?

"Alexa, set the [The Fans] to [blue]"

## Command messages needed for boff

* Turn On
* Turn Off
* Fan Speed x (%)

* Dim Leds by x
* Set Leds to  x

* Set Color to ...,...,...,...

## Parameters needed from boff

See StateReport and discovery capabilities.

### Fan Parameters

* powerState (Power Controller) - fan only (leds done by brightness) (Field: masterPower 1 or 0.)
* powerLevel (Power Level Controller) (0..100) - fan (Field: speedPercentage)

### LED Parameters

* brightness (Brightness Controller) - leds only. (0..100) (Field: ledBrightness 0..100)
* percentage (Percentage controller - might clash with )
* color (not sure....)

### Other...

* temperature (in °C)

## Setup

Visual Studio Setup:

Install AWS Toolkit: https://aws.amazon.com/visualstudio/

Setup the AWS Explorer with IAM credentials.

// See: https://github.com/DamianMehers/AlexaSmartHomeDemo

// NB: To add Amazon.Lambda.Tools use... dotnet add package Amazon.Lambda.Tools (fails with NuGet)

## Deploy

To Update a new version

* Use the Publish To AWS Lambda (right click on project) to upload
* Create a new version of the Lambda, copy the arn.
* Paste arn into skill.
* Copy Skill Id
* Add Alexa Smart Home trigger to Lambda
* Paste skill it.
* Save!

## References



Alexa Developer Console: 
https://developer.amazon.com/home.html


Alexa SmartHome Documentation:

This application is using Alexa V3 Smarthome API.
	
Alexa.TemperatureSensor
https://developer.amazon.com/docs/device-apis/alexa-temperaturesensor.html

Alexa.PercentageController
https://developer.amazon.com/docs/device-apis/alexa-percentagecontroller.html


Alexa.BrightnessController
https://developer.amazon.com/docs/device-apis/alexa-brightnesscontroller.html

Alexa.ColorController
https://developer.amazon.com/docs/device-apis/alexa-colorcontroller.html


Alexa.PowerController
https://developer.amazon.com/docs/device-apis/alexa-powercontroller.html

Alexa.PowerLevelController
https://developer.amazon.com/docs/device-apis/alexa-powerlevelcontroller.html


Sample Messages: https://github.com/alexa/alexa-smarthome/tree/master/sample_messages