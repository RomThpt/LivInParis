using System;
using System.Collections.Generic;

namespace LivInParis.Models
{
    public class Cuisinier
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? Email { get; set; }
        public string? Adresse { get; set; }
        public float NoteMoyenne { get; set; }
        public DateTime DateInscription { get; set; }

        // Navigation properties
        public List<Plat> Plats { get; set; } = new List<Plat>();
    }
}