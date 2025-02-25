namespace LivInParis.Models;

public class Graphe
{
    public Dictionary<int, Noeud> Noeuds { get; private set; }
    public List<Lien> Liens { get; private set; }
    public bool[,]? MatriceAdjacence { get; private set; }
    public RepresentationMode Mode { get; private set; }

    public Graphe(RepresentationMode mode)
    {
        Noeuds = new Dictionary<int, Noeud>();
        Liens = new List<Lien>();
        Mode = mode;
    }

    public void AjouterNoeud(int id)
    {
        if (!Noeuds.ContainsKey(id))
        {
            Noeuds.Add(id, new Noeud(id));
        }
    }

    public void AjouterLien(int id1, int id2)
    {
        if (Mode == RepresentationMode.Liste)
        {
            AjouterNoeud(id1);
            AjouterNoeud(id2);

            Noeud n1 = Noeuds[id1];
            Noeud n2 = Noeuds[id2];

            Lien nouveauLien = new Lien(n1, n2);

            if (!Liens.Contains(nouveauLien))
            {
                Liens.Add(nouveauLien);
                n1.AjouterLien(nouveauLien);
                n2.AjouterLien(nouveauLien);
            }
        }
        else // Matrice mode
        {
            AjouterNoeud(id1);
            AjouterNoeud(id2);

            int maxId = Math.Max(id1, id2);
            if (MatriceAdjacence == null)
            {
                MatriceAdjacence = new bool[maxId + 1, maxId + 1];
            }
            else if (MatriceAdjacence.GetLength(0) <= maxId)
            {
                int newSize = maxId + 1;
                bool[,] newMatrice = new bool[newSize, newSize];
                int oldSize = MatriceAdjacence.GetLength(0);
                for (int i = 0; i < oldSize; i++)
                    for (int j = 0; j < oldSize; j++)
                        newMatrice[i, j] = MatriceAdjacence[i, j];
                MatriceAdjacence = newMatrice;
            }

            MatriceAdjacence[id1, id2] = true;
            MatriceAdjacence[id2, id1] = true;
        }
    }

    private IEnumerable<int> GetVoisins(int noeudId)
    {
        if (Mode == RepresentationMode.Liste)
        {
            return Noeuds[noeudId].Voisins.Select(v => v.Id);
        }
        else
        {
            List<int> voisins = new List<int>();
            if (MatriceAdjacence != null && noeudId < MatriceAdjacence.GetLength(0))
                for (int j = 0; j < MatriceAdjacence.GetLength(0); j++)
                    if (MatriceAdjacence[noeudId, j])
                        voisins.Add(j);
            return voisins;
        }
    }

    public void ParcoursLargeur(int depart)
    {
        if (!Noeuds.ContainsKey(depart)) return;

        var visites = new HashSet<int>();
        var file = new Queue<int>();
        file.Enqueue(depart);
        visites.Add(depart);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Parcours en largeur:");
        while (file.Count > 0)
        {
            int current = file.Dequeue();
            Console.Write($"{current} ");
            foreach (var voisin in GetVoisins(current))
                if (visites.Add(voisin))
                    file.Enqueue(voisin);
        }
        Console.WriteLine();
        Console.ResetColor();
    }

    public void ParcoursProfondeur(int depart)
    {
        HashSet<int> visites = new HashSet<int>();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Parcours en profondeur:");
        ParcoursProfondeurRecursif(depart, visites);
        Console.WriteLine();
        Console.ResetColor();
    }

    private void ParcoursProfondeurRecursif(int noeud, HashSet<int> visites)
    {
        if (!Noeuds.ContainsKey(noeud) || visites.Contains(noeud)) return;
        visites.Add(noeud);
        Console.Write($"{noeud} ");
        foreach (var voisin in GetVoisins(noeud))
            ParcoursProfondeurRecursif(voisin, visites);
    }

    public bool EstConnexe()
    {
        if (Noeuds.Count == 0)
            return true;

        HashSet<int> visites = new HashSet<int>();
        ParcoursProfondeurRecursif(Noeuds.Keys.First(), visites);

        return visites.Count == Noeuds.Count;
    }

    public bool ContientCycle()
    {
        HashSet<int> visites = new HashSet<int>();
        HashSet<int> recursionStack = new HashSet<int>();

        foreach (var noeud in Noeuds.Keys)
        {
            if (DetecteCycleRecursif(noeud, visites, recursionStack))
                return true;
        }

        return false;
    }

    private bool DetecteCycleRecursif(int noeud, HashSet<int> visites, HashSet<int> recursionStack)
    {
        if (recursionStack.Contains(noeud))
            return true;

        if (visites.Contains(noeud))
            return false;

        visites.Add(noeud);
        recursionStack.Add(noeud);

        foreach (var voisin in GetVoisins(noeud))
        {
            if (DetecteCycleRecursif(voisin, visites, recursionStack))
                return true;
        }

        recursionStack.Remove(noeud);
        return false;
    }

    public void AfficherProprietes()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\nPropriétés du graphe:");
        Console.WriteLine($"Nombre de noeuds: {Noeuds.Count}");
        Console.WriteLine($"Nombre de liens: {Liens.Count}");
        Console.WriteLine($"Mode de représentation: {Mode}");
        Console.ResetColor();
    }

    public static Graphe LoadFromMtxFile(string path, RepresentationMode mode)
    {
        var graphe = new Graphe(mode);
        foreach (var line in File.ReadLines(path))
        {
            if (line.StartsWith("%")) continue;
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) continue;
            if (int.TryParse(parts[0], out int id1) && int.TryParse(parts[1], out int id2))
                graphe.AjouterLien(id1 - 1, id2 - 1);
        }
        return graphe;
    }
}
