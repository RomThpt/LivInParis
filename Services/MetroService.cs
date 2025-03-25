using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LivInParis.Models;

namespace LivInParis.Services
{
    public class MetroService
    {
        private List<MetroStation> _stations = new List<MetroStation>();
        private List<MetroLine> _lines = new List<MetroLine>();
        private Dictionary<string, MetroLine> _linesByNumber = new Dictionary<string, MetroLine>();
        private Dictionary<string, MetroStation> _stationsByName = new Dictionary<string, MetroStation>();

        public Graphe<string> BuildMetroGraph()
        {
            var graph = new Graphe<string>();

            // Ajouter toutes les stations comme nœuds
            foreach (var station in _stations)
            {
                graph.AddNode(station.Name);
            }

            // Ajouter des connexions entre les stations sur la même ligne
            foreach (var line in _lines)
            {
                for (int i = 0; i < line.Stations.Count - 1; i++)
                {
                    graph.AddEdge(line.Stations[i].Name, line.Stations[i + 1].Name);
                }
            }

            return graph;
        }

        // Nouvelle méthode pour obtenir les coordonnées géographiques des stations
        public Dictionary<string, (double Lat, double Lon)> GetStationGeoPositions()
        {
            var positions = new Dictionary<string, (double Lat, double Lon)>();

            foreach (var station in _stations)
            {
                positions[station.Name] = (station.Latitude, station.Longitude);
            }

            return positions;
        }

        // Méthode combinée qui retourne à la fois le graphe et les positions
        public (Graphe<string> Graph, Dictionary<string, (double Lat, double Lon)> Positions) BuildMetroGraphWithPositions()
        {
            var graph = BuildMetroGraph();
            var positions = GetStationGeoPositions();

            return (graph, positions);
        }

