using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LivInParis.Models;
using LivInParis.Services;

Console.WriteLine("Bienvenue à Liv in Paris!");

// Création du service métro et chargement des données
var metroService = new MetroService();
// metroService.LoadSampleMetroData();
metroService.LoadMetroDataFromCsv("./public/MetroParis.csv");

// Obtenir toutes les stations et lignes
var stations = metroService.GetAllStations();
var lines = metroService.GetAllLines();

// Afficher les informations
Console.WriteLine($"\nNombre de stations de métro: {stations.Count}");
Console.WriteLine($"Nombre de lignes de métro: {lines.Count}");

// Construire le graphe du métro avec les positions géographiques
var (metroGraph, stationGeoPositions) = metroService.BuildMetroGraphWithPositions();

// Debug output to check stations data
Console.WriteLine("\nVérification des coordonnées des stations:");
bool hasValidCoordinates = false;

// Check a sample of station coordinates
int sampleSize = Math.Min(5, stations.Count);
for (int i = 0; i < sampleSize; i++)
{
    var station = stations[i];
    Console.WriteLine($"Station: {station.Name}, Lat: {station.Latitude}, Lon: {station.Longitude}");

    // Check if the station has valid coordinates
    if (station.Latitude != 0 && station.Longitude != 0)
    {
        hasValidCoordinates = true;
    }
}

// If no valid coordinates, generate artificial positions
if (!hasValidCoordinates)
{
    Console.WriteLine("Attention: Aucune coordonnée géographique valide détectée. Utilisation de coordonnées artificielles.");
    stationGeoPositions.Clear();

    // Generate artificial positions in a circle
    double centerLat = 48.8566; // Paris center latitude
    double centerLon = 2.3522;  // Paris center longitude
    double radius = 0.05;       // ~5km in decimal degrees

    int i = 0;
    foreach (var station in stations)
    {
        double angle = 2 * Math.PI * i / stations.Count;
        double lat = centerLat + radius * Math.Cos(angle);
        double lon = centerLon + radius * Math.Sin(angle);

        stationGeoPositions[station.Name] = (lat, lon);
        i++;
    }
}

// Log some stats about the coordinates
if (stationGeoPositions.Count > 0)
{
    var minLat = stationGeoPositions.Values.Min(p => p.Lat);
    var maxLat = stationGeoPositions.Values.Max(p => p.Lat);
    var minLon = stationGeoPositions.Values.Min(p => p.Lon);
    var maxLon = stationGeoPositions.Values.Max(p => p.Lon);

    Console.WriteLine($"Coordonnées limites: Lat({minLat} à {maxLat}), Lon({minLon} à {maxLon})");
}
else
{
    Console.WriteLine("Aucune coordonnée disponible. Veuillez vérifier le format du fichier CSV.");
    return; // Exit the program if we don't have valid data
}

// Visualize the graph using geographic coordinates
GraphVisualizer.VisualizeGeo(
    metroGraph,
    "metro_map.png",
    stationGeoPositions
);

Console.WriteLine($"\nVisualisation du réseau: {"metro_map.png"}");

// Comparaison des différents algorithmes de recherche de chemin
Console.WriteLine("\n=== Comparaison des algorithmes de recherche de chemin ===");

// Sélection de stations pour le test
var random = new Random();
var stationsList = stations.ToList();
var sourceStation = stationsList[random.Next(stationsList.Count)];
var destStation = stationsList[random.Next(stationsList.Count)];

// Assurer que source et destination sont différentes
while (sourceStation.Name == destStation.Name)
{
    destStation = stationsList[random.Next(stationsList.Count)];
}

Console.WriteLine($"Trajet: {sourceStation.Name} → {destStation.Name}");

// Chronomètre pour mesurer les performances
var stopwatch = new System.Diagnostics.Stopwatch();

// Test de l'algorithme de Dijkstra
stopwatch.Start();
var (dijkstraDistances, dijkstraPredecessors) = metroGraph.DijkstraWithPath(sourceStation.Name);
var dijkstraPath = metroGraph.GetPathFromPredecessors(dijkstraPredecessors, sourceStation.Name, destStation.Name);
stopwatch.Stop();
long dijkstraTime = stopwatch.ElapsedMilliseconds;
double dijkstraDistance = dijkstraDistances[destStation.Name];

