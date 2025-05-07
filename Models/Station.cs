using System;

namespace LivInParis.Models
{
    public class Station
    {
        public string Id { get; set; }
        public string Nom { get; set; }
        public string Adresse { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Ligne { get; set; }

        public Station()
        {
        }

        public Station(string id, string nom, string adresse, float latitude, float longitude, string ligne = "1")
        {
            Id = id;
            Nom = nom;
            Adresse = adresse;
            Latitude = latitude;
            Longitude = longitude;
            Ligne = ligne;
        }
    }
}