        public void LoadSampleMetroData()
        {
            // Création des lignes
            AddLine("1", "Ligne 1");
            AddLine("2", "Ligne 2");
            AddLine("3", "Ligne 3");
            AddLine("4", "Ligne 4");
            AddLine("5", "Ligne 5");
            AddLine("6", "Ligne 6");
            AddLine("7", "Ligne 7");
            AddLine("8", "Ligne 8");
            AddLine("9", "Ligne 9");
            AddLine("11", "Ligne 11");
            AddLine("12", "Ligne 12");
            AddLine("13", "Ligne 13");
            AddLine("14", "Ligne 14");

            // Dictionnaire de coordonnées réelles pour les stations principales
            var stationCoordinates = new Dictionary<string, (double lat, double lon)>
            {
                // Ligne 1
                {"La Défense", (48.891601, 2.238327)},
                {"Charles de Gaulle - Étoile", (48.873963, 2.295036)},
                {"George V", (48.871212, 2.301067)},
                {"Franklin D. Roosevelt", (48.868836, 2.309926)},
                {"Champs-Élysées - Clemenceau", (48.867778, 2.312852)},
                {"Concorde", (48.865491, 2.321582)},
                {"Tuileries", (48.864501, 2.328366)},
                {"Palais Royal - Musée du Louvre", (48.861833, 2.336408)},
                {"Châtelet", (48.858615, 2.346440)},
                {"Hôtel de Ville", (48.857058, 2.351401)},
                {"Bastille", (48.853085, 2.369511)},
                {"Gare de Lyon", (48.844898, 2.373825)},
                {"Nation", (48.848299, 2.395926)},
                
                // Ligne 2
                {"Porte Dauphine", (48.871607, 2.275318)},
                {"Victor Hugo", (48.869941, 2.288096)},
                {"Place de Clichy", (48.883762, 2.327699)},
                {"Barbès - Rochechouart", (48.883449, 2.349681)},
                {"Stalingrad", (48.884242, 2.365380)},
                {"Jaurès", (48.881880, 2.370264)},
                
                // Ligne 3
                {"Pont de Levallois - Bécon", (48.898108, 2.280706)},
                {"Porte de Champerret", (48.885763, 2.293152)},
                {"Opéra", (48.870707, 2.331349)},
                {"République", (48.868134, 2.363777)},
                {"Père Lachaise", (48.862049, 2.388908)},
                {"Gallieni", (48.865217, 2.416265)},
                
                // Ligne 4
                {"Porte de Clignancourt", (48.897708, 2.344428)},
                {"Gare du Nord", (48.880147, 2.355379)},
                {"Les Halles", (48.862306, 2.344590)},
                {"Saint-Michel", (48.853287, 2.343889)},
                {"Montparnasse - Bienvenüe", (48.842232, 2.321784)},
                {"Porte d'Orléans", (48.823516, 2.326417)},
                
                // Ligne 5
                {"Bobigny - Pablo Picasso", (48.906600, 2.449791)},
                {"Gare de l'Est", (48.876538, 2.358103)},
                {"Place d'Italie", (48.831086, 2.355701)},
                
                // Ligne 6
                {"Denfert-Rochereau", (48.833936, 2.332518)},
                
                // Autres stations importantes avec correspondances
                {"Châtelet - Les Halles", (48.862303, 2.345877)},
                {"Saint-Lazare", (48.875319, 2.323895)},
            };

            // Ajout des stations avec leurs coordonnées
            // Pour les stations qui existent sur plusieurs lignes, on les ajoute une seule fois 
            // avec la liste complète des lignes

            // Ligne 1
            AddStation("La Défense", new[] { "1" }, stationCoordinates.GetValueOrDefault("La Défense").lat, stationCoordinates.GetValueOrDefault("La Défense").lon);
            AddStation("Charles de Gaulle - Étoile", new[] { "1", "2", "6" }, stationCoordinates.GetValueOrDefault("Charles de Gaulle - Étoile").lat, stationCoordinates.GetValueOrDefault("Charles de Gaulle - Étoile").lon);
            AddStation("George V", new[] { "1" }, stationCoordinates.GetValueOrDefault("George V").lat, stationCoordinates.GetValueOrDefault("George V").lon);
            AddStation("Franklin D. Roosevelt", new[] { "1", "9" }, stationCoordinates.GetValueOrDefault("Franklin D. Roosevelt").lat, stationCoordinates.GetValueOrDefault("Franklin D. Roosevelt").lon);
            AddStation("Concorde", new[] { "1", "8", "12" }, stationCoordinates.GetValueOrDefault("Concorde").lat, stationCoordinates.GetValueOrDefault("Concorde").lon);
            AddStation("Châtelet", new[] { "1", "4", "7", "11", "14" }, stationCoordinates.GetValueOrDefault("Châtelet").lat, stationCoordinates.GetValueOrDefault("Châtelet").lon);
            AddStation("Gare de Lyon", new[] { "1", "14" }, stationCoordinates.GetValueOrDefault("Gare de Lyon").lat, stationCoordinates.GetValueOrDefault("Gare de Lyon").lon);
            AddStation("Nation", new[] { "1", "2", "6", "9" }, stationCoordinates.GetValueOrDefault("Nation").lat, stationCoordinates.GetValueOrDefault("Nation").lon);

            // Ligne 2
            AddStation("Porte Dauphine", new[] { "2" }, stationCoordinates.GetValueOrDefault("Porte Dauphine").lat, stationCoordinates.GetValueOrDefault("Porte Dauphine").lon);
            AddStation("Place de Clichy", new[] { "2", "13" }, stationCoordinates.GetValueOrDefault("Place de Clichy").lat, stationCoordinates.GetValueOrDefault("Place de Clichy").lon);
            AddStation("Barbès - Rochechouart", new[] { "2", "4" }, stationCoordinates.GetValueOrDefault("Barbès - Rochechouart").lat, stationCoordinates.GetValueOrDefault("Barbès - Rochechouart").lon);
            AddStation("Stalingrad", new[] { "2", "5", "7" }, stationCoordinates.GetValueOrDefault("Stalingrad").lat, stationCoordinates.GetValueOrDefault("Stalingrad").lon);

            // Ligne 3
            AddStation("Opéra", new[] { "3", "7", "8" }, stationCoordinates.GetValueOrDefault("Opéra").lat, stationCoordinates.GetValueOrDefault("Opéra").lon);
            AddStation("République", new[] { "3", "5", "8", "9", "11" }, stationCoordinates.GetValueOrDefault("République").lat, stationCoordinates.GetValueOrDefault("République").lon);
            AddStation("Père Lachaise", new[] { "3", "2" }, stationCoordinates.GetValueOrDefault("Père Lachaise").lat, stationCoordinates.GetValueOrDefault("Père Lachaise").lon);

            // Ligne 4
            AddStation("Gare du Nord", new[] { "4", "5" }, stationCoordinates.GetValueOrDefault("Gare du Nord").lat, stationCoordinates.GetValueOrDefault("Gare du Nord").lon);
            AddStation("Montparnasse - Bienvenüe", new[] { "4", "6", "12", "13" }, stationCoordinates.GetValueOrDefault("Montparnasse - Bienvenüe").lat, stationCoordinates.GetValueOrDefault("Montparnasse - Bienvenüe").lon);

            // Ligne 5
            AddStation("Gare de l'Est", new[] { "4", "5", "7" }, stationCoordinates.GetValueOrDefault("Gare de l'Est").lat, stationCoordinates.GetValueOrDefault("Gare de l'Est").lon);

            // Ligne 6
            AddStation("Denfert-Rochereau", new[] { "4", "6" }, stationCoordinates.GetValueOrDefault("Denfert-Rochereau").lat, stationCoordinates.GetValueOrDefault("Denfert-Rochereau").lon);

            // Connexions entre stations
            ConnectStationsOnLine("1", new[] { "La Défense", "Charles de Gaulle - Étoile", "George V", "Franklin D. Roosevelt", "Concorde", "Châtelet", "Gare de Lyon", "Nation" });
            ConnectStationsOnLine("2", new[] { "Porte Dauphine", "Charles de Gaulle - Étoile", "Place de Clichy", "Barbès - Rochechouart", "Stalingrad", "Nation" });
            ConnectStationsOnLine("3", new[] { "Opéra", "République", "Père Lachaise" });
            ConnectStationsOnLine("4", new[] { "Barbès - Rochechouart", "Gare du Nord", "Gare de l'Est", "Châtelet", "Montparnasse - Bienvenüe", "Denfert-Rochereau" });
            ConnectStationsOnLine("5", new[] { "Stalingrad", "Gare du Nord", "Gare de l'Est", "République" });
            ConnectStationsOnLine("6", new[] { "Charles de Gaulle - Étoile", "Montparnasse - Bienvenüe", "Denfert-Rochereau", "Nation" });

            Console.WriteLine("Données métro chargées avec succès!");
        }