// Test de l'algorithme de Bellman-Ford
stopwatch.Restart();
var (bellmanFordDistances, bellmanFordPredecessors) = metroGraph.BellmanFord(sourceStation.Name);
var bellmanFordPath = metroGraph.GetPathFromPredecessors(bellmanFordPredecessors, sourceStation.Name, destStation.Name);
stopwatch.Stop();
long bellmanFordTime = stopwatch.ElapsedMilliseconds;
double bellmanFordDistance = bellmanFordDistances[destStation.Name];

// Heuristique pour A* (distance euclidienne)
double Heuristic(string a, string b)
{
    if (!stationGeoPositions.TryGetValue(a, out var posA) || !stationGeoPositions.TryGetValue(b, out var posB))
        return 0;

    // Distance euclidienne simple
    double dx = posA.Lon - posB.Lon;
    double dy = posA.Lat - posB.Lat;
    return Math.Sqrt(dx * dx + dy * dy) * 100; // Facteur d'échelle pour A*
}

// Test de l'algorithme A*
stopwatch.Restart();
var (aStarDistances, aStarPredecessors) = metroGraph.AStar(sourceStation.Name, destStation.Name, Heuristic);
var aStarPath = metroGraph.GetPathFromPredecessors(aStarPredecessors, sourceStation.Name, destStation.Name);
stopwatch.Stop();
long aStarTime = stopwatch.ElapsedMilliseconds;
double aStarDistance = aStarDistances[destStation.Name];

// Test de l'algorithme de Floyd-Warshall (attention: peut être lent pour de grands graphes)
Console.WriteLine("\nCalcul de Floyd-Warshall en cours (peut prendre du temps)...");
stopwatch.Restart();
var (floydWarshallDistances, floydWarshallPredecessors) = metroGraph.FloydWarshall();
var floydWarshallPath = metroGraph.GetPathFloydWarshall(floydWarshallPredecessors, sourceStation.Name, destStation.Name);
stopwatch.Stop();
long floydWarshallTime = stopwatch.ElapsedMilliseconds;
double floydWarshallDistance = floydWarshallDistances[sourceStation.Name][destStation.Name];

// Affichage des résultats
Console.WriteLine("\nRésultats de la comparaison des algorithmes:");
Console.WriteLine($"{"Algorithme",-15} {"Temps (ms)",10} {"Stations",8} {"Distance",10}");
Console.WriteLine(new string('-', 50));
Console.WriteLine($"{"Dijkstra",-15} {dijkstraTime,10} {dijkstraPath.Count,8} {dijkstraDistance,10:F2}");
Console.WriteLine($"{"Bellman-Ford",-15} {bellmanFordTime,10} {bellmanFordPath.Count,8} {bellmanFordDistance,10:F2}");
Console.WriteLine($"{"A*",-15} {aStarTime,10} {aStarPath.Count,8} {aStarDistance,10:F2}");
Console.WriteLine($"{"Floyd-Warshall",-15} {floydWarshallTime,10} {floydWarshallPath.Count,8} {floydWarshallDistance,10:F2}");

// Visualisation des différents chemins
Console.WriteLine("\nVisualisations des chemins:");

// Affiche les détails du chemin Dijkstra sans visualisation spécifique
Console.WriteLine($"\nChemin Dijkstra:");
Console.WriteLine($"  Nombre de stations: {dijkstraPath.Count}");
Console.WriteLine($"  Distance totale: {dijkstraDistance:F2}");
Console.WriteLine($"  Itinéraire: {string.Join(" → ", dijkstraPath)}");

// Affiche les détails du chemin Bellman-Ford
Console.WriteLine($"\nChemin Bellman-Ford:");
Console.WriteLine($"  Nombre de stations: {bellmanFordPath.Count}");
Console.WriteLine($"  Distance totale: {bellmanFordDistance:F2}");
Console.WriteLine($"  Itinéraire: {string.Join(" → ", bellmanFordPath)}");

// Affiche les détails du chemin A*
Console.WriteLine($"\nChemin A*:");
Console.WriteLine($"  Nombre de stations: {aStarPath.Count}");
Console.WriteLine($"  Distance totale: {aStarDistance:F2}");
Console.WriteLine($"  Itinéraire: {string.Join(" → ", aStarPath)}");

// Affiche les détails du chemin Floyd-Warshall
Console.WriteLine($"\nChemin Floyd-Warshall:");
Console.WriteLine($"  Nombre de stations: {floydWarshallPath.Count}");
Console.WriteLine($"  Distance totale: {floydWarshallDistance:F2}");


