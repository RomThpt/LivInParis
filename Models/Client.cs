using System;
using System.Collections.Generic;

namespace LivInParis.Models
{
    public enum TypeClient
    {
        Individuel,
        EntrepriseLocale
    }

    public class Client
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public string? Adresse { get; set; }
        public TypeClient Type { get; set; }
        public float NoteMoyenne { get; set; }
        public DateTime DateInscription { get; set; }

        // Navigation properties
        public List<Commande> Commandes { get; set; } = new List<Commande>();
    }
}