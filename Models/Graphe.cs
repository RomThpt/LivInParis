namespace LivInParis.Models;

public class Graphe<T> where T : IEquatable<T>
{
    public Dictionary<T, Noeud<T>> Noeuds { get; private set; }
    public List<Lien<T>> Liens { get; private set; }
    public Dictionary<T, Dictionary<T, bool>>? MatriceAdjacence { get; private set; }
    public RepresentationMode Mode { get; private set; }

    public Graphe(RepresentationMode mode)
    {
        Noeuds = new Dictionary<T, Noeud<T>>();
        Liens = new List<Lien<T>>();
        Mode = mode;
        if (mode == RepresentationMode.Matrice)
        {
            MatriceAdjacence = new Dictionary<T, Dictionary<T, bool>>();
        }
    }

    public void AjouterNoeud(T id)
    {
        if (!Noeuds.ContainsKey(id))
        {
            Noeuds.Add(id, new Noeud<T>(id));

            if (Mode == RepresentationMode.Matrice && MatriceAdjacence != null)
            {
                MatriceAdjacence[id] = new Dictionary<T, bool>();
            }
        }
    }

    public void AjouterLien(T id1, T id2, double poids = 1.0)
    {
        if (Mode == RepresentationMode.Liste)
        {
            AjouterNoeud(id1);
            AjouterNoeud(id2);

            Noeud<T> n1 = Noeuds[id1];
            Noeud<T> n2 = Noeuds[id2];

            Lien<T> nouveauLien = new Lien<T>(n1, n2, poids);

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

            if (MatriceAdjacence != null)
            {
                if (!MatriceAdjacence[id1].ContainsKey(id2))
                    MatriceAdjacence[id1][id2] = true;

                if (!MatriceAdjacence[id2].ContainsKey(id1))
                    MatriceAdjacence[id2][id1] = true;

                // Également ajouter le lien pour garder la cohérence
                Noeud<T> n1 = Noeuds[id1];
                Noeud<T> n2 = Noeuds[id2];
                Lien<T> nouveauLien = new Lien<T>(n1, n2, poids);

                if (!Liens.Contains(nouveauLien))
                {
                    Liens.Add(nouveauLien);
                }
            }
        }
    }

    private IEnumerable<T> GetVoisins(T noeudId)
    {
        if (Mode == RepresentationMode.Liste)
        {
            return Noeuds[noeudId].Voisins.Select(v => v.Id);
        }
        else if (MatriceAdjacence != null)
        {
            return MatriceAdjacence[noeudId].Where(kvp => kvp.Value).Select(kvp => kvp.Key);
        }
        return Enumerable.Empty<T>();
    }

    public void ParcoursLargeur(T depart)
    {
        if (!Noeuds.ContainsKey(depart)) return;

        var visites = new HashSet<T>();
        var file = new Queue<T>();
        file.Enqueue(depart);
        visites.Add(depart);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Parcours en largeur:");
        while (file.Count > 0)
        {
            T current = file.Dequeue();
            Console.Write($"{current} ");
            foreach (var voisin in GetVoisins(current))
                if (visites.Add(voisin))
                    file.Enqueue(voisin);
        }
        Console.WriteLine();
        Console.ResetColor();
    }

    public void ParcoursProfondeur(T depart)
    {
        HashSet<T> visites = new HashSet<T>();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Parcours en profondeur:");
        ParcoursProfondeurRecursif(depart, visites);
        Console.WriteLine();
        Console.ResetColor();
    }

    private void ParcoursProfondeurRecursif(T noeud, HashSet<T> visites)
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

        HashSet<T> visites = new HashSet<T>();
        ParcoursProfondeurRecursif(Noeuds.Keys.First(), visites);

