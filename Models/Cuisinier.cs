using System;
using System.Collections.Generic;

namespace LivInParis.Models
{
    /// <summary>
    /// Représente un cuisinier dans le système
    /// </summary>
    public class Cuisinier
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Adresse { get; set; }
        public string Specialite { get; set; }
        public decimal NoteMoyenne { get; set; }
        public List<Plat> Plats { get; set; }

        public Cuisinier()
        {
            Plats = new List<Plat>();
            NoteMoyenne = 0;
        }
    }
}