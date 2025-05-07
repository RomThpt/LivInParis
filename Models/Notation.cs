using System;

namespace LivInParis.Models
{
    /// <summary>
    /// Représente une notation donnée par un client à un cuisinier
    /// </summary>
    public enum TypeNotation
    {
        ClientVersCuisinier,
        CuisinierVersClient
    }

    public class Notation
    {
        public int Id { get; set; }
        public int CommandeID { get; set; }
        public int NoteurID { get; set; }
        public int NoteID { get; set; }
        public TypeNotation Type { get; set; }
        public int Note { get; set; } // 1-5 stars
        public string? Commentaire { get; set; }
        public DateTime DateNotation { get; set; }

        // Navigation property
        public Commande? Commande { get; set; }

        public Notation()
        {
            DateNotation = DateTime.Now;
        }
    }
}