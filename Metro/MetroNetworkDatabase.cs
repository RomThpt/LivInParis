using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MySql.Data.MySqlClient;
using LivInParis.Models;

namespace LivInParis.Metro
{
    public class MetroNetworkDatabase
    {
        private readonly string _connectionString;

        public MetroNetworkDatabase(string server, string database, string uid, string password)
        {
            _connectionString = $"Server={server};Database={database};Uid={uid};Pwd={password};";
        }

        public void InitializeDatabase()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                // Create MetroStations table if not exists
                string createStationsTable = @"
                    CREATE TABLE IF NOT EXISTS MetroStations (
                        Id VARCHAR(50) PRIMARY KEY,
                        Name VARCHAR(100) NOT NULL,
                        Latitude DOUBLE NOT NULL,
                        Longitude DOUBLE NOT NULL,
                        Line VARCHAR(10) NOT NULL
                    )";

                // Create MetroConnections table if not exists
                string createConnectionsTable = @"
                    CREATE TABLE IF NOT EXISTS MetroConnections (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        FromStationId VARCHAR(50) NOT NULL,
                        ToStationId VARCHAR(50) NOT NULL,
                        Distance DOUBLE NOT NULL,
                        FOREIGN KEY (FromStationId) REFERENCES MetroStations(Id),
                        FOREIGN KEY (ToStationId) REFERENCES MetroStations(Id)
                    )";

