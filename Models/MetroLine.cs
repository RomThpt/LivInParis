using System;
using System.Collections.Generic;
using System.Linq;

namespace LivInParis.Models
{
    public class MetroLine
    {
        public string Number { get; set; }
        public string Color { get; set; }
        public List<MetroStation> Stations { get; set; } = new List<MetroStation>();

        public MetroLine(string number, string color = "")
        {
            Number = number;
            Color = color;
        }

        public void AddStation(MetroStation station)
        {
            if (!Stations.Contains(station))
            {
                Stations.Add(station);
                station.AddLine(this);
            }
        }

        public override string ToString()
        {
            return Number;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not MetroLine other)
                return false;
            return Number == other.Number;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }
    }
}