        public void LoadMetroDataFromCsv(string filePath)
        {
            // Effacer les données existantes
            _stations.Clear();
            _lines.Clear();
            _linesByNumber.Clear();
            _stationsByName.Clear();

            // Dictionnaire pour stocker les IDs des stations et les objets pour une recherche rapide
            var stationsById = new Dictionary<string, MetroStation>();
            var connections = new List<(string FromId, string ToId)>();

            try
            {
                // Lire toutes les lignes du fichier
                var lines = File.ReadAllLines(filePath);
                bool isFirstLine = true;

                foreach (var line in lines)
                {
                    // Ignorer les lignes vides, les commentaires et l'entête (première ligne)
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//") || line.StartsWith("#"))
                        continue;

                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        if (line.Contains(";") && line.ToLower().Contains("id;ligne"))
                            continue; // Ignorer la ligne d'en-tête
                    }

                    var parts = line.Split(';');
                    if (parts.Length < 6) continue; // Format attendu: id;ligne;terminus;nom;latitude;longitude

                    // Extraction des données
                    string id = parts[0].Trim();
                    string lineNumber = parts[1].Trim();
                    string isTerminus = parts[2].Trim(); // "0" ou "1"
                    string stationName = parts[3].Trim().Replace('_', ' '); // Remplacer les underscores par des espaces

                    // Amélioration de l'extraction des coordonnées
                    double lat = 0, lon = 0;
                    string latStr = parts[4].Trim().Replace(',', '.');
                    string lonStr = parts[5].Trim().Replace(',', '.');

                    // Utiliser InvariantCulture pour s'assurer que le point décimal est correctement interprété
                    if (!double.TryParse(latStr, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out lat))
                    {
                        Console.WriteLine($"Erreur de parsing pour latitude ({latStr}) de {stationName}");
                    }

                    if (!double.TryParse(lonStr, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out lon))
                    {
                        Console.WriteLine($"Erreur de parsing pour longitude ({lonStr}) de {stationName}");
                    }

                    // Vérification des coordonnées valides
                    if (lat == 0 || lon == 0)
                    {
                        Console.WriteLine($"Coordonnées potentiellement invalides pour {stationName}: Lat={lat}, Lon={lon}");
                    }

                    // Créer un identifiant unique pour cette station
                    string stationId = $"{lineNumber}:{id}";

                    // Ajouter la ligne si elle n'existe pas déjà
                    if (!_linesByNumber.ContainsKey(lineNumber))
                    {
                        // Extraire la partie numérique pour la couleur
                        int lineColorIndex = ExtractLineNumber(lineNumber);
                        AddLine(lineNumber, GetColorForLine(lineColorIndex));
                    }

                    // Créer une station si elle n'existe pas encore
                    if (!_stationsByName.TryGetValue(stationName, out var station))
                    {
                        station = new MetroStation(stationName, lat, lon);
                        _stations.Add(station);
                        _stationsByName[stationName] = station;
                    }
                    else if (station.Latitude == 0 && station.Longitude == 0 && (lat != 0 || lon != 0))
                    {
                        // Mettre à jour les coordonnées si la station existe déjà mais n'a pas de coordonnées valides
                        station.Latitude = lat;
                        station.Longitude = lon;
                    }

                    // Ajouter la ligne à la station
                    if (_linesByNumber.TryGetValue(lineNumber, out var metroLine))
                    {
                        station.AddLine(metroLine);
                    }

                    // Ajouter à nos collections pour le traitement des connexions
                    stationsById[stationId] = station;
                }

                // Créer des connexions entre les stations sur la même ligne
                // Grouper les stations par ligne
                var stationsByLine = stationsById
                    .GroupBy(s => s.Key.Split(':')[0]) // Grouper par numéro de ligne
                    .ToDictionary(g => g.Key, g => g.OrderBy(s => int.Parse(s.Key.Split(':')[1])).ToList()); // Trier par ID

                // Pour chaque ligne, créer des connexions entre stations consécutives
                foreach (var lineStations in stationsByLine)
                {
                    string lineNumber = lineStations.Key;
                    var stations = lineStations.Value;

                    for (int i = 0; i < stations.Count - 1; i++)
                    {
                        string fromId = stations[i].Key;
                        string toId = stations[i + 1].Key;

                        if (stationsById.TryGetValue(fromId, out var fromStation) &&
                            stationsById.TryGetValue(toId, out var toStation))
                        {
                            // Ajouter les stations à la ligne
                            if (_linesByNumber.TryGetValue(lineNumber, out var line))
                            {
                                line.AddStation(fromStation);
                                line.AddStation(toStation);
                            }
                        }
                    }
                }

                Console.WriteLine($"Chargé {_stations.Count} stations et {_lines.Count} lignes depuis {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des données: {ex.Message}");

                // En cas d'erreur, charger les données de secours
                LoadSampleMetroData();
            }
        }

