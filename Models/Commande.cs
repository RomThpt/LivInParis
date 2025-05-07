using System;
using System.Collections.Generic;

namespace LivInParis.Models
{
    /// <summary>
    /// Représente une commande de repas
    /// </summary>
    public enum StatutCommande
    {
        EnAttente,
        Confirmee,
        EnPreparation,
        EnLivraison,
        Livree,
        Annulee
    }

    public class Commande
    {
        public int Id { get; set; }
        public int ClientID { get; set; }
        public DateTime DateCommande { get; set; }
        public DateTime DateLivraisonPrevue { get; set; }
        public decimal PrixTotal { get; set; }
        public StatutCommande Statut { get; set; }
        public string? AdresseLivraison { get; set; }
        public string? Commentaires { get; set; }

        // Navigation properties
        public Client? Client { get; set; }
        public List<CommandeItem> Items { get; set; } = new List<CommandeItem>();
        public Livraison? Livraison { get; set; }

        public Commande()
        {
            DateCommande = DateTime.Now;
            Statut = StatutCommande.EnAttente;
        }
    }

    /// <summary>
    /// Représente un item dans une commande
    /// </summary>
    public class CommandeItem
    {
        public int Id { get; set; }
        public int CommandeID { get; set; }
        public int PlatID { get; set; }
        public int Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }

        // Navigation properties
        public Commande? Commande { get; set; }
        public Plat? Plat { get; set; }
    }
}