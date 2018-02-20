using System.Collections.Generic;

namespace Tinamous.SmartHome.Tinamous.Dtos
{
    public class FieldDescriptorDto
    {
        public int Channel { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Unit { get; set; }
        public bool IsPrimary { get; set; }
        public List<string> Tags { get; set; }
    }
}