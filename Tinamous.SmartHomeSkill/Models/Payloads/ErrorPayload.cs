﻿using Newtonsoft.Json;

namespace Tinamous.SmartHome.Models.Payloads
{
    /// <summary>
    /// See https://developer.amazon.com/docs/device-apis/alexa-errorresponse.html
    /// </summary>
    public class ErrorPayload : Payload
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}