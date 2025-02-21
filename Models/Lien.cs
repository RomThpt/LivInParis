namespace LivInParis.Models;

public class Lien
{
    public Noeud Noeud1 { get; private set; }
    public Noeud Noeud2 { get; private set; }

    public Lien(Noeud n1, Noeud n2)
    {
        Noeud1 = n1;
        Noeud2 = n2;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Lien autre)
        {
            return (Noeud1.Id == autre.Noeud1.Id && Noeud2.Id == autre.Noeud2.Id) ||
                   (Noeud1.Id == autre.Noeud2.Id && Noeud2.Id == autre.Noeud1.Id);
        }
        return false;
    }

    public override int GetHashCode()
    {
        int min = Math.Min(Noeud1.Id, Noeud2.Id);
        int max = Math.Max(Noeud1.Id, Noeud2.Id);
        return HashCode.Combine(min, max);
    }
}
