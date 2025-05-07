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

        public MetroNetworkDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ImporterDonneesMetro(string cheminFichierXml)
        {
            /// Créer les tables si elles n'existent pas
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    CREATE TABLE IF NOT EXISTS StationsMetro (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        Nom VARCHAR(100) NOT NULL,
                        Ligne VARCHAR(50),
                        Latitude DECIMAL(10,8) NOT NULL,
                        Longitude DECIMAL(11,8) NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS ConnexionsMetro (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        StationDepartId INT NOT NULL,
                        StationArriveeId INT NOT NULL,
                        DureeMinutes INT NOT NULL,
                        FOREIGN KEY (StationDepartId) REFERENCES StationsMetro(Id),
                        FOREIGN KEY (StationArriveeId) REFERENCES StationsMetro(Id)
                    );";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                /// Charger le document XML
                var doc = new XmlDocument();
                doc.Load(cheminFichierXml);

                /// Ouvrir la connexion à la base de données
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        /// Commencer la transaction pour de meilleures performances
                        /// Effacer les données existantes
                        using (var cmd = new MySqlCommand("DELETE FROM ConnexionsMetro; DELETE FROM StationsMetro;", connection, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        /// Importer les stations
                        var stations = doc.SelectNodes("//station");
                        if (stations != null)
                        {
                            foreach (XmlNode station in stations)
                            {
                                string query = @"
                                    INSERT INTO StationsMetro (Nom, Ligne, Latitude, Longitude)
                                    VALUES (@Nom, @Ligne, @Latitude, @Longitude)";

                                using (var cmd = new MySqlCommand(query, connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@Nom", station.SelectSingleNode("nom")?.InnerText);
                                    cmd.Parameters.AddWithValue("@Ligne", station.SelectSingleNode("ligne")?.InnerText);
                                    cmd.Parameters.AddWithValue("@Latitude", Convert.ToDouble(station.SelectSingleNode("latitude")?.InnerText));
                                    cmd.Parameters.AddWithValue("@Longitude", Convert.ToDouble(station.SelectSingleNode("longitude")?.InnerText));
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        /// Importer les connexions
                        var connections = doc.SelectNodes("//connection");
                        if (connections != null)
                        {
                            foreach (XmlNode connection in connections)
                            {
                                string query = @"
                                    INSERT INTO ConnexionsMetro (StationDepartId, StationArriveeId, DureeMinutes)
                                    SELECT 
                                        s1.Id as StationDepartId,
                                        s2.Id as StationArriveeId,
                                        @DureeMinutes
                                    FROM StationsMetro s1, StationsMetro s2
                                    WHERE s1.Nom = @StationDepart AND s2.Nom = @StationArrivee";

                                using (var cmd = new MySqlCommand(query, connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@StationDepart", connection.SelectSingleNode("depart")?.InnerText);
                                    cmd.Parameters.AddWithValue("@StationArrivee", connection.SelectSingleNode("arrivee")?.InnerText);
                                    cmd.Parameters.AddWithValue("@DureeMinutes", Convert.ToInt32(connection.SelectSingleNode("duree")?.InnerText));
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        /// Valider la transaction
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
                        using (var cmd = new MySqlCommand("DELETE FROM ConnexionsMetro;", connection, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        /// Puis supprimer les stations
                        using (var cmd = new MySqlCommand("DELETE FROM StationsMetro;", connection, transaction))
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
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM StationsMetro;", connection))
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
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM ConnexionsMetro;", connection))
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
                using (var cmd = new MySqlCommand("SELECT Id FROM StationsMetro ORDER BY Id;", connection))
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
                    SELECT StationArriveeId 
                    FROM ConnexionsMetro 
                    WHERE StationDepartId = @StationId
                    UNION
                    SELECT StationDepartId 
                    FROM ConnexionsMetro 
                    WHERE StationArriveeId = @StationId";

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
                    FROM StationsMetro s1
                    JOIN StationsMetro s2 ON s1.Ligne = s2.Ligne
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