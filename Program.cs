using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LivInParis.Models;
using LivInParis.Services;

Console.WriteLine("Bienvenue à Liv in Paris!");

// Création du service métro et chargement des données
var metroService = new MetroService();
metroService.LoadSampleMetroData();

// Obtenir toutes les stations et lignes
var stations = metroService.GetAllStations();
var lines = metroService.GetAllLines();

// Afficher les informations
Console.WriteLine($"\nNombre de stations de métro: {stations.Count}");
Console.WriteLine($"Nombre de lignes de métro: {lines.Count}");

// Construire le graphe du métro
var metroGraph = metroService.BuildMetroGraph();


// Visualisation du réseau métro complet
GraphVisualizer.Visualize(metroGraph, "./public/graphe/metro_network_.png");
Console.WriteLine($"\nVisualisation du réseau: {"./public/graphe/metro_network_.png"}");

// Recherche d'itinéraire
if (stations.Count > 0)
{
    // Exemple de stations de départ et d'arrivée
    var startStation = stations[0];
    var endStation = stations[stations.Count > 3 ? 3 : stations.Count - 1];

    Console.WriteLine($"\nRecherche du chemin de {startStation.Name} à {endStation.Name}:");

    var path = metroGraph.GetShortestPath(startStation.Name, endStation.Name);

    if (path.Count > 0)
    {
        Console.WriteLine("Itinéraire trouvé:");
        foreach (var stationName in path)
        {
            Console.WriteLine($" - {stationName}");
        }

        // Visualisation de l'itinéraire
        string itineraryFilePath = Path.Combine($"metro_itinerary.png");
        GraphVisualizer.Visualize(metroGraph, itineraryFilePath, path);
        Console.WriteLine($"\nVisualisation de l'itinéraire: {itineraryFilePath}");
    }
    else
    {
        Console.WriteLine("Aucun itinéraire trouvé.");
    }
}
