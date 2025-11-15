using System;

namespace Crosshair10
{
    [Serializable] // needed for XmlSerializer
    public class CrosshairPreset
    {
        public string Name { get; set; } = "New";
        public double Size { get; set; }
        public double Thickness { get; set; }
        public double Gap { get; set; }
        public double OutlineThickness { get; set; }
        public string ColorHex { get; set; } = "#00FF00";
    }
}