using LivInParis.Models;

string filePath = Path.Combine("public", "Association-soc-karate", "soc-karate.mtx");

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("***************************");
Console.WriteLine("********  Analyse  ********");
Console.WriteLine("***************************");
Console.ResetColor();

var grapheListe = Graphe<int>.LoadFromMtxFile(filePath, RepresentationMode.Liste);
AnalyzeGraph(grapheListe);

// Visualization
GraphVisualizer.Visualize(grapheListe, "karate_liste.png");
Console.WriteLine("\nkarate_liste.png genéré");

void AnalyzeGraph<T>(Graphe<T> g) where T : IEquatable<T>
{
    if (g.Noeuds.Count > 0)
    {
        T startNode = g.Noeuds.Keys.First();
        g.ParcoursLargeur(startNode);
        g.ParcoursProfondeur(startNode);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\nConnexe: {g.EstConnexe()}");
        Console.WriteLine($"Contient cycle: {g.ContientCycle()}");
        Console.ResetColor();
    }

    g.AfficherProprietes();
}

// Démonstration des nouveaux algorithmes de plus court chemin
if (grapheListe.Noeuds.Count > 0)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n***************************");
    Console.WriteLine("**** Plus courts chemins ****");
    Console.WriteLine("***************************");
    Console.ResetColor();

    int sourceNode = 0; // Premier nœud du graphe
    Console.WriteLine($"Calcul des plus courts chemins depuis le nœud {sourceNode}");

    // Dijkstra
    Console.WriteLine("\nRésultats de Dijkstra:");
    var distancesDijkstra = grapheListe.Dijkstra(sourceNode);
    foreach (var (node, distance) in distancesDijkstra.OrderBy(kvp => kvp.Value).Take(5))
    {
        Console.WriteLine($"  Distance au nœud {node}: {distance}");
    }

    // Bellman-Ford
    Console.WriteLine("\nRésultats de Bellman-Ford:");
    var distancesBF = grapheListe.BellmanFord(sourceNode);
    foreach (var (node, distance) in distancesBF.OrderBy(kvp => kvp.Value).Take(5))
    {
        Console.WriteLine($"  Distance au nœud {node}: {distance}");
    }
}
