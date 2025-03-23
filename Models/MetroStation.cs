using System;
using System.Collections.Generic;

namespace LivInParis.Models
{
    public class MetroStation
    {
        public string Name { get; set; }
        public List<MetroLine> Lines { get; set; } = new List<MetroLine>();
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public MetroStation(string name, double latitude = 0, double longitude = 0)
        {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
        }

        public void AddLine(MetroLine line)
        {
            if (!Lines.Contains(line))
            {
                Lines.Add(line);
            }
        }

        public override string ToString()
        {
            return $"Station: {Name} - Lines: {string.Join(", ", Lines)}";
        }
    }
}