        return visites.Count == Noeuds.Count;
    }

    public bool ContientCycle()
    {
        HashSet<T> visites = new HashSet<T>();
        HashSet<T> recursionStack = new HashSet<T>();

        foreach (var noeud in Noeuds.Keys)
        {
            if (DetecteCycleRecursif(noeud, visites, recursionStack))
                return true;
        }

        return false;
    }

    private bool DetecteCycleRecursif(T noeud, HashSet<T> visites, HashSet<T> recursionStack)
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

    public Dictionary<T, double> Dijkstra(T depart)
    {
        var distances = new Dictionary<T, double>();
        var predecesseurs = new Dictionary<T, T>();
        var nonVisites = new HashSet<T>();

        foreach (var noeud in Noeuds.Keys)
        {
            distances[noeud] = double.PositiveInfinity;
            nonVisites.Add(noeud);
        }

        distances[depart] = 0;

        while (nonVisites.Count > 0)
        {
            // Trouver le nœud non visité avec la plus petite distance
            T courant = default!;
            double minDistance = double.PositiveInfinity;

            foreach (var noeud in nonVisites)
            {
                if (distances[noeud] < minDistance)
                {
                    minDistance = distances[noeud];
                    courant = noeud;
                }
            }

            // Si aucun chemin n'est trouvé, on s'arrête
            if (minDistance == double.PositiveInfinity)
                break;

            nonVisites.Remove(courant);

            // Mettre à jour les distances pour les voisins
            foreach (var lien in Noeuds[courant].Liens)
            {
                var voisin = lien.Noeud1.Id!.Equals(courant) ? lien.Noeud2.Id : lien.Noeud1.Id;
                var nouvelleDistance = distances[courant] + lien.Poids;

                if (nouvelleDistance < distances[voisin])
                {
                    distances[voisin] = nouvelleDistance;
                    predecesseurs[voisin] = courant;
                }
            }
        }

        return distances;
    }

    public Dictionary<T, double> BellmanFord(T depart)
    {
        var distances = new Dictionary<T, double>();
        var predecesseurs = new Dictionary<T, T>();

        foreach (var noeud in Noeuds.Keys)
        {
            distances[noeud] = double.PositiveInfinity;
        }

        distances[depart] = 0;

        // Relaxation des arêtes |V|-1 fois
        for (int i = 0; i < Noeuds.Count - 1; i++)
        {
            foreach (var lien in Liens)
            {
                var u = lien.Noeud1.Id;
                var v = lien.Noeud2.Id;
                var poids = lien.Poids;

                if (distances[u] != double.PositiveInfinity && distances[u] + poids < distances[v])
                {
                    distances[v] = distances[u] + poids;
                    predecesseurs[v] = u;
                }

                // Pour les graphes non orientés, vérifier dans l'autre sens aussi
                if (distances[v] != double.PositiveInfinity && distances[v] + poids < distances[u])
                {
                    distances[u] = distances[v] + poids;
                    predecesseurs[u] = v;
                }
            }
        }

        // Vérifier les cycles de poids négatif
        foreach (var lien in Liens)
        {
            var u = lien.Noeud1.Id;
            var v = lien.Noeud2.Id;
            var poids = lien.Poids;

            if (distances[u] != double.PositiveInfinity && distances[u] + poids < distances[v])
            {
                Console.WriteLine("Le graphe contient un cycle de poids négatif");
                return new Dictionary<T, double>();
            }

            if (distances[v] != double.PositiveInfinity && distances[v] + poids < distances[u])
            {
                Console.WriteLine("Le graphe contient un cycle de poids négatif");
                return new Dictionary<T, double>();
            }
        }

        return distances;
    }

    public List<T> ReconstruireChemin(T depart, T fin, Dictionary<T, T> predecesseurs)
    {
        var chemin = new List<T>();

        if (!predecesseurs.ContainsKey(fin))
            return chemin;

        var courant = fin;
        while (!courant.Equals(depart))
        {
            chemin.Add(courant);
            if (!predecesseurs.TryGetValue(courant, out courant))
                return new List<T>(); // Pas de chemin trouvé
        }

        chemin.Add(depart);
        chemin.Reverse();
        return chemin;
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

    public static Graphe<int> LoadFromMtxFile(string path, RepresentationMode mode)
    {
        var graphe = new Graphe<int>(mode);
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
