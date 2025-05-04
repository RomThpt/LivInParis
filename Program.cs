using System;
using System.Collections.Generic;
using LivInParis.Models;
using LivInParis.Services;
using MySql.Data.MySqlClient;
using System.Data;

namespace LivInParis
{
    class Program
    {
        // Database connection settings
        private const string SERVER = "localhost";
        private const string DATABASE = "livinparis";
        private const string ADMIN_USER = "root";
        private const string ADMIN_PASSWORD = "Maman888"; // Mot de passe vide ou modifier selon votre configuration

        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.Title = "Liv'In Paris - Système de Gestion";

                // Créer et initialiser la base de données
                InitialiserBaseDeDonnees();

                // Démarrer l'interface console
                AfficherMenuPrincipal();
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
            Console.WriteLine("Initialisation de la base de données...");

            try
            {
                // Créer la connexion en mode admin
                using (var connection = new MySqlConnection(
                    $"Server={SERVER};Uid={ADMIN_USER};Pwd={ADMIN_PASSWORD};"))
                {
                    connection.Open();

                    // Créer la base de données si elle n'existe pas
                    using (var cmd = new MySqlCommand(
                        $"CREATE DATABASE IF NOT EXISTS {DATABASE} CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;",
                        connection))
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine($"Base de données '{DATABASE}' prête.");
                    }

                    // Utiliser la base de données
                    using (var cmd = new MySqlCommand($"USE {DATABASE}", connection))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Créer les tables si elles n'existent pas
                    CreerTablesSQL(connection);
                }

