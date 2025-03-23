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

        public void LoadSampleMetroData()
        {
            // Déterminer le chemin du fichier CSV
            string[] possiblePaths = {
                "./public/MetroParis.csv",
            };

            string? filePath = possiblePaths.FirstOrDefault(File.Exists);

            if (filePath == null)
            {
                throw new FileNotFoundException("Le fichier MetroParis.csv est introuvable");
            }

            // Utiliser la méthode existante pour charger les données du CSV
            LoadMetroDataFromCsv(filePath);
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

            // Ignorer la ligne d'en-tête
            var lines = File.ReadAllLines(filePath).Skip(1);

            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length < 8) continue;

                var id = parts[0].Trim();
                var name = parts[1].Trim();
                var prevId = parts[2].Trim();
                var nextId = parts[3].Trim();

                // Analyser les coordonnées, par défaut à 0 si l'analyse échoue
                double.TryParse(parts[6].Trim(), out double lat);
                double.TryParse(parts[7].Trim(), out double lon);

                // Créer une station si elle n'existe pas
                if (!stationsById.ContainsKey(id))
                {
                    var station = new MetroStation(name, lat, lon);
                    stationsById[id] = station;
                    _stations.Add(station);
                    _stationsByName[name] = station;
                }

                // Stocker les connexions pour un traitement ultérieur
                if (!string.IsNullOrEmpty(prevId))
                {
                    connections.Add((prevId, id));
                }

                if (!string.IsNullOrEmpty(nextId))
                {
                    connections.Add((id, nextId));
                }
            }

            // Identifier les lignes en analysant les connexions
            IdentifyLinesFromConnections(stationsById, connections);
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