        private void IdentifyLinesFromConnections(
            Dictionary<string, MetroStation> stationsById,
            List<(string FromId, string ToId)> connections)
        {
            // Créer un graphe de connexions pour identifier les lignes
            var connectionGraph = new Dictionary<string, List<string>>();

            foreach (var (fromId, toId) in connections)
            {
                if (!connectionGraph.ContainsKey(fromId))
                    connectionGraph[fromId] = new List<string>();

                if (!connectionGraph.ContainsKey(toId))
                    connectionGraph[toId] = new List<string>();

                if (!connectionGraph[fromId].Contains(toId))
                    connectionGraph[fromId].Add(toId);

                if (!connectionGraph[toId].Contains(fromId))
                    connectionGraph[toId].Add(fromId);
            }

            // Trouver les points de départ des lignes (stations avec une seule connexion ou terminaux)
            var termini = connectionGraph.Where(kvp => kvp.Value.Count == 1)
                                        .Select(kvp => kvp.Key)
                                        .ToList();

            // Créer des lignes
            int lineCount = 0;
            var processedStations = new HashSet<string>();

            foreach (var terminus in termini)
            {
                if (processedStations.Contains(terminus))
                    continue;

                lineCount++;
                var lineNumber = lineCount.ToString();
                var lineColor = GetColorForLine(lineCount);

                AddLine(lineNumber, lineColor);

                // Tracer la ligne depuis ce terminus
                var stationSequence = TraceLine(terminus, connectionGraph, processedStations);

                // Ajouter des stations à la ligne
                var stationNames = stationSequence
                    .Where(id => stationsById.ContainsKey(id))
                    .Select(id => stationsById[id].Name)
                    .ToArray();

                // Ajouter la référence de la ligne aux stations
                foreach (var id in stationSequence)
                {
                    if (stationsById.ContainsKey(id))
                    {
                        stationsById[id].AddLine(_linesByNumber[lineNumber]);
                    }
                }

                ConnectStationsOnLine(lineNumber, stationNames);
            }
        }

