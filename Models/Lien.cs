using System;

namespace LivInParis.Models;

// Classe non générique existante pour la rétrocompatibilité
public class Lien
{
    public Noeud Source { get; }
    public Noeud Destination { get; }
    public double Poids { get; }

    public Lien(Noeud source, Noeud destination, double poids = 1.0)
    {
        Source = source;
        Destination = destination;
        Poids = poids;
    }

    public override bool Equals(object? obj) =>
        obj is Lien other &&
        Source.Id == other.Source.Id &&
        Destination.Id == other.Destination.Id;

    public override int GetHashCode() =>
        HashCode.Combine(Source.Id, Destination.Id);
}

// Nouvelle classe de lien générique
public class Lien<T> where T : IEquatable<T>
{
    public Noeud<T> Source { get; }
    public Noeud<T> Target { get; }
    public double Weight { get; }

    public Lien(Noeud<T> source, Noeud<T> target, double weight = 1.0)
    {
        Source = source;
        Target = target;
        Weight = weight;
    }

    public override bool Equals(object? obj) =>
        obj is Lien<T> other &&
        Source.Id.Equals(other.Source.Id) &&
        Target.Id.Equals(other.Target.Id);

    public override int GetHashCode() =>
        HashCode.Combine(Source.Id, Target.Id);
}
