namespace LivInParis.Models;

public class Lien<T> where T : IEquatable<T>
{
    public Noeud<T> Noeud1 { get; private set; }
    public Noeud<T> Noeud2 { get; private set; }
    public double Poids { get; private set; }

    public Lien(Noeud<T> n1, Noeud<T> n2, double poids = 1.0)
    {
        Noeud1 = n1;
        Noeud2 = n2;
        Poids = poids;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Lien<T> autre)
        {
            return (Noeud1.Id!.Equals(autre.Noeud1.Id) && Noeud2.Id!.Equals(autre.Noeud2.Id)) ||
                   (Noeud1.Id!.Equals(autre.Noeud2.Id) && Noeud2.Id!.Equals(autre.Noeud1.Id));
        }
        return false;
    }

    public override int GetHashCode()
    {
        var id1Hash = Noeud1.Id!.GetHashCode();
        var id2Hash = Noeud2.Id!.GetHashCode();
        return HashCode.Combine(Math.Min(id1Hash, id2Hash), Math.Max(id1Hash, id2Hash));
    }
}
