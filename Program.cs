using System;
using LivInParis.database;
using LivInParis.UI;

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

                // Initialize the database
                Console.WriteLine("Initialisation de la base de données...");
                var dbManager = new DatabaseManager(SERVER, ADMIN_USER, ADMIN_PASSWORD);

                // Create the database and tables
                dbManager.InitializeDatabase(DATABASE);

                // Create different users with different permissions
                CreateDatabaseUsers(dbManager);

                // Populate sample data if tables are empty
                CheckAndPopulateData(dbManager);

                // Start the console UI
                var ui = new ConsoleUI(SERVER, DATABASE, ADMIN_USER, ADMIN_PASSWORD);
                ui.DisplayMainMenu();
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

        static void CreateDatabaseUsers(DatabaseManager dbManager)
        {
            try
            {
                // Create a read-only user
                dbManager.CreateDatabaseUser(
                    "livinparis_readonly",
                    "readonly123",
                    DATABASE,
                    "SELECT"
                );

                // Create a user for client operations
                dbManager.CreateDatabaseUser(
                    "livinparis_client",
                    "client123",
                    DATABASE,
                    "SELECT, INSERT, UPDATE, DELETE ON Clients"
                );

                // Create a user for cook operations
                dbManager.CreateDatabaseUser(
                    "livinparis_cook",
                    "cook123",
                    DATABASE,
                    "SELECT, INSERT, UPDATE, DELETE ON Cooks"
                );

                // Create a user for order operations
                dbManager.CreateDatabaseUser(
                    "livinparis_order",
                    "order123",
                    DATABASE,
                    "SELECT, INSERT, UPDATE, DELETE ON Orders, OrderItems"
                );

                // Create a full access user (except for administrative operations)
                dbManager.CreateDatabaseUser(
                    "livinparis_manager",
                    "manager123",
                    DATABASE,
                    "SELECT, INSERT, UPDATE, DELETE"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Avertissement: Création des utilisateurs échouée: {ex.Message}");
                Console.WriteLine("Le programme continuera avec l'utilisateur admin.");
            }
        }

        static void CheckAndPopulateData(DatabaseManager dbManager)
        {
            try
            {
                // Check if tables are empty
                bool shouldPopulate = false;

                using (var connection = new MySql.Data.MySqlClient.MySqlConnection(
                    $"Server={SERVER};Database={DATABASE};Uid={ADMIN_USER};Pwd={ADMIN_PASSWORD};"))
                {
                    connection.Open();

                    string checkQuery = "SELECT COUNT(*) FROM Clients";
                    using (var command = new MySql.Data.MySqlClient.MySqlCommand(checkQuery, connection))
                    {
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        shouldPopulate = count == 0;
                    }
                }

                if (shouldPopulate)
                {
                    Console.WriteLine("Les tables sont vides. Ajout de données d'exemple...");
                    dbManager.PopulateSampleData(DATABASE);
                }
                else
                {
                    Console.WriteLine("Base de données déjà peuplée.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Avertissement: Vérification des données échouée: {ex.Message}");
            }
        }
    }
}