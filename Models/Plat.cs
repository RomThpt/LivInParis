using System;
using System.Collections.Generic;

namespace LivInParis.Models
{
    public enum CategoriePlat
    {
        Entree,
        PlatPrincipal,
        Dessert
    }

    public class Plat
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public CategoriePlat Categorie { get; set; }
        public int CuisinierID { get; set; }
        public string? NationaliteCuisine { get; set; }
        public List<string> IngredientsPhares { get; set; } = new List<string>();
        public List<string> RegimesAlimentaires { get; set; } = new List<string>(); // Végétarien, Sans Gluten, etc.
        public int NombrePersonnes { get; set; }
        public DateTime DatePreparation { get; set; }
        public DateTime DateExpiration { get; set; }
        public decimal PrixParPersonne { get; set; }
        public string? PhotoUrl { get; set; }
        public bool EstDisponible { get; set; } = true;

        // Navigation properties
        public Cuisinier? Cuisinier { get; set; }
        public List<CommandeItem> CommandeItems { get; set; } = new List<CommandeItem>();
    }
}