                // Vérifier si des données de test doivent être ajoutées
                VerifierEtAjouterDonneesTest();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur d'initialisation de la base de données: {ex.Message}");
                throw;
            }
        }

        static void CreerTablesSQL(MySqlConnection connection)
        {
            // Table Cuisiniers
            ExecuterCommandeSQL(connection, @"
                CREATE TABLE IF NOT EXISTS Cuisiniers (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Nom VARCHAR(100) NOT NULL,
                    Prenom VARCHAR(100) NOT NULL,
                    Email VARCHAR(150) NOT NULL UNIQUE,
                    Adresse TEXT NOT NULL,
                    NoteMoyenne FLOAT DEFAULT 0,
                    DateInscription DATETIME NOT NULL
                )");

            // Table Clients
            ExecuterCommandeSQL(connection, @"
                CREATE TABLE IF NOT EXISTS Clients (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Nom VARCHAR(100) NOT NULL,
                    Prenom VARCHAR(100) NOT NULL,
                    Email VARCHAR(150) NOT NULL UNIQUE,
                    Telephone VARCHAR(20) NOT NULL,
                    Adresse TEXT NOT NULL,
                    Type TINYINT NOT NULL,
                    NoteMoyenne FLOAT DEFAULT 0,
                    DateInscription DATETIME NOT NULL
                )");

            // Table Plats
            ExecuterCommandeSQL(connection, @"
                CREATE TABLE IF NOT EXISTS Plats (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Nom VARCHAR(150) NOT NULL,
                    Categorie TINYINT NOT NULL,
                    CuisinierID INT NOT NULL,
                    NationaliteCuisine VARCHAR(100) NOT NULL,
                    NombrePersonnes INT NOT NULL,
                    DatePreparation DATETIME NOT NULL,
                    DateExpiration DATETIME NOT NULL,
                    PrixParPersonne DECIMAL(10,2) NOT NULL,
                    PhotoUrl VARCHAR(255),
                    EstDisponible BOOLEAN DEFAULT TRUE,
                    FOREIGN KEY (CuisinierID) REFERENCES Cuisiniers(Id)
                )");

            // Table IngredientsPlat
            ExecuterCommandeSQL(connection, @"
                CREATE TABLE IF NOT EXISTS IngredientsPlat (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    PlatID INT NOT NULL,
                    NomIngredient VARCHAR(100) NOT NULL,
                    FOREIGN KEY (PlatID) REFERENCES Plats(Id) ON DELETE CASCADE
                )");

            // Table RegimesAlimentairesPlat
            ExecuterCommandeSQL(connection, @"
                CREATE TABLE IF NOT EXISTS RegimesAlimentairesPlat (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    PlatID INT NOT NULL,
                    TypeRegime VARCHAR(100) NOT NULL,
                    FOREIGN KEY (PlatID) REFERENCES Plats(Id) ON DELETE CASCADE
                )");

            // Table Commandes
            ExecuterCommandeSQL(connection, @"
                CREATE TABLE IF NOT EXISTS Commandes (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    ClientID INT NOT NULL,
                    DateCommande DATETIME NOT NULL,
                    DateLivraisonPrevue DATETIME NOT NULL,
                    PrixTotal DECIMAL(10,2) NOT NULL,
                    Statut TINYINT NOT NULL,
                    AdresseLivraison TEXT NOT NULL,
                    Commentaires TEXT,
                    FOREIGN KEY (ClientID) REFERENCES Clients(Id)
                )");

            // Table CommandeItems
            ExecuterCommandeSQL(connection, @"
                CREATE TABLE IF NOT EXISTS CommandeItems (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    CommandeID INT NOT NULL,
                    PlatID INT NOT NULL,
                    Quantite INT NOT NULL,
                    PrixUnitaire DECIMAL(10,2) NOT NULL,
                    FOREIGN KEY (CommandeID) REFERENCES Commandes(Id) ON DELETE CASCADE,
                    FOREIGN KEY (PlatID) REFERENCES Plats(Id)
                )");

            // Table Livraisons
            ExecuterCommandeSQL(connection, @"
                CREATE TABLE IF NOT EXISTS Livraisons (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    CommandeID INT NOT NULL UNIQUE,
                    DateDepart DATETIME NOT NULL,
                    DateArrivee DATETIME,
                    ItineraireSuggere TEXT,
                    Statut TINYINT NOT NULL,
                    CommentaireLivreur TEXT,
                    FOREIGN KEY (CommandeID) REFERENCES Commandes(Id) ON DELETE CASCADE
                )");

            // Table Notations
            ExecuterCommandeSQL(connection, @"
                CREATE TABLE IF NOT EXISTS Notations (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    CommandeID INT NOT NULL,
                    NoteurID INT NOT NULL,
                    NoteID INT NOT NULL,
                    Type TINYINT NOT NULL,
                    Note TINYINT NOT NULL,
                    Commentaire TEXT,
                    DateNotation DATETIME NOT NULL,
                    FOREIGN KEY (CommandeID) REFERENCES Commandes(Id) ON DELETE CASCADE
                )");

            // Table HistoriqueCommandes
            ExecuterCommandeSQL(connection, @"
                CREATE TABLE IF NOT EXISTS HistoriqueCommandes (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    CommandeID INT NOT NULL,
                    ActionDate DATETIME NOT NULL,
                    Statut TINYINT NOT NULL,
                    Commentaire TEXT,
                    FOREIGN KEY (CommandeID) REFERENCES Commandes(Id) ON DELETE CASCADE
                )");

            // Procédure stockée pour mettre à jour la note moyenne d'un cuisinier
            ExecuterCommandeSQL(connection, @"
                DROP PROCEDURE IF EXISTS MettreAJourNoteMoyenneCuisinier;
                CREATE PROCEDURE MettreAJourNoteMoyenneCuisinier(IN cuisinierID INT)
                BEGIN
                    UPDATE Cuisiniers 
                    SET NoteMoyenne = (
                        SELECT AVG(Note) 
                        FROM Notations 
                        WHERE Type = 0 AND NoteID = cuisinierID
                    )
                    WHERE Id = cuisinierID;
                END");

            // Procédure stockée pour mettre à jour la note moyenne d'un client
            ExecuterCommandeSQL(connection, @"
                DROP PROCEDURE IF EXISTS MettreAJourNoteMoyenneClient;
                CREATE PROCEDURE MettreAJourNoteMoyenneClient(IN clientID INT)
                BEGIN
                    UPDATE Clients 
                    SET NoteMoyenne = (
                        SELECT AVG(Note) 
                        FROM Notations 
                        WHERE Type = 1 AND NoteID = clientID
                    )
                    WHERE Id = clientID;
                END");

            // Procédure stockée pour obtenir les livraisons par cuisinier
            ExecuterCommandeSQL(connection, @"
                DROP PROCEDURE IF EXISTS ObtenirLivraisonsParCuisinier;
                CREATE PROCEDURE ObtenirLivraisonsParCuisinier()
                BEGIN
                    SELECT 
                        c.Id AS CuisinierID, 
                        CONCAT(c.Prenom, ' ', c.Nom) AS NomCuisinier,
                        COUNT(l.Id) AS NombreLivraisons
                    FROM Cuisiniers c
                    LEFT JOIN Plats p ON c.Id = p.CuisinierID
                    LEFT JOIN CommandeItems ci ON p.Id = ci.PlatID
                    LEFT JOIN Commandes cmd ON ci.CommandeID = cmd.Id
                    LEFT JOIN Livraisons l ON cmd.Id = l.CommandeID
                    GROUP BY c.Id
                    ORDER BY NombreLivraisons DESC;
                END");

            // Procédure stockée pour obtenir les commandes par période
            ExecuterCommandeSQL(connection, @"
                DROP PROCEDURE IF EXISTS ObtenirCommandesParPeriode;
                CREATE PROCEDURE ObtenirCommandesParPeriode(IN dateDebut DATETIME, IN dateFin DATETIME)
                BEGIN
                    SELECT 
                        cmd.Id AS CommandeID,
                        cmd.DateCommande,
                        CONCAT(cl.Prenom, ' ', cl.Nom) AS NomClient,
                        cmd.PrixTotal,
                        cmd.Statut
                    FROM Commandes cmd
                    JOIN Clients cl ON cmd.ClientID = cl.Id
                    WHERE cmd.DateCommande BETWEEN dateDebut AND dateFin
                    ORDER BY cmd.DateCommande;
                END");

            // Procédure stockée pour obtenir les commandes d'un client
            ExecuterCommandeSQL(connection, @"
                DROP PROCEDURE IF EXISTS ObtenirCommandesClient;
                CREATE PROCEDURE ObtenirCommandesClient(IN clientID INT)
                BEGIN
                    SELECT 
                        cmd.Id AS CommandeID,
                        cmd.DateCommande,
                        cmd.DateLivraisonPrevue,
                        cmd.PrixTotal,
                        cmd.Statut,
                        IFNULL(l.Statut, -1) AS StatutLivraison
                    FROM Commandes cmd
                    LEFT JOIN Livraisons l ON cmd.Id = l.CommandeID
                    WHERE cmd.ClientID = clientID
                    ORDER BY cmd.DateCommande DESC;
                END");

            // Tables pour le réseau de métro (pour l'optimisation des livraisons)
            ExecuterCommandeSQL(connection, @"
                CREATE TABLE IF NOT EXISTS MetroStations (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Nom VARCHAR(100) NOT NULL UNIQUE,
                    Latitude FLOAT,
                    Longitude FLOAT,
                    CodePostal VARCHAR(10)
                )");

            ExecuterCommandeSQL(connection, @"
                CREATE TABLE IF NOT EXISTS MetroConnexions (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    StationDepart VARCHAR(100) NOT NULL,
                    StationArrivee VARCHAR(100) NOT NULL,
                    LigneMetro VARCHAR(10),
                    Duree FLOAT NOT NULL,
                    Distance FLOAT,
                    UNIQUE (StationDepart, StationArrivee),
                    FOREIGN KEY (StationDepart) REFERENCES MetroStations(Nom),
                    FOREIGN KEY (StationArrivee) REFERENCES MetroStations(Nom)
                )");

            // Table pour stocker les plannings des cuisiniers
            ExecuterCommandeSQL(connection, @"
                CREATE TABLE IF NOT EXISTS PlanningCuisiniers (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    CuisinierID INT NOT NULL,
                    DateDebut DATETIME NOT NULL,
                    DateFin DATETIME NOT NULL,
                    GroupeColor INT NOT NULL,
                    FOREIGN KEY (CuisinierID) REFERENCES Cuisiniers(Id)
                )");

            Console.WriteLine("Structure de la base de données créée avec succès.");
        }

        static void ExecuterCommandeSQL(MySqlConnection connection, string commandeSQL)
        {
            using (var cmd = new MySqlCommand(commandeSQL, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        static void VerifierEtAjouterDonneesTest()
        {
            using (var connection = new MySqlConnection($"Server={SERVER};Database={DATABASE};Uid={ADMIN_USER};Pwd={ADMIN_PASSWORD};"))
            {
                connection.Open();

                // Vérifier s'il y a déjà des cuisiniers dans la base
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM Cuisiniers", connection))
                {
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count == 0)
                    {
                        Console.WriteLine("Ajout de données test...");
                        // Ajouter des cuisiniers tests
                        AjouterCuisinierTest(connection, "Dubois", "Marie", "marie.dubois@example.com", "12 rue de la Paix, 75001 Paris");
                        AjouterCuisinierTest(connection, "Martin", "Pierre", "pierre.martin@example.com", "45 avenue Victor Hugo, 75016 Paris");
                        AjouterCuisinierTest(connection, "Leroy", "Sophie", "sophie.leroy@example.com", "8 boulevard Saint-Michel, 75005 Paris");
                        AjouterCuisinierTest(connection, "Dupont", "Jean", "jean.dupont@example.com", "23 rue des Martyrs, 75009 Paris");

                        // Ajouter des clients tests
                        AjouterClientTest(connection, "Petit", "Antoine", "antoine.petit@example.com", "0678901234", "14 rue Montorgueil, 75002 Paris", TypeClient.Individuel);
                        AjouterClientTest(connection, "Moreau", "Camille", "camille.moreau@example.com", "0612345678", "67 rue de Rennes, 75006 Paris", TypeClient.Individuel);
                        AjouterClientTest(connection, "Bernard", "Eric", "eric.bernard@example.com", "0698765432", "5 place de la République, 75003 Paris", TypeClient.Individuel);
                        AjouterClientTest(connection, "Le Café Gourmand", "Contact", "contact@cafegourmand.fr", "0145678901", "34 rue de Rivoli, 75004 Paris", TypeClient.EntrepriseLocale);
                    }
                }

                // Vérifier s'il y a déjà des stations de métro dans la base
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM MetroStations", connection))
                {
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count == 0)
                    {
                        Console.WriteLine("Ajout des stations de métro...");
                        AjouterDonneesMetroTest(connection);
                    }
                }
            }
        }

        static void AjouterDonneesMetroTest(MySqlConnection connection)
        {
            // Ajouter des stations de métro
            string[] stations = {
                "Châtelet", "Hôtel de Ville", "Saint-Paul", "Bastille",
                "République", "Arts et Métiers", "Réaumur-Sébastopol",
                "Louvre-Rivoli", "Palais Royal", "Opéra", "Saint-Michel",
                "Odéon", "Cité", "Les Halles", "Strasbourg-Saint-Denis"
            };

            string[] codesPostaux = {
                "75001", "75004", "75004", "75004",
                "75003", "75003", "75003",
                "75001", "75001", "75002", "75006",
                "75006", "75001", "75001", "75002"
            };

            // Insérer les stations
            for (int i = 0; i < stations.Length; i++)
            {
                string query = @"
                    INSERT INTO MetroStations (Nom, CodePostal) 
                    VALUES (@Nom, @CodePostal)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Nom", stations[i]);
                    cmd.Parameters.AddWithValue("@CodePostal", codesPostaux[i]);
                    cmd.ExecuteNonQuery();
                }
            }

            // Ajouter des connexions entre stations (avec durée en minutes)
            var connexions = new[] {
                ("Châtelet", "Hôtel de Ville", 2.0),
                ("Châtelet", "Louvre-Rivoli", 2.5),
                ("Châtelet", "Saint-Michel", 3.0),
                ("Châtelet", "Les Halles", 1.5),
                ("Hôtel de Ville", "Saint-Paul", 2.0),
                ("Saint-Paul", "Bastille", 3.0),
                ("Bastille", "République", 5.0),
                ("République", "Arts et Métiers", 2.0),
                ("République", "Strasbourg-Saint-Denis", 2.5),
                ("Arts et Métiers", "Réaumur-Sébastopol", 2.0),
                ("Réaumur-Sébastopol", "Strasbourg-Saint-Denis", 1.5),
                ("Louvre-Rivoli", "Palais Royal", 1.5),
                ("Palais Royal", "Opéra", 3.0),
                ("Saint-Michel", "Odéon", 2.0),
                ("Saint-Michel", "Cité", 1.5),
                ("Cité", "Châtelet", 2.0)
            };

            // Insérer les connexions
            foreach (var (depart, arrivee, duree) in connexions)
            {
                string query = @"
                    INSERT INTO MetroConnexions (StationDepart, StationArrivee, Duree, Distance) 
                    VALUES (@StationDepart, @StationArrivee, @Duree, @Duree * 0.5)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StationDepart", depart);
                    cmd.Parameters.AddWithValue("@StationArrivee", arrivee);
                    cmd.Parameters.AddWithValue("@Duree", duree);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        static void AjouterCuisinierTest(MySqlConnection connection, string nom, string prenom, string email, string adresse)
        {
            string query = @"
                INSERT INTO Cuisiniers (Nom, Prenom, Email, Adresse, DateInscription) 
                VALUES (@Nom, @Prenom, @Email, @Adresse, @DateInscription)";

            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Nom", nom);
                cmd.Parameters.AddWithValue("@Prenom", prenom);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Adresse", adresse);
                cmd.Parameters.AddWithValue("@DateInscription", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }

        static void AjouterClientTest(MySqlConnection connection, string nom, string prenom, string email, string telephone, string adresse, TypeClient type)
        {
            string query = @"
                INSERT INTO Clients (Nom, Prenom, Email, Telephone, Adresse, Type, DateInscription) 
                VALUES (@Nom, @Prenom, @Email, @Telephone, @Adresse, @Type, @DateInscription)";

            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Nom", nom);
                cmd.Parameters.AddWithValue("@Prenom", prenom);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Telephone", telephone);
                cmd.Parameters.AddWithValue("@Adresse", adresse);
                cmd.Parameters.AddWithValue("@Type", (int)type);
                cmd.Parameters.AddWithValue("@DateInscription", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }

        static void AfficherMenuPrincipal()
        {
            var dbService = new DatabaseService(SERVER, DATABASE, ADMIN_USER, ADMIN_PASSWORD);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("┌───────────────────────────────────────┐");
                Console.WriteLine("│           LIV'IN PARIS               │");
                Console.WriteLine("│    SYSTÈME DE GESTION DE REPAS        │");
                Console.WriteLine("└───────────────────────────────────────┘");
                Console.WriteLine();
                Console.WriteLine("1. Gérer les plats");
                Console.WriteLine("2. Gérer les commandes");
                Console.WriteLine("3. Gérer les notations");
                Console.WriteLine("4. Statistiques");
                Console.WriteLine("5. Import/Export de données");
                Console.WriteLine("0. Quitter");
                Console.WriteLine();
                Console.Write("Votre choix: ");

                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        GererPlats(dbService);
                        break;
                    case "2":
                        GererCommandes(dbService);
                        break;
                    case "3":
                        GererNotations(dbService);
                        break;
                    case "4":
                        AfficherStatistiques(dbService);
                        break;
                    case "5":
                        GererImportExport(dbService);
                        break;
                    case "0":
                        Console.WriteLine("Au revoir!");
                        return;
                    default:
                        Console.WriteLine("Option non valide. Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void GererPlats(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│           GESTION DES PLATS           │");
            Console.WriteLine("└───────────────────────────────────────┘");
            Console.WriteLine();
            Console.WriteLine("1. Créer un nouveau plat");
            Console.WriteLine("2. Rechercher des plats");
            Console.WriteLine("0. Retour au menu principal");
            Console.WriteLine();
            Console.Write("Votre choix: ");

            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    CreerNouveauPlat(dbService);
                    break;
                case "2":
                    RechercherPlats(dbService);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Option non valide. Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    break;
            }
        }

        static void CreerNouveauPlat(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│           CRÉATION D'UN PLAT          │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                // Récupérer la liste des cuisiniers
                var cuisiniers = ObtenirListeCuisiniers(dbService);
                if (cuisiniers.Count == 0)
                {
                    Console.WriteLine("Aucun cuisinier n'est disponible. Impossible de créer un plat.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }

                // Création d'un nouveau plat
                var plat = new Plat
                {
                    DatePreparation = DateTime.Now,
                    EstDisponible = true,
                    IngredientsPhares = new List<string>(),
                    RegimesAlimentaires = new List<string>()
                };

                // Nom du plat
                Console.Write("Nom du plat: ");
                plat.Nom = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(plat.Nom))
                {
                    Console.WriteLine("Le nom du plat ne peut pas être vide.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }

                // Catégorie
                Console.WriteLine("Catégorie:");
                Console.WriteLine("1. Entrée");
                Console.WriteLine("2. Plat principal");
                Console.WriteLine("3. Dessert");
                Console.Write("Votre choix: ");
                string choixCategorie = Console.ReadLine();
                switch (choixCategorie)
                {
                    case "1": plat.Categorie = CategoriePlat.Entree; break;
                    case "2": plat.Categorie = CategoriePlat.PlatPrincipal; break;
                    case "3": plat.Categorie = CategoriePlat.Dessert; break;
                    default:
                        Console.WriteLine("Catégorie invalide.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                }

                // Cuisinier
                Console.WriteLine("Sélection du cuisinier:");
                for (int i = 0; i < cuisiniers.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {cuisiniers[i].Prenom} {cuisiniers[i].Nom}");
                }
                Console.Write("Votre choix: ");
                if (int.TryParse(Console.ReadLine(), out int indexCuisinier) &&
                    indexCuisinier >= 1 && indexCuisinier <= cuisiniers.Count)
                {
                    plat.CuisinierID = cuisiniers[indexCuisinier - 1].Id;
                }
                else
                {
                    Console.WriteLine("Choix de cuisinier invalide.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }

                // Nationalité de la cuisine
                Console.Write("Nationalité de la cuisine: ");
                plat.NationaliteCuisine = Console.ReadLine();

                // Nombre de personnes
                Console.Write("Nombre de personnes: ");
                if (!int.TryParse(Console.ReadLine(), out int nombrePersonnes) || nombrePersonnes <= 0)
                {
                    Console.WriteLine("Nombre de personnes invalide.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }
                plat.NombrePersonnes = nombrePersonnes;

                // Date d'expiration
                Console.Write("Nombre de jours avant expiration: ");
                if (int.TryParse(Console.ReadLine(), out int joursExpiration) && joursExpiration > 0)
                {
                    plat.DateExpiration = DateTime.Now.AddDays(joursExpiration);
                }
                else
                {
                    Console.WriteLine("Nombre de jours invalide.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }

                // Prix par personne
                Console.Write("Prix par personne (€): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal prix) || prix <= 0)
                {
                    Console.WriteLine("Prix invalide.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }
                plat.PrixParPersonne = prix;

                // Ingrédients phares
                Console.WriteLine("Saisie des ingrédients phares (terminez par une ligne vide):");
                while (true)
                {
                    Console.Write("Ingrédient: ");
                    string ingredient = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(ingredient))
                        break;
                    plat.IngredientsPhares.Add(ingredient);
                }

                // Régimes alimentaires
                Console.WriteLine("Saisie des régimes alimentaires (terminez par une ligne vide):");
                Console.WriteLine("Exemples: Végétarien, Sans Gluten, Végan, etc.");
                while (true)
                {
                    Console.Write("Régime: ");
                    string regime = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(regime))
                        break;
                    plat.RegimesAlimentaires.Add(regime);
                }

                // URL de la photo
                Console.Write("URL de la photo (facultatif): ");
                string photoUrl = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(photoUrl))
                {
                    plat.PhotoUrl = photoUrl;
                }

                // Enregistrer le plat dans la base de données
                int platID = dbService.InsererPlat(plat);
                Console.WriteLine($"Plat '{plat.Nom}' créé avec succès (ID: {platID}).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création du plat: {ex.Message}");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        // Méthode utilitaire pour obtenir la liste des cuisiniers
        static List<Cuisinier> ObtenirListeCuisiniers(DatabaseService dbService)
        {
            var cuisiniers = new List<Cuisinier>();
            using (var connection = new MySqlConnection(
                $"Server={SERVER};Database={DATABASE};Uid={ADMIN_USER};Pwd={ADMIN_PASSWORD};"))
            {
                connection.Open();
                string query = "SELECT Id, Nom, Prenom FROM Cuisiniers ORDER BY Nom, Prenom";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cuisiniers.Add(new Cuisinier
                            {
                                Id = reader.GetInt32("Id"),
                                Nom = reader.GetString("Nom"),
                                Prenom = reader.GetString("Prenom")
                            });
                        }
                    }
                }
            }
            return cuisiniers;
        }

        static void RechercherPlats(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│           RECHERCHE DE PLATS          │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                // Critères de recherche
                string nomFiltre = null;
                CategoriePlat? categorie = null;
                string nationalite = null;
                decimal? prixMax = null;
                string ingredient = null;
                string regime = null;

                // Menu de recherche
                Console.WriteLine("Filtres de recherche (laissez vide pour ignorer):");

                Console.Write("Nom du plat contient: ");
                string saisieNom = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(saisieNom))
                    nomFiltre = saisieNom;

                Console.WriteLine("Catégorie:");
                Console.WriteLine("0. Tous");
                Console.WriteLine("1. Entrée");
                Console.WriteLine("2. Plat principal");
                Console.WriteLine("3. Dessert");
                Console.Write("Votre choix: ");
                if (int.TryParse(Console.ReadLine(), out int choixCategorie) && choixCategorie >= 1 && choixCategorie <= 3)
                {
                    categorie = (CategoriePlat)(choixCategorie - 1);
                }

                Console.Write("Nationalité de cuisine contient: ");
                string saisieNationalite = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(saisieNationalite))
                    nationalite = saisieNationalite;

                Console.Write("Prix maximum par personne (€): ");
                string saisiePrix = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(saisiePrix) && decimal.TryParse(saisiePrix, out decimal prix) && prix > 0)
                    prixMax = prix;

                Console.Write("Ingrédient contient: ");
                string saisieIngredient = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(saisieIngredient))
                    ingredient = saisieIngredient;

                Console.Write("Régime alimentaire (ex: Végétarien): ");
                string saisieRegime = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(saisieRegime))
                    regime = saisieRegime;

                // Exécuter la recherche
                var plats = dbService.RechercherPlats(nomFiltre, categorie, nationalite, prixMax, ingredient, regime);

                // Afficher les résultats
                Console.WriteLine($"\n{plats.Count} plat(s) trouvé(s):");
                Console.WriteLine("---------------------------------------------------");

                if (plats.Count == 0)
                {
                    Console.WriteLine("Aucun plat ne correspond aux critères de recherche.");
                }
                else
                {
                    foreach (var plat in plats)
                    {
                        Console.WriteLine($"ID: {plat.Id}");
                        Console.WriteLine($"Nom: {plat.Nom}");
                        Console.WriteLine($"Catégorie: {plat.Categorie}");
                        Console.WriteLine($"Cuisinier: {plat.Cuisinier.Prenom} {plat.Cuisinier.Nom}");
                        Console.WriteLine($"Nationalité: {plat.NationaliteCuisine}");
                        Console.WriteLine($"Prix par personne: {plat.PrixParPersonne:C}");
                        Console.WriteLine($"Nombre de personnes: {plat.NombrePersonnes}");
                        Console.WriteLine($"Préparé le: {plat.DatePreparation:dd/MM/yyyy}");
                        Console.WriteLine($"À consommer avant: {plat.DateExpiration:dd/MM/yyyy}");

                        if (plat.IngredientsPhares.Count > 0)
                        {
                            Console.WriteLine($"Ingrédients: {string.Join(", ", plat.IngredientsPhares)}");
                        }

                        if (plat.RegimesAlimentaires.Count > 0)
                        {
                            Console.WriteLine($"Régimes: {string.Join(", ", plat.RegimesAlimentaires)}");
                        }

                        Console.WriteLine("---------------------------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la recherche: {ex.Message}");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void GererCommandes(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│         GESTION DES COMMANDES         │");
            Console.WriteLine("└───────────────────────────────────────┘");
            Console.WriteLine();
            Console.WriteLine("1. Créer une nouvelle commande");
            Console.WriteLine("2. Valider une commande");
            Console.WriteLine("3. Consulter l'historique des commandes");
            Console.WriteLine("0. Retour au menu principal");
            Console.WriteLine();
            Console.Write("Votre choix: ");

            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    CreerNouvelleCommande(dbService);
                    break;
                case "2":
                    ValiderCommande(dbService);
                    break;
                case "3":
                    ConsulterHistoriqueCommandes(dbService);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Option non valide. Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    break;
            }
        }

        static void CreerNouvelleCommande(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│       CRÉATION D'UNE COMMANDE         │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                // 1. Sélectionner un client
                Console.WriteLine("Sélection du client:");

                using (var connection = new MySqlConnection(
                    $"Server={SERVER};Database={DATABASE};Uid={ADMIN_USER};Pwd={ADMIN_PASSWORD};"))
                {
                    connection.Open();

                    // Récupérer la liste des clients
                    var clients = new List<(int Id, string Nom, string Prenom, string Adresse)>();
                    using (var cmd = new MySqlCommand("SELECT Id, Nom, Prenom, Adresse FROM Clients ORDER BY Nom", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                clients.Add((
                                    reader.GetInt32("Id"),
                                    reader.GetString("Nom"),
                                    reader.GetString("Prenom"),
                                    reader.GetString("Adresse")
                                ));
                            }
                        }
                    }

                    if (clients.Count == 0)
                    {
                        Console.WriteLine("Aucun client n'est disponible. Impossible de créer une commande.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }

                    for (int i = 0; i < clients.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {clients[i].Prenom} {clients[i].Nom}");
                    }

                    Console.Write("Votre choix: ");
                    if (!int.TryParse(Console.ReadLine(), out int indexClient) ||
                        indexClient < 1 || indexClient > clients.Count)
                    {
                        Console.WriteLine("Choix de client invalide.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }

                    var clientSelectionne = clients[indexClient - 1];

                    // 2. Créer la commande
                    var commande = new Commande
                    {
                        ClientID = clientSelectionne.Id,
                        DateCommande = DateTime.Now,
                        Statut = StatutCommande.EnAttente,
                        Items = new List<CommandeItem>()
                    };

                    // 3. Configurer l'adresse de livraison
                    Console.WriteLine("\nAdresse de livraison:");
                    Console.WriteLine($"1. Utiliser l'adresse du client: {clientSelectionne.Adresse}");
                    Console.WriteLine("2. Spécifier une autre adresse");
                    Console.Write("Votre choix: ");

                    string choixAdresse = Console.ReadLine();
                    if (choixAdresse == "1")
                    {
                        commande.AdresseLivraison = clientSelectionne.Adresse;
                    }
                    else if (choixAdresse == "2")
                    {
                        Console.Write("Nouvelle adresse de livraison: ");
                        string nouvelleAdresse = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(nouvelleAdresse))
                        {
                            Console.WriteLine("L'adresse ne peut pas être vide.");
                            Console.WriteLine("Appuyez sur une touche pour continuer...");
                            Console.ReadKey();
                            return;
                        }
                        commande.AdresseLivraison = nouvelleAdresse;
                    }
                    else
                    {
                        Console.WriteLine("Choix invalide.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }

                    // 4. Date de livraison prévue
                    Console.Write("\nJours avant livraison (1-7): ");
                    if (!int.TryParse(Console.ReadLine(), out int joursLivraison) ||
                        joursLivraison < 1 || joursLivraison > 7)
                    {
                        Console.WriteLine("Nombre de jours invalide.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }
                    commande.DateLivraisonPrevue = DateTime.Now.AddDays(joursLivraison);

                    // 5. Ajouter un plat à la commande
                    Console.WriteLine("\nAjout d'un plat à la commande:");

                    // Récupérer la liste des plats disponibles
                    var plats = new List<(int Id, string Nom, decimal Prix)>();
                    using (var cmd = new MySqlCommand("SELECT Id, Nom, PrixParPersonne FROM Plats WHERE EstDisponible = 1", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                plats.Add((
                                    reader.GetInt32("Id"),
                                    reader.GetString("Nom"),
                                    reader.GetDecimal("PrixParPersonne")
                                ));
                            }
                        }
                    }

                    if (plats.Count == 0)
                    {
                        Console.WriteLine("Aucun plat disponible. Impossible de créer la commande.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }

                    Console.WriteLine("Plats disponibles:");
                    for (int i = 0; i < plats.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {plats[i].Nom} - {plats[i].Prix:C} par personne");
                    }

                    Console.Write("Sélectionner un plat: ");
                    if (!int.TryParse(Console.ReadLine(), out int indexPlat) ||
                        indexPlat < 1 || indexPlat > plats.Count)
                    {
                        Console.WriteLine("Choix de plat invalide.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }

                    var platSelectionne = plats[indexPlat - 1];

                    // Quantité
                    Console.Write($"Quantité de \"{platSelectionne.Nom}\" (1-10): ");
                    if (!int.TryParse(Console.ReadLine(), out int quantite) ||
                        quantite < 1 || quantite > 10)
                    {
                        Console.WriteLine("Quantité invalide.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }

                    // Ajouter l'item à la commande
                    var commandeItem = new CommandeItem
                    {
                        PlatID = platSelectionne.Id,
                        Quantite = quantite,
                        PrixUnitaire = platSelectionne.Prix
                    };

                    commande.Items.Add(commandeItem);
                    commande.PrixTotal = commandeItem.PrixUnitaire * commandeItem.Quantite;

                    // 6. Commentaires
                    Console.Write("\nCommentaires (facultatif): ");
                    string commentaires = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(commentaires))
                    {
                        commande.Commentaires = commentaires;
                    }

                    // 7. Résumé de la commande
                    Console.WriteLine("\nRésumé de la commande:");
                    Console.WriteLine($"Client: {clientSelectionne.Prenom} {clientSelectionne.Nom}");
                    Console.WriteLine($"Adresse de livraison: {commande.AdresseLivraison}");
                    Console.WriteLine($"Date de livraison prévue: {commande.DateLivraisonPrevue:dd/MM/yyyy}");
                    Console.WriteLine($"Plat: {platSelectionne.Nom} x{quantite} = {commande.PrixTotal:C}");

                    // 8. Confirmation
                    Console.Write("\nConfirmer la commande? (O/N): ");
                    if (Console.ReadLine().Trim().ToUpper().StartsWith("O"))
                    {
                        // Enregistrer la commande
                        int commandeID = dbService.CreerCommande(commande);
                        Console.WriteLine($"Commande créée avec succès (ID: {commandeID}).");
                    }
                    else
                    {
                        Console.WriteLine("Création de commande annulée.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création de la commande: {ex.Message}");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void ValiderCommande(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│      VALIDATION D'UNE COMMANDE        │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                using (var connection = new MySqlConnection(
                    $"Server={SERVER};Database={DATABASE};Uid={ADMIN_USER};Pwd={ADMIN_PASSWORD};"))
                {
                    connection.Open();

                    // 1. Récupérer les commandes en attente
                    var commandes = new List<(int Id, string NomClient, decimal PrixTotal)>();
                    string query = @"
                        SELECT c.Id, cl.Nom, cl.Prenom, c.PrixTotal 
                        FROM Commandes c
                        JOIN Clients cl ON c.ClientID = cl.Id
                        WHERE c.Statut = @Statut
                        ORDER BY c.DateCommande";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Statut", (int)StatutCommande.EnAttente);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                commandes.Add((
                                    reader.GetInt32("Id"),
                                    $"{reader.GetString("Prenom")} {reader.GetString("Nom")}",
                                    reader.GetDecimal("PrixTotal")
                                ));
                            }
                        }
                    }

                    if (commandes.Count == 0)
                    {
                        Console.WriteLine("Aucune commande en attente.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }

                    Console.WriteLine("Commandes en attente:");
                    for (int i = 0; i < commandes.Count; i++)
                    {
                        var commande = commandes[i];
                        Console.WriteLine($"{i + 1}. Commande #{commande.Id} - Client: {commande.NomClient} - Prix: {commande.PrixTotal:C}");
                    }

                    // 2. Sélectionner une commande
                    Console.Write("\nSélectionner une commande à valider (0 pour annuler): ");
                    if (!int.TryParse(Console.ReadLine(), out int indexCommande) ||
                        indexCommande < 0 || indexCommande > commandes.Count)
                    {
                        Console.WriteLine("Choix invalide.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }

                    if (indexCommande == 0)
                    {
                        return;
                    }

                    var commandeSelectionnee = commandes[indexCommande - 1];

                    // 3. Créer la livraison
                    var livraison = new Livraison
                    {
                        CommandeID = commandeSelectionnee.Id,
                        DateDepart = DateTime.Now.AddHours(1), // Par défaut dans 1 heure
                        Statut = StatutLivraison.Planifiee
                    };

                    // 4. Configurer l'itinéraire suggéré
                    Console.Write("Itinéraire suggéré (facultatif): ");
                    string itineraire = Console.ReadLine();
                    if (!string.IsNullOrEmpty(itineraire))
                    {
                        livraison.ItineraireSuggere = itineraire;
                    }

                    // 5. Confirmation
                    Console.WriteLine("\nRésumé de la validation:");
                    Console.WriteLine($"Commande #{commandeSelectionnee.Id} - Client: {commandeSelectionnee.NomClient}");
                    Console.WriteLine($"Date de départ: {livraison.DateDepart:dd/MM/yyyy HH:mm}");

                    if (!string.IsNullOrEmpty(livraison.ItineraireSuggere))
                    {
                        Console.WriteLine($"Itinéraire suggéré: {livraison.ItineraireSuggere}");
                    }

                    Console.Write("\nConfirmer la validation? (O/N): ");
                    if (Console.ReadLine().Trim().ToUpper().StartsWith("O"))
                    {
                        // Valider la commande
                        dbService.ValiderCommandeEtCreerLivraison(commandeSelectionnee.Id, livraison);
                        Console.WriteLine("Commande validée avec succès et livraison planifiée.");
                    }
                    else
                    {
                        Console.WriteLine("Validation de commande annulée.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la validation de la commande: {ex.Message}");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void ConsulterHistoriqueCommandes(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│      HISTORIQUE DES COMMANDES         │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                // 1. Demander la période
                Console.WriteLine("Sélectionner la période:");
                Console.WriteLine("1. Aujourd'hui");
                Console.WriteLine("2. Cette semaine");
                Console.WriteLine("3. Ce mois");
                Console.WriteLine("4. Personnalisée");
                Console.Write("Votre choix: ");

                string choix = Console.ReadLine();
                DateTime dateDebut;
                DateTime dateFin = DateTime.Now;

                switch (choix)
                {
                    case "1": // Aujourd'hui
                        dateDebut = DateTime.Today;
                        break;
                    case "2": // Cette semaine
                        dateDebut = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                        break;
                    case "3": // Ce mois
                        dateDebut = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        break;
                    case "4": // Personnalisée
                        Console.Write("Date de début (JJ/MM/AAAA): ");
                        if (!DateTime.TryParse(Console.ReadLine(), out dateDebut))
                        {
                            Console.WriteLine("Format de date invalide.");
                            Console.WriteLine("Appuyez sur une touche pour continuer...");
                            Console.ReadKey();
                            return;
                        }

                        Console.Write("Date de fin (JJ/MM/AAAA): ");
                        if (!DateTime.TryParse(Console.ReadLine(), out dateFin))
                        {
                            Console.WriteLine("Format de date invalide.");
                            Console.WriteLine("Appuyez sur une touche pour continuer...");
                            Console.ReadKey();
                            return;
                        }
                        break;
                    default:
                        Console.WriteLine("Choix invalide.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                }

                // 2. Récupérer les commandes
                var dataTable = dbService.ObtenirCommandesParPeriode(dateDebut, dateFin);

                // 3. Afficher les résultats
                Console.WriteLine($"\nCommandes du {dateDebut:dd/MM/yyyy} au {dateFin:dd/MM/yyyy}:");
                Console.WriteLine("---------------------------------------------------");

                if (dataTable.Rows.Count == 0)
                {
                    Console.WriteLine("Aucune commande trouvée pour cette période.");
                }
                else
                {
                    decimal totalPeriode = 0;

                    Console.WriteLine($"{"ID",-5} {"Date",-12} {"Client",-30} {"Prix",-10} {"Statut",-15}");
                    Console.WriteLine(new string('-', 75));

                    foreach (DataRow row in dataTable.Rows)
                    {
                        int commandeID = Convert.ToInt32(row["CommandeID"]);
                        DateTime dateCommande = Convert.ToDateTime(row["DateCommande"]);
                        string nomClient = row["NomClient"].ToString();
                        decimal prixTotal = Convert.ToDecimal(row["PrixTotal"]);
                        StatutCommande statut = (StatutCommande)Convert.ToInt32(row["Statut"]);

                        Console.WriteLine($"{commandeID,-5} {dateCommande:dd/MM/yyyy} {nomClient,-30} {prixTotal:C}  {statut}");

                        totalPeriode += prixTotal;
                    }

                    Console.WriteLine(new string('-', 75));
                    Console.WriteLine($"Total: {totalPeriode:C} pour {dataTable.Rows.Count} commande(s)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la consultation de l'historique: {ex.Message}");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void GererNotations(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│         GESTION DES NOTATIONS         │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                using (var connection = new MySqlConnection(
                    $"Server={SERVER};Database={DATABASE};Uid={ADMIN_USER};Pwd={ADMIN_PASSWORD};"))
                {
                    connection.Open();

                    // 1. Demander le type de notation
                    Console.WriteLine("Type de notation:");
                    Console.WriteLine("1. Client note un cuisinier");
                    Console.WriteLine("2. Cuisinier note un client");
                    Console.Write("Votre choix: ");

                    string choixType = Console.ReadLine();
                    TypeNotation typeNotation;

                    if (choixType == "1")
                    {
                        typeNotation = TypeNotation.ClientVersCuisinier;
                    }
                    else if (choixType == "2")
                    {
                        typeNotation = TypeNotation.CuisinierVersClient;
                    }
                    else
                    {
                        Console.WriteLine("Choix invalide.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }

                    // 2. Sélectionner la commande livrée
                    var commandes = new List<(int Id, int ClientId, string NomClient, int CuisinierID, string NomCuisinier)>();
                    string query = @"
                        SELECT c.Id, c.ClientID, cl.Nom as ClientNom, cl.Prenom as ClientPrenom,
                             p.CuisinierID, cu.Nom as CuisinierNom, cu.Prenom as CuisinierPrenom
                        FROM Commandes c
                        JOIN Clients cl ON c.ClientID = cl.Id
                        JOIN CommandeItems ci ON c.Id = ci.CommandeID
                        JOIN Plats p ON ci.PlatID = p.Id
                        JOIN Cuisiniers cu ON p.CuisinierID = cu.Id
                        JOIN Livraisons l ON c.Id = l.CommandeID
                        WHERE c.Statut = @Statut AND l.Statut = @StatutLivraison
                        GROUP BY c.Id
                        ORDER BY c.DateCommande DESC";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Statut", (int)StatutCommande.Livree);
                        cmd.Parameters.AddWithValue("@StatutLivraison", (int)StatutLivraison.Terminee);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                commandes.Add((
                                    reader.GetInt32("Id"),
                                    reader.GetInt32("ClientID"),
                                    $"{reader.GetString("ClientPrenom")} {reader.GetString("ClientNom")}",
                                    reader.GetInt32("CuisinierID"),
                                    $"{reader.GetString("CuisinierPrenom")} {reader.GetString("CuisinierNom")}"
                                ));
                            }
                        }
                    }

                    if (commandes.Count == 0)
                    {
                        Console.WriteLine("Aucune commande livrée n'est disponible pour notation.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }

                    Console.WriteLine("\nCommandes livrées disponibles pour notation:");
                    for (int i = 0; i < commandes.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. Commande #{commandes[i].Id} - Client: {commandes[i].NomClient} - Cuisinier: {commandes[i].NomCuisinier}");
                    }

                    Console.Write("\nSélectionner une commande: ");
                    if (!int.TryParse(Console.ReadLine(), out int indexCommande) ||
                        indexCommande < 1 || indexCommande > commandes.Count)
                    {
                        Console.WriteLine("Choix invalide.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }

                    var commandeSelectionnee = commandes[indexCommande - 1];

                    // 3. Créer la notation
                    var notation = new Notation
                    {
                        CommandeID = commandeSelectionnee.Id,
                        Type = typeNotation,
                        DateNotation = DateTime.Now
                    };

                    // 4. Configurer le noteur et la personne notée
                    if (typeNotation == TypeNotation.ClientVersCuisinier)
                    {
                        notation.NoteurID = commandeSelectionnee.ClientId;
                        notation.NoteID = commandeSelectionnee.CuisinierID;
                        Console.WriteLine($"\nClient {commandeSelectionnee.NomClient} note Cuisinier {commandeSelectionnee.NomCuisinier}");
                    }
                    else
                    {
                        notation.NoteurID = commandeSelectionnee.CuisinierID;
                        notation.NoteID = commandeSelectionnee.ClientId;
                        Console.WriteLine($"\nCuisinier {commandeSelectionnee.NomCuisinier} note Client {commandeSelectionnee.NomClient}");
                    }

                    // 5. Demander la note
                    Console.Write("Note (1-5): ");
                    if (!int.TryParse(Console.ReadLine(), out int note) || note < 1 || note > 5)
                    {
                        Console.WriteLine("Note invalide. Veuillez entrer un nombre entre 1 et 5.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }
                    notation.Note = note;

                    // 6. Commentaire
                    Console.Write("Commentaire (facultatif): ");
                    string commentaire = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(commentaire))
                    {
                        notation.Commentaire = commentaire;
                    }

                    // 7. Confirmation
                    Console.WriteLine("\nRésumé de la notation:");
                    if (typeNotation == TypeNotation.ClientVersCuisinier)
                    {
                        Console.WriteLine($"Client {commandeSelectionnee.NomClient} donne une note de {notation.Note}/5 au cuisinier {commandeSelectionnee.NomCuisinier}");
                    }
                    else
                    {
                        Console.WriteLine($"Cuisinier {commandeSelectionnee.NomCuisinier} donne une note de {notation.Note}/5 au client {commandeSelectionnee.NomClient}");
                    }

                    if (!string.IsNullOrEmpty(notation.Commentaire))
                    {
                        Console.WriteLine($"Commentaire: {notation.Commentaire}");
                    }

                    Console.Write("\nConfirmer cette notation? (O/N): ");
                    if (Console.ReadLine().Trim().ToUpper().StartsWith("O"))
                    {
                        // Enregistrer la notation
                        dbService.AjouterNotation(notation);
                        Console.WriteLine("Notation ajoutée avec succès.");
                    }
                    else
                    {
                        Console.WriteLine("Ajout de notation annulé.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la gestion des notations: {ex.Message}");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void AfficherStatistiques(DatabaseService dbService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("┌───────────────────────────────────────┐");
                Console.WriteLine("│             STATISTIQUES              │");
                Console.WriteLine("└───────────────────────────────────────┘");
                Console.WriteLine();
                Console.WriteLine("1. Livraisons par cuisinier");
                Console.WriteLine("2. Commandes par période");
                Console.WriteLine("3. Commandes d'un client");
                Console.WriteLine("0. Retour au menu principal");
                Console.WriteLine();
                Console.Write("Votre choix: ");

                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        AfficherLivraisonsParCuisinier(dbService);
                        break;
                    case "2":
                        AfficherCommandesParPeriode(dbService);
                        break;
                    case "3":
                        AfficherCommandesClient(dbService);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Option non valide. Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void AfficherLivraisonsParCuisinier(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│       LIVRAISONS PAR CUISINIER        │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                var dataTable = dbService.ObtenirLivraisonsParCuisinier();

                if (dataTable.Rows.Count == 0)
                {
                    Console.WriteLine("Aucune donnée disponible.");
                }
                else
                {
                    Console.WriteLine($"{"ID",-5} {"Cuisinier",-30} {"Nombre de livraisons",-20}");
                    Console.WriteLine(new string('-', 60));

                    foreach (DataRow row in dataTable.Rows)
                    {
                        int cuisinierID = Convert.ToInt32(row["CuisinierID"]);
                        string nomCuisinier = row["NomCuisinier"].ToString();
                        int nombreLivraisons = Convert.ToInt32(row["NombreLivraisons"]);

                        Console.WriteLine($"{cuisinierID,-5} {nomCuisinier,-30} {nombreLivraisons,-20}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'affichage des statistiques: {ex.Message}");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void AfficherCommandesParPeriode(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│        COMMANDES PAR PÉRIODE          │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                // Demander la période
                Console.WriteLine("Sélectionner la période:");
                Console.WriteLine("1. Aujourd'hui");
                Console.WriteLine("2. Cette semaine");
                Console.WriteLine("3. Ce mois");
                Console.WriteLine("4. Personnalisée");
                Console.Write("Votre choix: ");

                string choix = Console.ReadLine();
                DateTime dateDebut;
                DateTime dateFin = DateTime.Now;

                switch (choix)
                {
                    case "1": // Aujourd'hui
                        dateDebut = DateTime.Today;
                        break;
                    case "2": // Cette semaine
                        dateDebut = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                        break;
                    case "3": // Ce mois
                        dateDebut = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        break;
                    case "4": // Personnalisée
                        Console.Write("Date de début (JJ/MM/AAAA): ");
                        if (!DateTime.TryParse(Console.ReadLine(), out dateDebut))
                        {
                            Console.WriteLine("Format de date invalide.");
                            Console.WriteLine("Appuyez sur une touche pour continuer...");
                            Console.ReadKey();
                            return;
                        }

                        Console.Write("Date de fin (JJ/MM/AAAA): ");
                        if (!DateTime.TryParse(Console.ReadLine(), out dateFin))
                        {
                            Console.WriteLine("Format de date invalide.");
                            Console.WriteLine("Appuyez sur une touche pour continuer...");
                            Console.ReadKey();
                            return;
                        }
                        break;
                    default:
                        Console.WriteLine("Choix invalide.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                }

                // Récupérer les commandes
                var dataTable = dbService.ObtenirCommandesParPeriode(dateDebut, dateFin);

                Console.WriteLine($"\nCommandes du {dateDebut:dd/MM/yyyy} au {dateFin:dd/MM/yyyy}:");
                Console.WriteLine("---------------------------------------------------");

                if (dataTable.Rows.Count == 0)
                {
                    Console.WriteLine("Aucune commande trouvée pour cette période.");
                }
                else
                {
                    decimal totalPeriode = 0;

                    Console.WriteLine($"{"ID",-5} {"Date",-12} {"Client",-30} {"Prix",-10} {"Statut",-15}");
                    Console.WriteLine(new string('-', 75));

                    foreach (DataRow row in dataTable.Rows)
                    {
                        int commandeID = Convert.ToInt32(row["CommandeID"]);
                        DateTime dateCommande = Convert.ToDateTime(row["DateCommande"]);
                        string nomClient = row["NomClient"].ToString();
                        decimal prixTotal = Convert.ToDecimal(row["PrixTotal"]);
                        StatutCommande statut = (StatutCommande)Convert.ToInt32(row["Statut"]);

                        Console.WriteLine($"{commandeID,-5} {dateCommande:dd/MM/yyyy} {nomClient,-30} {prixTotal:C}  {statut}");

                        totalPeriode += prixTotal;
                    }

                    Console.WriteLine(new string('-', 75));
                    Console.WriteLine($"Total: {totalPeriode:C} pour {dataTable.Rows.Count} commande(s)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'affichage des statistiques: {ex.Message}");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void AfficherCommandesClient(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│        COMMANDES D'UN CLIENT          │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                using (var connection = new MySqlConnection(
                    $"Server={SERVER};Database={DATABASE};Uid={ADMIN_USER};Pwd={ADMIN_PASSWORD};"))
                {
                    connection.Open();

                    // 1. Sélectionner un client
                    var clients = new List<(int Id, string Nom)>();
                    using (var cmd = new MySqlCommand("SELECT Id, Nom, Prenom FROM Clients ORDER BY Nom", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                clients.Add((
                                    reader.GetInt32("Id"),
                                    $"{reader.GetString("Prenom")} {reader.GetString("Nom")}"
                                ));
                            }
                        }
                    }

                    if (clients.Count == 0)
                    {
                        Console.WriteLine("Aucun client n'est disponible.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }

                    Console.WriteLine("Sélection du client:");
                    for (int i = 0; i < clients.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {clients[i].Nom}");
                    }

                    Console.Write("Votre choix: ");
                    if (!int.TryParse(Console.ReadLine(), out int indexClient) ||
                        indexClient < 1 || indexClient > clients.Count)
                    {
                        Console.WriteLine("Choix de client invalide.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }

                    var clientSelectionne = clients[indexClient - 1];

                    // 2. Récupérer les commandes du client
                    var dataTable = dbService.ObtenirCommandesClient(clientSelectionne.Id);

                    Console.WriteLine($"\nCommandes du client {clientSelectionne.Nom}:");
                    Console.WriteLine("---------------------------------------------------");

                    if (dataTable.Rows.Count == 0)
                    {
                        Console.WriteLine("Aucune commande trouvée pour ce client.");
                    }
                    else
                    {
                        decimal totalClient = 0;

                        Console.WriteLine($"{"ID",-5} {"Date commande",-15} {"Livraison prévue",-20} {"Prix",-10} {"Statut",-15}");
                        Console.WriteLine(new string('-', 70));

                        foreach (DataRow row in dataTable.Rows)
                        {
                            int commandeID = Convert.ToInt32(row["CommandeID"]);
                            DateTime dateCommande = Convert.ToDateTime(row["DateCommande"]);
                            DateTime dateLivraison = Convert.ToDateTime(row["DateLivraisonPrevue"]);
                            decimal prixTotal = Convert.ToDecimal(row["PrixTotal"]);
                            StatutCommande statut = (StatutCommande)Convert.ToInt32(row["Statut"]);

                            Console.WriteLine($"{commandeID,-5} {dateCommande:dd/MM/yyyy} {dateLivraison:dd/MM/yyyy}         {prixTotal:C}  {statut}");

                            totalClient += prixTotal;
                        }

                        Console.WriteLine(new string('-', 70));
                        Console.WriteLine($"Total: {totalClient:C} pour {dataTable.Rows.Count} commande(s)");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'affichage des statistiques: {ex.Message}");
            }

            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void GererImportExport(DatabaseService dbService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("┌───────────────────────────────────────┐");
                Console.WriteLine("│       IMPORT/EXPORT DE DONNÉES        │");
                Console.WriteLine("└───────────────────────────────────────┘");
                Console.WriteLine();
                Console.WriteLine("1. Exporter les plats en JSON");
                Console.WriteLine("2. Importer les plats depuis JSON");
                Console.WriteLine("3. Exporter les plats en XML");
                Console.WriteLine("4. Importer les plats depuis XML");
                Console.WriteLine("5. Exporter les commandes en JSON");
                Console.WriteLine("6. Importer les commandes depuis JSON");
                Console.WriteLine("0. Retour au menu principal");
                Console.WriteLine();
                Console.Write("Votre choix: ");

                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        ExporterPlatsJSON(dbService);
                        break;
                    case "2":
                        ImporterPlatsJSON(dbService);
                        break;
                    case "3":
                        ExporterPlatsXML(dbService);
                        break;
                    case "4":
                        ImporterPlatsXML(dbService);
                        break;
                    case "5":
                        ExporterCommandesJSON(dbService);
                        break;
                    case "6":
                        ImporterCommandesJSON(dbService);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Option non valide. Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ExporterPlatsJSON(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│       EXPORTER LES PLATS EN JSON      │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                // 1. Récupérer les plats
                var plats = dbService.RechercherPlats();

                if (plats.Count == 0)
                {
                    Console.WriteLine("Aucun plat à exporter.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }

                // 2. Demander le chemin du fichier
                Console.Write("Chemin du fichier JSON (ex: plats.json): ");
                string cheminFichier = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(cheminFichier))
                {
                    cheminFichier = "plats.json";
                }

                // 3. Exporter les plats
                SerializationService.ExporterPlatsJSON(plats, cheminFichier);
                Console.WriteLine($"{plats.Count} plat(s) exporté(s) avec succès vers {cheminFichier}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'exportation: {ex.Message}");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void ImporterPlatsJSON(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│     IMPORTER LES PLATS DEPUIS JSON    │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                // 1. Demander le chemin du fichier
                Console.Write("Chemin du fichier JSON à importer: ");
                string cheminFichier = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(cheminFichier))
                {
                    Console.WriteLine("Chemin de fichier invalide.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }

                // 2. Importer les plats
                var plats = SerializationService.ImporterPlatsJSON(cheminFichier);

                if (plats.Count == 0)
                {
                    Console.WriteLine("Aucun plat trouvé dans le fichier JSON.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }

                // 3. Vérifier si les cuisiniers existent
                Console.WriteLine($"{plats.Count} plat(s) trouvé(s) dans le fichier.");
                Console.WriteLine("Préparation de l'import...");

                using (var connection = new MySqlConnection(
                    $"Server={SERVER};Database={DATABASE};Uid={ADMIN_USER};Pwd={ADMIN_PASSWORD};"))
                {
                    connection.Open();

                    // Récupérer les cuisiniers existants
                    var cuisiniers = new Dictionary<int, bool>();
                    using (var cmd = new MySqlCommand("SELECT Id FROM Cuisiniers", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cuisiniers[reader.GetInt32("Id")] = true;
                            }
                        }
                    }

                    // Vérifier si tous les cuisiniers existent
                    bool cuisiniersValides = true;
                    foreach (var plat in plats)
                    {
                        if (!cuisiniers.ContainsKey(plat.CuisinierID))
                        {
                            Console.WriteLine($"Erreur: Le cuisinier avec l'ID {plat.CuisinierID} n'existe pas pour le plat \"{plat.Nom}\".");
                            cuisiniersValides = false;
                        }
                    }

                    if (!cuisiniersValides)
                    {
                        Console.WriteLine("Import impossible: Certains cuisiniers référencés n'existent pas.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }
                }

                // 4. Importer les plats dans la base de données
                Console.Write($"Confirmer l'import de {plats.Count} plat(s)? (O/N): ");
                if (Console.ReadLine().Trim().ToUpper().StartsWith("O"))
                {
                    int compteur = 0;
                    foreach (var plat in plats)
                    {
                        // On réinitialise l'ID pour éviter les conflits
                        plat.Id = 0;
                        dbService.InsererPlat(plat);
                        compteur++;
                    }

                    Console.WriteLine($"{compteur} plat(s) importé(s) avec succès.");
                }
                else
                {
                    Console.WriteLine("Import annulé.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'importation: {ex.Message}");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void ExporterPlatsXML(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│        EXPORTER LES PLATS EN XML      │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                // 1. Récupérer les plats
                var plats = dbService.RechercherPlats();

                if (plats.Count == 0)
                {
                    Console.WriteLine("Aucun plat à exporter.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }

                // 2. Demander le chemin du fichier
                Console.Write("Chemin du fichier XML (ex: plats.xml): ");
                string cheminFichier = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(cheminFichier))
                {
                    cheminFichier = "plats.xml";
                }

                // 3. Exporter les plats
                SerializationService.ExporterPlatsXML(plats, cheminFichier);
                Console.WriteLine($"{plats.Count} plat(s) exporté(s) avec succès vers {cheminFichier}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'exportation: {ex.Message}");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void ImporterPlatsXML(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│     IMPORTER LES PLATS DEPUIS XML     │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                // 1. Demander le chemin du fichier
                Console.Write("Chemin du fichier XML à importer: ");
                string cheminFichier = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(cheminFichier))
                {
                    Console.WriteLine("Chemin de fichier invalide.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }

                // 2. Importer les plats
                var plats = SerializationService.ImporterPlatsXML(cheminFichier);

                if (plats.Count == 0)
                {
                    Console.WriteLine("Aucun plat trouvé dans le fichier XML.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }

                // 3. Vérifier si les cuisiniers existent
                Console.WriteLine($"{plats.Count} plat(s) trouvé(s) dans le fichier.");
                Console.WriteLine("Préparation de l'import...");

                using (var connection = new MySqlConnection(
                    $"Server={SERVER};Database={DATABASE};Uid={ADMIN_USER};Pwd={ADMIN_PASSWORD};"))
                {
                    connection.Open();

                    // Récupérer les cuisiniers existants
                    var cuisiniers = new Dictionary<int, bool>();
                    using (var cmd = new MySqlCommand("SELECT Id FROM Cuisiniers", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cuisiniers[reader.GetInt32("Id")] = true;
                            }
                        }
                    }

                    // Vérifier si tous les cuisiniers existent
                    bool cuisiniersValides = true;
                    foreach (var plat in plats)
                    {
                        if (!cuisiniers.ContainsKey(plat.CuisinierID))
                        {
                            Console.WriteLine($"Erreur: Le cuisinier avec l'ID {plat.CuisinierID} n'existe pas pour le plat \"{plat.Nom}\".");
                            cuisiniersValides = false;
                        }
                    }

                    if (!cuisiniersValides)
                    {
                        Console.WriteLine("Import impossible: Certains cuisiniers référencés n'existent pas.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }
                }

                // 4. Importer les plats dans la base de données
                Console.Write($"Confirmer l'import de {plats.Count} plat(s)? (O/N): ");
                if (Console.ReadLine().Trim().ToUpper().StartsWith("O"))
                {
                    int compteur = 0;
                    foreach (var plat in plats)
                    {
                        // On réinitialise l'ID pour éviter les conflits
                        plat.Id = 0;
                        dbService.InsererPlat(plat);
                        compteur++;
                    }

                    Console.WriteLine($"{compteur} plat(s) importé(s) avec succès.");
                }
                else
                {
                    Console.WriteLine("Import annulé.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'importation: {ex.Message}");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void ExporterCommandesJSON(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│     EXPORTER LES COMMANDES EN JSON    │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                // 1. Récupérer toutes les commandes
                var commandes = new List<Commande>();

                using (var connection = new MySqlConnection(
                    $"Server={SERVER};Database={DATABASE};Uid={ADMIN_USER};Pwd={ADMIN_PASSWORD};"))
                {
                    connection.Open();

                    // Récupérer les commandes
                    string queryCommandes = @"
                        SELECT * FROM Commandes 
                        ORDER BY DateCommande DESC";

                    using (var cmd = new MySqlCommand(queryCommandes, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var commande = new Commande
                                {
                                    Id = reader.GetInt32("Id"),
                                    ClientID = reader.GetInt32("ClientID"),
                                    DateCommande = reader.GetDateTime("DateCommande"),
                                    DateLivraisonPrevue = reader.GetDateTime("DateLivraisonPrevue"),
                                    PrixTotal = reader.GetDecimal("PrixTotal"),
                                    Statut = (StatutCommande)reader.GetInt32("Statut"),
                                    AdresseLivraison = reader.GetString("AdresseLivraison"),
                                    Commentaires = reader.IsDBNull(reader.GetOrdinal("Commentaires")) ? null : reader.GetString("Commentaires"),
                                    Items = new List<CommandeItem>()
                                };

                                commandes.Add(commande);
                            }
                        }
                    }

                    // Pour chaque commande, récupérer ses items
                    foreach (var commande in commandes)
                    {
                        string queryItems = @"
                            SELECT * FROM CommandeItems 
                            WHERE CommandeID = @CommandeID";

                        using (var cmd = new MySqlCommand(queryItems, connection))
                        {
                            cmd.Parameters.AddWithValue("@CommandeID", commande.Id);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var item = new CommandeItem
                                    {
                                        Id = reader.GetInt32("Id"),
                                        CommandeID = reader.GetInt32("CommandeID"),
                                        PlatID = reader.GetInt32("PlatID"),
                                        Quantite = reader.GetInt32("Quantite"),
                                        PrixUnitaire = reader.GetDecimal("PrixUnitaire")
                                    };

                                    commande.Items.Add(item);
                                }
                            }
                        }
                    }
                }

                if (commandes.Count == 0)
                {
                    Console.WriteLine("Aucune commande à exporter.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }

                // 2. Demander le chemin du fichier
                Console.Write("Chemin du fichier JSON (ex: commandes.json): ");
                string cheminFichier = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(cheminFichier))
                {
                    cheminFichier = "commandes.json";
                }

                // 3. Exporter les commandes
                SerializationService.ExporterCommandesJSON(commandes, cheminFichier);
                Console.WriteLine($"{commandes.Count} commande(s) exportée(s) avec succès vers {cheminFichier}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'exportation: {ex.Message}");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void ImporterCommandesJSON(DatabaseService dbService)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────┐");
            Console.WriteLine("│   IMPORTER LES COMMANDES DEPUIS JSON  │");
            Console.WriteLine("└───────────────────────────────────────┘");

            try
            {
                // 1. Demander le chemin du fichier
                Console.Write("Chemin du fichier JSON à importer: ");
                string cheminFichier = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(cheminFichier))
                {
                    Console.WriteLine("Chemin de fichier invalide.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }

                // 2. Importer les commandes
                var commandes = SerializationService.ImporterCommandesJSON(cheminFichier);

                if (commandes.Count == 0)
                {
                    Console.WriteLine("Aucune commande trouvée dans le fichier JSON.");
                    Console.WriteLine("Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }

                // 3. Vérifier si les clients et plats existent
                Console.WriteLine($"{commandes.Count} commande(s) trouvée(s) dans le fichier.");
                Console.WriteLine("Préparation de l'import...");

                using (var connection = new MySqlConnection(
                    $"Server={SERVER};Database={DATABASE};Uid={ADMIN_USER};Pwd={ADMIN_PASSWORD};"))
                {
                    connection.Open();

                    // Récupérer les clients existants
                    var clients = new Dictionary<int, bool>();
                    using (var cmd = new MySqlCommand("SELECT Id FROM Clients", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                clients[reader.GetInt32("Id")] = true;
                            }
                        }
                    }

                    // Récupérer les plats existants
                    var plats = new Dictionary<int, bool>();
                    using (var cmd = new MySqlCommand("SELECT Id FROM Plats", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                plats[reader.GetInt32("Id")] = true;
                            }
                        }
                    }

                    // Vérifier si tous les clients et plats existent
                    bool donneesValides = true;
                    foreach (var commande in commandes)
                    {
                        if (!clients.ContainsKey(commande.ClientID))
                        {
                            Console.WriteLine($"Erreur: Le client avec l'ID {commande.ClientID} n'existe pas pour la commande #{commande.Id}.");
                            donneesValides = false;
                        }

                        foreach (var item in commande.Items)
                        {
                            if (!plats.ContainsKey(item.PlatID))
                            {
                                Console.WriteLine($"Erreur: Le plat avec l'ID {item.PlatID} n'existe pas pour la commande #{commande.Id}.");
                                donneesValides = false;
                            }
                        }
                    }

                    if (!donneesValides)
                    {
                        Console.WriteLine("Import impossible: Certains clients ou plats référencés n'existent pas.");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return;
                    }
                }

                // 4. Importer les commandes dans la base de données
                Console.Write($"Confirmer l'import de {commandes.Count} commande(s)? (O/N): ");
                if (Console.ReadLine().Trim().ToUpper().StartsWith("O"))
                {
                    int compteur = 0;
                    foreach (var commande in commandes)
                    {
                        // Réinitialiser les IDs pour éviter les conflits
                        commande.Id = 0;
                        foreach (var item in commande.Items)
                        {
                            item.Id = 0;
                        }

                        dbService.CreerCommande(commande);
                        compteur++;
                    }

                    Console.WriteLine($"{compteur} commande(s) importée(s) avec succès.");
                }
                else
                {
                    Console.WriteLine("Import annulé.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'importation: {ex.Message}");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
    }
}