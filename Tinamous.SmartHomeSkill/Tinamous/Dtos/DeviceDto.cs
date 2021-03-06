﻿using System.Collections.Generic;

namespace Tinamous.SmartHome.Tinamous.Dtos
{
    /// <summary>
    /// Device definition from Tinamous Device API.
    /// </summary>
    /// <see cref="http://tinamous.com/ApiDocs/ResourceModel?modelName=DeviceDto"/>
    public class DeviceDto
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public List<FieldDescriptorDto> FieldDescriptors { get; set; }
        public bool Connected { get; set; }
        public bool IsReporting { get; set; }
        public List<string> Tags { get; set; }
        public List<MetaTagDto> MetaTags { get; set; }
    }
}