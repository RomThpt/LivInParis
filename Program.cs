using System;
using System.Collections.Generic;
using LivInParis.Models;
using LivInParis.Services;
using LivInParis.UI;
using MySql.Data.MySqlClient;
using System.Data;

namespace LivInParis
{
    class Program
    {
        /// Paramètres de connexion à la base de données
        private const string SERVER = "localhost";
        private const string DATABASE = "livinparis";
        private const string ADMIN_USER = "root";
        private const string ADMIN_PASSWORD = "Maman888";

        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.Title = "Liv'In Paris - Système de Gestion";

                /// Créer et initialiser la base de données
                InitialiserBaseDeDonnees();

                /// Démarrer l'interface console
                var consoleUI = new ConsoleUI(SERVER, DATABASE, ADMIN_USER, ADMIN_PASSWORD);
                consoleUI.DisplayMainMenu();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erreur critique: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("\nAppuyez sur une touche pour quitter...");
                Console.ReadKey();
            }
        }

        static void InitialiserBaseDeDonnees()
        {
            /// Créer la connexion en mode admin
            using (var connection = new MySqlConnection($"Server={SERVER};Uid={ADMIN_USER};Pwd={ADMIN_PASSWORD};"))
            {
                connection.Open();

                /// Créer la base de données si elle n'existe pas
                using (var cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {DATABASE};", connection))
                {
                    cmd.ExecuteNonQuery();
                }

                /// Utiliser la base de données
                using (var cmd = new MySqlCommand($"USE {DATABASE};", connection))
                {
                    cmd.ExecuteNonQuery();
                }

                /// Créer les tables si elles n'existent pas
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = connection;

                    /// Vérifier si des données de test doivent être ajoutées
                    bool ajouterDonneesTest = true;

                    /// Table Cuisiniers
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Cuisiniers (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            Nom VARCHAR(50) NOT NULL,
                            Prenom VARCHAR(50) NOT NULL,
                            Email VARCHAR(100) UNIQUE NOT NULL,
                            Telephone VARCHAR(20) NOT NULL,
                            Adresse TEXT NOT NULL,
                            NoteMoyenne DECIMAL(3,2) DEFAULT 0.00,
                            Statut TINYINT DEFAULT 1
                        );";
                    cmd.ExecuteNonQuery();

                    /// Table Clients
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Clients (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            Nom VARCHAR(50) NOT NULL,
                            Prenom VARCHAR(50) NOT NULL,
                            Email VARCHAR(100) UNIQUE NOT NULL,
                            Telephone VARCHAR(20) NOT NULL,
                            Adresse TEXT NOT NULL,
                            TypeClient ENUM('Individuel', 'Entreprise') NOT NULL,
                            NoteMoyenne DECIMAL(3,2) DEFAULT 0.00,
                            Statut TINYINT DEFAULT 1
                        );";
                    cmd.ExecuteNonQuery();

                    /// Table Plats
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Plats (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            Nom VARCHAR(100) NOT NULL,
                            Description TEXT,
                            PrixParPersonne DECIMAL(10,2) NOT NULL,
                            CuisinierId INT NOT NULL,
                            DatePreparation DATETIME NOT NULL,
                            DateExpiration DATETIME NOT NULL,
                            NombrePersonnes INT NOT NULL,
                            NationaliteCuisine VARCHAR(50),
                            PhotoUrl VARCHAR(255),
                            FOREIGN KEY (CuisinierId) REFERENCES Cuisiniers(Id)
                        );";
                    cmd.ExecuteNonQuery();

                    /// Table IngredientsPlat
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS IngredientsPlat (
                            PlatId INT NOT NULL,
                            Ingredient VARCHAR(100) NOT NULL,
                            PRIMARY KEY (PlatId, Ingredient),
                            FOREIGN KEY (PlatId) REFERENCES Plats(Id)
                        );";
                    cmd.ExecuteNonQuery();

                    /// Table RegimesAlimentairesPlat
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS RegimesAlimentairesPlat (
                            PlatId INT NOT NULL,
                            Regime VARCHAR(50) NOT NULL,
                            PRIMARY KEY (PlatId, Regime),
                            FOREIGN KEY (PlatId) REFERENCES Plats(Id)
                        );";
                    cmd.ExecuteNonQuery();

                    /// Table Commandes
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Commandes (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            ClientId INT NOT NULL,
                            DateCommande DATETIME NOT NULL,
                            PrixTotal DECIMAL(10,2) NOT NULL,
                            Statut TINYINT DEFAULT 0,
                            AdresseLivraison TEXT NOT NULL,
                            FOREIGN KEY (ClientId) REFERENCES Clients(Id)
                        );";
                    cmd.ExecuteNonQuery();

                    /// Table CommandeItems
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS CommandeItems (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            CommandeId INT NOT NULL,
                            PlatId INT NOT NULL,
                            Quantite INT NOT NULL,
                            PrixUnitaire DECIMAL(10,2) NOT NULL,
                            FOREIGN KEY (CommandeId) REFERENCES Commandes(Id),
                            FOREIGN KEY (PlatId) REFERENCES Plats(Id)
                        );";
                    cmd.ExecuteNonQuery();

                    /// Table Livraisons
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Livraisons (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            CommandeId INT NOT NULL,
                            CuisinierId INT NOT NULL,
                            DateLivraison DATETIME NOT NULL,
                            Statut TINYINT DEFAULT 0,
                            CheminLivraison TEXT,
                            FOREIGN KEY (CommandeId) REFERENCES Commandes(Id),
                            FOREIGN KEY (CuisinierId) REFERENCES Cuisiniers(Id)
                        );";
                    cmd.ExecuteNonQuery();

                    /// Table Notations
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Notations (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            CommandeId INT NOT NULL,
                            NoteurId INT NOT NULL,
                            Note INT NOT NULL CHECK (Note BETWEEN 1 AND 5),
                            Commentaire TEXT,
                            DateNotation DATETIME NOT NULL,
                            FOREIGN KEY (CommandeId) REFERENCES Commandes(Id)
                        );";
                    cmd.ExecuteNonQuery();

                    /// Table HistoriqueCommandes
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS HistoriqueCommandes (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            CommandeId INT NOT NULL,
                            StatutPrecedent TINYINT,
                            StatutNouveau TINYINT,
                            DateModification DATETIME NOT NULL,
                            FOREIGN KEY (CommandeId) REFERENCES Commandes(Id)
                        );";
                    cmd.ExecuteNonQuery();

                    /// Procédure stockée pour mettre à jour la note moyenne d'un cuisinier
                    cmd.CommandText = @"
                        CREATE PROCEDURE IF NOT EXISTS UpdateCuisinierNote(IN cuisinierId INT)
                        BEGIN
                            UPDATE Cuisiniers c
                            SET c.NoteMoyenne = (
                                SELECT AVG(n.Note)
                                FROM Notations n
                                JOIN Commandes cmd ON n.CommandeId = cmd.Id
                                JOIN Plats p ON cmd.Id IN (
                                    SELECT CommandeId 
                                    FROM CommandeItems 
                                    WHERE PlatId = p.Id
                                )
                                WHERE p.CuisinierId = cuisinierId
                            )
                            WHERE c.Id = cuisinierId;
                        END;";
                    cmd.ExecuteNonQuery();

                    /// Procédure stockée pour mettre à jour la note moyenne d'un client
                    cmd.CommandText = @"
                        CREATE PROCEDURE IF NOT EXISTS UpdateClientNote(IN clientId INT)
                        BEGIN
                            UPDATE Clients c
                            SET c.NoteMoyenne = (
                                SELECT AVG(n.Note)
                                FROM Notations n
                                JOIN Commandes cmd ON n.CommandeId = cmd.Id
                                WHERE cmd.ClientId = clientId
                            )
                            WHERE c.Id = clientId;
                        END;";
                    cmd.ExecuteNonQuery();

                    /// Procédure stockée pour obtenir les livraisons par cuisinier
                    cmd.CommandText = @"
                        CREATE PROCEDURE IF NOT EXISTS GetLivraisonsByCuisinier(
                            IN cuisinierId INT,
                            IN dateDebut DATE,
                            IN dateFin DATE
                        )
                        BEGIN
                            SELECT 
                                l.Id,
                                l.DateLivraison,
                                c.Nom as ClientNom,
                                c.Prenom as ClientPrenom,
                                c.Adresse as AdresseLivraison,
                                cmd.PrixTotal
                            FROM Livraisons l
                            JOIN Commandes cmd ON l.CommandeId = cmd.Id
                            JOIN Clients c ON cmd.ClientId = c.Id
                            WHERE l.CuisinierId = cuisinierId
                            AND DATE(l.DateLivraison) BETWEEN dateDebut AND dateFin
                            ORDER BY l.DateLivraison;
                        END;";
                    cmd.ExecuteNonQuery();

                    /// Procédure stockée pour obtenir les commandes par période
                    cmd.CommandText = @"
                        CREATE PROCEDURE IF NOT EXISTS GetCommandesByPeriod(
                            IN dateDebut DATE,
                            IN dateFin DATE
                        )
                        BEGIN
                            SELECT 
                                cmd.Id,
                                cmd.DateCommande,
                                c.Nom as ClientNom,
                                c.Prenom as ClientPrenom,
                                cmd.PrixTotal,
                                cmd.Statut
                            FROM Commandes cmd
                            JOIN Clients c ON cmd.ClientId = c.Id
                            WHERE DATE(cmd.DateCommande) BETWEEN dateDebut AND dateFin
                            ORDER BY cmd.DateCommande;
                        END;";
                    cmd.ExecuteNonQuery();

                    /// Procédure stockée pour obtenir les commandes d'un client
                    cmd.CommandText = @"
                        CREATE PROCEDURE IF NOT EXISTS GetClientCommandes(
                            IN clientId INT
                        )
                        BEGIN
                            SELECT 
                                cmd.Id,
                                cmd.DateCommande,
                                cmd.PrixTotal,
                                cmd.Statut,
                                GROUP_CONCAT(
                                    CONCAT(p.Nom, ' (', ci.Quantite, ')')
                                    SEPARATOR ', '
                                ) as Plats
                            FROM Commandes cmd
                            JOIN CommandeItems ci ON cmd.Id = ci.CommandeId
                            JOIN Plats p ON ci.PlatId = p.Id
                            WHERE cmd.ClientId = clientId
                            GROUP BY cmd.Id
                            ORDER BY cmd.DateCommande DESC;
                        END;";
                    cmd.ExecuteNonQuery();

                    /// Tables pour le réseau de métro (pour l'optimisation des livraisons)
                    cmd.CommandText = @"
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
                    cmd.ExecuteNonQuery();

                    /// Table pour stocker les plannings des cuisiniers
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS PlanningsCuisiniers (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            CuisinierId INT NOT NULL,
                            JourSemaine TINYINT NOT NULL CHECK (JourSemaine BETWEEN 1 AND 7),
                            HeureDebut TIME NOT NULL,
                            HeureFin TIME NOT NULL,
                            FOREIGN KEY (CuisinierId) REFERENCES Cuisiniers(Id)
                        );";
                    cmd.ExecuteNonQuery();

                    if (ajouterDonneesTest)
                    {
                        /// Vérifier s'il y a déjà des cuisiniers dans la base
                        using (var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Cuisiniers;", connection))
                        {
                            long count = (long)checkCmd.ExecuteScalar();
                            if (count == 0)
                            {
                                /// Ajouter des cuisiniers tests
                                cmd.CommandText = @"
                                    INSERT INTO Cuisiniers (Nom, Prenom, Email, Telephone, Adresse) VALUES
                                    ('Dupont', 'Jean', 'jean.dupont@email.com', '0123456789', '15 rue de la Paix, 75001 Paris'),
                                    ('Martin', 'Sophie', 'sophie.martin@email.com', '0234567890', '25 avenue des Champs-Élysées, 75008 Paris');";
                                cmd.ExecuteNonQuery();

                                /// Ajouter des clients tests
                                cmd.CommandText = @"
                                    INSERT INTO Clients (Nom, Prenom, Email, Telephone, Adresse, TypeClient) VALUES
                                    ('Dubois', 'Marie', 'marie.dubois@email.com', '0345678901', '10 rue de Rivoli, 75004 Paris', 'Individuel'),
                                    ('Leroy', 'Pierre', 'pierre.leroy@email.com', '0456789012', '5 boulevard Saint-Germain, 75005 Paris', 'Entreprise');";
                                cmd.ExecuteNonQuery();
                            }
                        }

                        /// Vérifier s'il y a déjà des stations de métro dans la base
                        using (var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM StationsMetro;", connection))
                        {
                            long count = (long)checkCmd.ExecuteScalar();
                            if (count == 0)
                            {
                                /// Ajouter des stations de métro
                                cmd.CommandText = @"
                                    INSERT INTO StationsMetro (Nom, Ligne, Latitude, Longitude) VALUES
                                    ('Châtelet', '1,4,7,11,14', 48.8584, 2.3476),
                                    ('Gare de Lyon', '1,14', 48.8447, 2.3739),
                                    ('Opéra', '3,7,8', 48.8714, 2.3317),
                                    ('Bastille', '1,5,8', 48.8530, 2.3687),
                                    ('Saint-Lazare', '3,12,13,14', 48.8756, 2.3247);";
                                cmd.ExecuteNonQuery();

                                /// Ajouter des connexions entre stations (avec durée en minutes)
                                cmd.CommandText = @"
                                    INSERT INTO ConnexionsMetro (StationDepartId, StationArriveeId, DureeMinutes)
                                    SELECT 
                                        s1.Id as StationDepartId,
                                        s2.Id as StationArriveeId,
                                        CASE 
                                            WHEN s1.Nom = 'Châtelet' AND s2.Nom = 'Gare de Lyon' THEN 8
                                            WHEN s1.Nom = 'Châtelet' AND s2.Nom = 'Opéra' THEN 5
                                            WHEN s1.Nom = 'Châtelet' AND s2.Nom = 'Bastille' THEN 6
                                            WHEN s1.Nom = 'Gare de Lyon' AND s2.Nom = 'Bastille' THEN 7
                                            WHEN s1.Nom = 'Opéra' AND s2.Nom = 'Saint-Lazare' THEN 4
                                            ELSE 10
                                        END as DureeMinutes
                                    FROM StationsMetro s1
                                    CROSS JOIN StationsMetro s2
                                    WHERE s1.Id < s2.Id;";
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }
    }
}