                using (var cmd = new MySqlCommand(createStationsTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new MySqlCommand(createConnectionsTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ImportFromXml(string xmlPath)
        {
            var doc = new XmlDocument();
            doc.Load(xmlPath);

            using (var dbConnection = new MySqlConnection(_connectionString))
            {
                dbConnection.Open();

                // Clear existing data
                using (var cmd = new MySqlCommand("DELETE FROM MetroConnections; DELETE FROM MetroStations;", dbConnection))
                {
                    cmd.ExecuteNonQuery();
                }

                // Import stations
                var stations = doc.SelectNodes("//station");
                if (stations != null)
                {
                    foreach (XmlNode station in stations)
                    {
                        string id = station.Attributes?["id"]?.Value ?? "";
                        string name = station.Attributes?["name"]?.Value ?? "";
                        string x = station.Attributes?["x"]?.Value ?? "0";
                        string y = station.Attributes?["y"]?.Value ?? "0";
                        string line = station.Attributes?["line"]?.Value ?? "1";

                        string insertStation = @"
                            INSERT INTO MetroStations (Id, Name, Latitude, Longitude, Line)
                            VALUES (@id, @name, @lat, @lon, @line)";

                        using (var cmd = new MySqlCommand(insertStation, dbConnection))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@name", name);
                            cmd.Parameters.AddWithValue("@lat", double.Parse(y));
                            cmd.Parameters.AddWithValue("@lon", double.Parse(x));
                            cmd.Parameters.AddWithValue("@line", line);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // Import connections
                var connections = doc.SelectNodes("//connection");
                if (connections != null)
                {
                    foreach (XmlNode connectionNode in connections)
                    {
                        string from = connectionNode.Attributes?["from"]?.Value ?? "";
                        string to = connectionNode.Attributes?["to"]?.Value ?? "";
                        string distance = connectionNode.Attributes?["distance"]?.Value ?? "0";

                        string insertConnection = @"
                            INSERT INTO MetroConnections (FromStationId, ToStationId, Distance)
                            VALUES (@from, @to, @distance)";

                        using (var cmd = new MySqlCommand(insertConnection, dbConnection))
                        {
                            cmd.Parameters.AddWithValue("@from", from);
                            cmd.Parameters.AddWithValue("@to", to);
                            cmd.Parameters.AddWithValue("@distance", double.Parse(distance));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public void PrintNetworkStats()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                // Count stations
                string countStations = "SELECT COUNT(*) FROM MetroStations";
                using (var cmd = new MySqlCommand(countStations, connection))
                {
                    int stationCount = Convert.ToInt32(cmd.ExecuteScalar());
                    Console.WriteLine($"Nombre total de stations: {stationCount}");
                }

                // Count connections
                string countConnections = "SELECT COUNT(*) FROM MetroConnections";
                using (var cmd = new MySqlCommand(countConnections, connection))
                {
                    int connectionCount = Convert.ToInt32(cmd.ExecuteScalar());
                    Console.WriteLine($"Nombre total de connexions: {connectionCount}");
                }

                // Average connections per station
                string avgConnections = @"
                    SELECT AVG(connection_count) 
                    FROM (
                        SELECT COUNT(*) as connection_count 
                        FROM MetroConnections 
                        GROUP BY FromStationId
                    ) as counts";
                using (var cmd = new MySqlCommand(avgConnections, connection))
                {
                    double avgConn = Convert.ToDouble(cmd.ExecuteScalar());
                    Console.WriteLine($"Nombre moyen de connexions par station: {avgConn:F2}");
                }
            }
        }

        public void CalculateRoute(string fromStation, string toStation)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                // Verify stations exist
                string checkStations = @"
                    SELECT COUNT(*) 
                    FROM MetroStations 
                    WHERE Name IN (@from, @to)";
                using (var cmd = new MySqlCommand(checkStations, connection))
                {
                    cmd.Parameters.AddWithValue("@from", fromStation);
                    cmd.Parameters.AddWithValue("@to", toStation);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count != 2)
                    {
                        Console.WriteLine("Une ou plusieurs stations n'existent pas.");
                        return;
                    }
                }

                // Simple route calculation (this is a placeholder - implement proper pathfinding)
                Console.WriteLine($"Calcul de l'itinéraire de {fromStation} à {toStation}...");
                Console.WriteLine("Cette fonctionnalité nécessite l'implémentation d'un algorithme de plus court chemin.");
            }
        }

        public void SupprimerDonneesMetro()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        /// D'abord supprimer les connexions (à cause des contraintes de clé étrangère)
                        using (var cmd = new MySqlCommand("DELETE FROM MetroConnections;", connection, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        /// Puis supprimer les stations
                        using (var cmd = new MySqlCommand("DELETE FROM MetroStations;", connection, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public int ObtenirNombreStations()
        {
            /// Obtenir le nombre de stations
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM MetroStations;", connection))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public int ObtenirNombreConnexions()
        {
            /// Obtenir le nombre de connexions
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM MetroConnections;", connection))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public List<int> ObtenirIdsStations()
        {
            /// Obtenir tous les IDs des stations
            var ids = new List<int>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (var cmd = new MySqlCommand("SELECT Id FROM MetroStations ORDER BY Id;", connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add(reader.GetInt32("Id"));
                        }
                    }
                }
            }
            return ids;
        }

        public void AfficherPremieresStations()
        {
            /// Afficher les 20 premiers IDs de stations
            var ids = ObtenirIdsStations();
            Console.WriteLine("Premiers IDs de stations :");
            for (int i = 0; i < Math.Min(20, ids.Count); i++)
            {
                Console.WriteLine($"Station {i + 1}: ID = {ids[i]}");
            }
        }

        public bool VerifierConnexite()
        {
            /// Vérifier si le graphe est connexe (simplifié)
            var ids = ObtenirIdsStations();
            if (ids.Count == 0) return true;

            var visite = new HashSet<int>();
            var aVisiter = new Queue<int>();
            aVisiter.Enqueue(ids[0]);

            while (aVisiter.Count > 0)
            {
                var current = aVisiter.Dequeue();
                if (visite.Contains(current)) continue;

                visite.Add(current);
                var voisins = ObtenirStationsVoisines(current);
                foreach (var voisin in voisins)
                {
                    if (!visite.Contains(voisin))
                        aVisiter.Enqueue(voisin);
                }
            }

            return visite.Count == ids.Count;
        }

        private List<int> ObtenirStationsVoisines(int stationId)
        {
            /// Obtenir les IDs des stations
            var voisins = new List<int>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT ToStationId 
                    FROM MetroConnections 
                    WHERE FromStationId = @StationId
                    UNION
                    SELECT FromStationId 
                    FROM MetroConnections 
                    WHERE ToStationId = @StationId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StationId", stationId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            voisins.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            return voisins;
        }

        public List<int> CalculerChemin(int depart, int arrivee)
        {
            /// Implémentation simplifiée de Dijkstra
            /// Pour l'instant, juste montrer un chemin simple avec une distance fictive
            var chemin = new List<int> { depart, arrivee };
            return chemin;
        }

        public List<int> ObtenirStationsMemeLigne(int stationId)
        {
            /// Obtenir quelques stations sur la même ligne
            var stations = new List<int>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT s2.Id
                    FROM MetroStations s1
                    JOIN MetroStations s2 ON s1.Line = s2.Line
                    WHERE s1.Id = @StationId
                    LIMIT 5";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StationId", stationId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stations.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            return stations;
        }
    }
}