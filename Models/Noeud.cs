namespace LivInParis.Models;

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
                if (lien.Noeud1 == this)
                    voisins.Add(lien.Noeud2);
                else
                    voisins.Add(lien.Noeud1);
            }
            return voisins;
        }
    }
}
