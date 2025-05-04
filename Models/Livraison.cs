using System;

namespace LivInParis.Models
{
    public enum StatutLivraison
    {
        Planifiee,
        EnCours,
        Terminee,
        Annulee
    }

    public class Livraison
    {
        public int Id { get; set; }
        public int CommandeID { get; set; }
        public DateTime DateDepart { get; set; }
        public DateTime? DateArrivee { get; set; }
        public string? ItineraireSuggere { get; set; }
        public StatutLivraison Statut { get; set; }
        public string? CommentaireLivreur { get; set; }

        // Navigation property
        public Commande? Commande { get; set; }
    }
}