using System;
using System.Collections.Generic;

namespace LivInParis.Models;

// Classe non générique existante pour la rétrocompatibilité
public class Noeud
{
    public int Id { get; private set; }
    public List<Lien> Liens { get; private set; }

    public Noeud(int id)
    {
        Id = id;
        Liens = new List<Lien>();
    }

    public void AjouterLien(Lien lien)
    {
        if (!Liens.Contains(lien))
        {
            Liens.Add(lien);
        }
    }

    public List<Noeud> Voisins
    {
        get
        {
            List<Noeud> voisins = new List<Noeud>();
            foreach (var lien in Liens)
            {
                if (lien.Source == this)
                    voisins.Add(lien.Destination);
                else
                    voisins.Add(lien.Source);
            }
            return voisins;
        }
    }
}

// Nouvelle classe de nœud générique
public class Noeud<T> where T : IEquatable<T>
{
    public T Id { get; }
    public List<Lien<T>> OutgoingEdges { get; }

    public Noeud(T id)
    {
        Id = id;
        OutgoingEdges = new List<Lien<T>>();
    }

    public void AddEdge(Lien<T> edge)
    {
        if (!OutgoingEdges.Contains(edge))
            OutgoingEdges.Add(edge);
    }
}
