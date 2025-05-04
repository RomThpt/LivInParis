namespace LivInParis.Models;

using System.Globalization;
using System.Xml.Linq;

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
                var voisin = lien.Noeud1.Id.Equals(courant) ? lien.Noeud2.Id : lien.Noeud1.Id;
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

    public static Graphe<string> LoadFromXmlFile(string path, RepresentationMode mode)
    {
        var graphe = new Graphe<string>(mode);

        XDocument xml = XDocument.Load(path);

        // Parse all stations
        foreach (XElement stationElement in xml.Descendants("station"))
        {
            string id = stationElement.Attribute("id")?.Value ?? string.Empty;
            string nom = stationElement.Element("nom")?.Value ?? id;

            if (double.TryParse(stationElement.Element("latitude")?.Value,
                                CultureInfo.InvariantCulture, out double latitude) &&
                double.TryParse(stationElement.Element("longitude")?.Value,
                                CultureInfo.InvariantCulture, out double longitude))
            {
                graphe.AjouterNoeud(id);
                graphe.Noeuds[id].Latitude = latitude;
                graphe.Noeuds[id].Longitude = longitude;
                graphe.Noeuds[id].Nom = nom;
            }
        }

        // Parse services to create connections
        foreach (XElement service in xml.Descendants("service"))
        {
            foreach (XElement trajet in service.Elements("trajet"))
            {
                var stations = trajet.Elements("station")
                    .Select(s => s.Attribute("ref-id")?.Value)
                    .Where(id => id != null)
                    .ToList();

                for (int i = 0; i < stations.Count - 1; i++)
                {
                    // Calculate distance between stations using Haversine formula for edge weight
                    var station1 = graphe.Noeuds[stations[i]!];
                    var station2 = graphe.Noeuds[stations[i + 1]!];
                    double distance = GeoCalculateur.CalculerDistanceHaversine(
                        station1.Latitude, station1.Longitude,
                        station2.Latitude, station2.Longitude);

                    graphe.AjouterLien(stations[i]!, stations[i + 1]!, distance);
                }
            }
        }

        return graphe;
    }

    public Dictionary<T, int> ColorerGraphe()
    {
        var couleurs = new Dictionary<T, int>();
        var noeudsOrdonnes = new List<T>(Noeuds.Keys);

        // Trier les noeuds par degré décroissant (nombre de connexions)
        noeudsOrdonnes.Sort((a, b) =>
            GetVoisins(b).Count().CompareTo(GetVoisins(a).Count()));

        foreach (var noeud in noeudsOrdonnes)
        {
            // Ensemble des couleurs déjà utilisées par les voisins
            var couleursVoisins = new HashSet<int>();
            foreach (var voisin in GetVoisins(noeud))
            {
                if (couleurs.ContainsKey(voisin))
                    couleursVoisins.Add(couleurs[voisin]);
            }

            // Trouver la première couleur disponible
            int couleur = 0;
            while (couleursVoisins.Contains(couleur))
                couleur++;

            couleurs[noeud] = couleur;
        }

        return couleurs;
    }

    public void AfficherColoration()
    {
        var coloration = ColorerGraphe();
        var nombreCouleurs = coloration.Values.Max() + 1;

        Console.WriteLine($"Coloration du graphe avec {nombreCouleurs} couleurs:");

        // Grouper les noeuds par couleur
        var noeudsParCouleur = new Dictionary<int, List<T>>();
        foreach (var kvp in coloration)
        {
            if (!noeudsParCouleur.ContainsKey(kvp.Value))
                noeudsParCouleur[kvp.Value] = new List<T>();

            noeudsParCouleur[kvp.Value].Add(kvp.Key);
        }

        // Afficher les noeuds regroupés par couleur
        foreach (var couleur in Enumerable.Range(0, nombreCouleurs))
        {
            Console.ForegroundColor = (ConsoleColor)(couleur % 15 + 1);
            Console.Write($"Couleur {couleur}: ");

            if (noeudsParCouleur.ContainsKey(couleur))
            {
                Console.WriteLine(string.Join(", ", noeudsParCouleur[couleur]));
            }
            else
            {
                Console.WriteLine("(aucun noeud)");
            }
        }

        Console.ResetColor();
    }

    public List<Lien<T>> TrouverCircuitEulerien()
    {
        // Vérifier si le graphe est connexe
        if (!EstConnexe())
            throw new InvalidOperationException("Le graphe doit être connexe pour trouver un circuit eulérien.");

        // Identifier les noeuds de degré impair
        var noeudsImpairs = Noeuds.Values
            .Where(n => n.Liens.Count % 2 != 0)
            .Select(n => n.Id)
            .ToList();

        if (noeudsImpairs.Count > 0 && noeudsImpairs.Count != 2)
            throw new InvalidOperationException("Le graphe doit être eulérien ou semi-eulérien.");

        // Point de départ: premier noeud avec degré impair ou n'importe quel noeud si tous les degrés sont pairs
        T depart = noeudsImpairs.Count == 2 ? noeudsImpairs[0] : Noeuds.Keys.First();

        // Créer une copie des liens pour marquer ceux qu'on a visités
        var liensRestants = new List<Lien<T>>(Liens);
        var circuit = new List<Lien<T>>();
        var currentNoeud = depart;

        while (liensRestants.Count > 0)
        {
            // Trouver un lien adjacent au noeud courant
            var lien = liensRestants.FirstOrDefault(l =>
                l.Noeud1.Id.Equals(currentNoeud) || l.Noeud2.Id.Equals(currentNoeud));

            if (lien != null)
            {
                liensRestants.Remove(lien);
                circuit.Add(lien);

                // Passer au noeud suivant
                currentNoeud = lien.Noeud1.Id.Equals(currentNoeud) ? lien.Noeud2.Id : lien.Noeud1.Id;
            }
            else
            {
                // Si on est bloqué, cela signifie qu'on a un sous-circuit - on doit explorer un autre chemin
                // Cette implémentation simplifiée peut ne pas trouver le circuit optimal
                // L'algorithme complet du facteur chinois serait nécessaire pour un graphe non eulérien
                break;
            }
        }

        return circuit;
    }

    public double CalculerLongueurTotaleCircuit(List<Lien<T>> circuit)
    {
        return circuit.Sum(lien => lien.Poids);
    }

    public void AfficherCircuitEulerien()
    {
        try
        {
            var circuit = TrouverCircuitEulerien();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Circuit eulérien trouvé:");

            var itineraire = new List<T>();
            T noeudPrecedent = default;

            foreach (var lien in circuit)
            {
                if (itineraire.Count == 0)
                {
                    itineraire.Add(lien.Noeud1.Id);
                    noeudPrecedent = lien.Noeud2.Id;
                }
                else
                {
                    if (lien.Noeud1.Id.Equals(noeudPrecedent))
                    {
                        itineraire.Add(noeudPrecedent);
                        noeudPrecedent = lien.Noeud2.Id;
                    }
                    else
                    {
                        itineraire.Add(noeudPrecedent);
                        noeudPrecedent = lien.Noeud1.Id;
                    }
                }
            }

            // Ajouter le dernier noeud pour compléter le circuit
            itineraire.Add(noeudPrecedent);

            Console.WriteLine(string.Join(" → ", itineraire));
            Console.WriteLine($"Longueur totale: {CalculerLongueurTotaleCircuit(circuit)}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Impossible de trouver un circuit eulérien: {ex.Message}");
            Console.ResetColor();
        }
    }
}
