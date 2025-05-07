using System;
using System.Collections.Generic;

namespace LivInParis.Models;

/// <summary>
/// Représente un nœud dans le graphe du réseau de métro
/// </summary>
public class Noeud<T> where T : IEquatable<T>
{
    public T Id { get; private set; }
    public List<Lien<T>> Liens { get; private set; }

    // Propriétés géographiques pour les applications de transport
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Nom { get; set; }

    public Noeud(T id)
    {
        Id = id;
        Liens = new List<Lien<T>>();
    }

    public void AjouterLien(Lien<T> lien)
    {
        if (!Liens.Contains(lien))
        {
            Liens.Add(lien);
        }
    }

    public List<Noeud<T>> Voisins
    {
        get
        {
            List<Noeud<T>> voisins = new List<Noeud<T>>();
            foreach (var lien in Liens)
            {
                if (lien.Noeud1.Id!.Equals(Id))
                    voisins.Add(lien.Noeud2);
                else
                    voisins.Add(lien.Noeud1);
            }
            return voisins;
        }
    }
}