        private List<string> TraceLine(
            string startId,
            Dictionary<string, List<string>> connectionGraph,
            HashSet<string> processedStations)
        {
            var result = new List<string> { startId };
            processedStations.Add(startId);

            var currentId = startId;
            bool foundNext = true;

            while (foundNext)
            {
                foundNext = false;

                if (!connectionGraph.ContainsKey(currentId))
                    break;

                foreach (var nextId in connectionGraph[currentId])
                {
                    if (processedStations.Contains(nextId))
                        continue;

                    // Suivre le chemin
                    currentId = nextId;
                    result.Add(currentId);
                    processedStations.Add(currentId);
                    foundNext = true;
                    break;
                }
            }

            return result;
        }

        private int ExtractLineNumber(string lineNumber)
        {
            // Extraire la partie numérique du numéro de ligne (ex: "3bis" → 3)
            string numericPart = new string(lineNumber.TakeWhile(char.IsDigit).ToArray());
            if (int.TryParse(numericPart, out int result))
                return result;
            return 1; // Valeur par défaut
        }

        private string GetColorForLine(int lineNumber)
        {
            // Sélection simple de couleur basée sur le numéro de ligne
            string[] colors = { "Yellow", "Green", "Blue", "Red", "Purple", "Orange", "Brown", "Pink", "Teal", "Lime" };
            return colors[(lineNumber - 1) % colors.Length];
        }

        private void AddLine(string number, string color)
        {
            var line = new MetroLine(number, color);
            _lines.Add(line);
            _linesByNumber[number] = line;
        }

        private void AddStation(string name, string[] lineNumbers, double lat = 0, double lon = 0)
        {
            if (!_stationsByName.TryGetValue(name, out var station))
            {
                station = new MetroStation(name, lat, lon);
                _stations.Add(station);
                _stationsByName[name] = station;
            }

            foreach (var lineNumber in lineNumbers)
            {
                if (_linesByNumber.TryGetValue(lineNumber, out var line))
                {
                    station.AddLine(line);
                }
            }
        }

        private void ConnectStationsOnLine(string lineNumber, string[] stationNames)
        {
            if (!_linesByNumber.TryGetValue(lineNumber, out var line))
                return;

            foreach (var stationName in stationNames)
            {
                if (_stationsByName.TryGetValue(stationName, out var station))
                {
                    line.AddStation(station);
                }
            }
        }

        public List<MetroStation> GetAllStations() => _stations;

        public List<MetroLine> GetAllLines() => _lines;
    }
}
