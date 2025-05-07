using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Xml;
using System.IO;
using LivInParis.Models;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Diagnostics;
using SkiaSharp;
using LivInParis.Services;  /// Ajout de la référence pour SerializationService

namespace LivInParis.UI
{
    public class ConsoleUI
    {
        private readonly string _connectionString;
        private readonly ConsoleColor _headerColor = ConsoleColor.Cyan;
        private readonly ConsoleColor _errorColor = ConsoleColor.Red;
        private readonly ConsoleColor _successColor = ConsoleColor.Green;
        private readonly ConsoleColor _menuColor = ConsoleColor.Yellow;

        public ConsoleUI(string server, string database, string uid, string password)
        {
            _connectionString = $"Server={server};Database={database};Uid={uid};Pwd={password};";
        }

        public void DisplayMainMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                DisplayHeader("LIV'IN PARIS - SYSTÈME DE GESTION");

                Console.ForegroundColor = _menuColor;
                Console.WriteLine("┌─────────────────────────────────────┐");
                Console.WriteLine("│           MENU PRINCIPAL            │");
                Console.WriteLine("├─────────────────────────────────────┤");
                Console.WriteLine("│ 1. Gestion des clients              │");
                Console.WriteLine("│ 2. Gestion des cuisiniers           │");
                Console.WriteLine("│ 3. Gestion des commandes            │");
                Console.WriteLine("│ 4. Statistiques                     │");
                Console.WriteLine("│ 5. Gestion du réseau métro          │");
                Console.WriteLine("│ 6. Import/Export de données         │");
                Console.WriteLine("│ 0. Quitter                          │");
                Console.WriteLine("└─────────────────────────────────────┘");
                Console.ResetColor();

                Console.Write("\nVotre choix: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DisplayClientMenu();
                        break;
                    case "2":
                        DisplayCookMenu();
                        break;
                    case "3":
                        DisplayOrderMenu();
                        break;
                    case "4":
                        DisplayStatisticsMenu();
                        break;
                    case "5":
                        DisplayMetroMenu();
                        break;
                    case "6":
                        DisplayImportExportMenu();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        DisplayError("Option invalide. Veuillez réessayer.");
                        WaitForKey();
                        break;
                }
            }
        }

        private void DisplayClientMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                DisplayHeader("GESTION DES CLIENTS");

                Console.ForegroundColor = _menuColor;
                Console.WriteLine("┌─────────────────────────────────────┐");
                Console.WriteLine("│         MENU DES CLIENTS            │");
                Console.WriteLine("├─────────────────────────────────────┤");
                Console.WriteLine("│ 1. Ajouter un client                │");
                Console.WriteLine("│ 2. Modifier un client               │");
                Console.WriteLine("│ 3. Supprimer un client              │");
                Console.WriteLine("│ 4. Liste des clients (alphabétique) │");
                Console.WriteLine("│ 5. Liste des clients (par rue)      │");
                Console.WriteLine("│ 6. Liste des clients (montant achat)│");
                Console.WriteLine("│ 0. Retour au menu principal         │");
                Console.WriteLine("└─────────────────────────────────────┘");
                Console.ResetColor();

                Console.Write("\nVotre choix: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddClient();
                        break;
                    case "2":
                        ModifyClient();
                        break;
                    case "3":
                        DeleteClient();
                        break;
                    case "4":
                        ListClientsByName();
                        break;
                    case "5":
                        ListClientsByStreet();
                        break;
                    case "6":
                        ListClientsByPurchase();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        DisplayError("Option invalide. Veuillez réessayer.");
                        WaitForKey();
                        break;
                }
            }
        }

        private void DisplayCookMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                DisplayHeader("GESTION DES CUISINIERS");

                Console.ForegroundColor = _menuColor;
                Console.WriteLine("┌─────────────────────────────────────┐");
                Console.WriteLine("│        MENU DES CUISINIERS          │");
                Console.WriteLine("├─────────────────────────────────────┤");
                Console.WriteLine("│ 1. Ajouter un cuisinier             │");
                Console.WriteLine("│ 2. Modifier un cuisinier            │");
                Console.WriteLine("│ 3. Supprimer un cuisinier           │");
                Console.WriteLine("│ 4. Historique clients               │");
                Console.WriteLine("│ 5. Plats par fréquence              │");
                Console.WriteLine("│ 6. Plat du jour                     │");
                Console.WriteLine("│ 0. Retour au menu principal         │");
                Console.WriteLine("└─────────────────────────────────────┘");
                Console.ResetColor();

                Console.Write("\nVotre choix: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddCook();
                        break;
                    case "2":
                        ModifyCook();
                        break;
                    case "3":
                        DeleteCook();
                        break;
                    case "4":
                        DisplayClientHistory();
                        break;
                    case "5":
                        DisplayDishesByFrequency();
                        break;
                    case "6":
                        DisplayDishOfTheDay();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        DisplayError("Option invalide. Veuillez réessayer.");
                        WaitForKey();
                        break;
                }
            }
        }

        private void DisplayOrderMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                DisplayHeader("GESTION DES COMMANDES");

                Console.ForegroundColor = _menuColor;
                Console.WriteLine("┌─────────────────────────────────────┐");
                Console.WriteLine("│        MENU DES COMMANDES           │");
                Console.WriteLine("├─────────────────────────────────────┤");
                Console.WriteLine("│ 1. Créer une commande               │");
                Console.WriteLine("│ 2. Modifier une commande            │");
                Console.WriteLine("│ 3. Calculer le prix                 │");
                Console.WriteLine("│ 4. Déterminer chemin de livraison   │");
                Console.WriteLine("│ 0. Retour au menu principal         │");
                Console.WriteLine("└─────────────────────────────────────┘");
                Console.ResetColor();

                Console.Write("\nVotre choix: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateOrder();
                        break;
                    case "2":
                        ModifyOrder();
                        break;
                    case "3":
                        CalculatePrice();
                        break;
                    case "4":
                        DetermineDeliveryPath();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        DisplayError("Option invalide. Veuillez réessayer.");
                        WaitForKey();
                        break;
                }
            }
        }

        private void DisplayStatisticsMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                DisplayHeader("STATISTIQUES");

                Console.ForegroundColor = _menuColor;
                Console.WriteLine("┌─────────────────────────────────────┐");
                Console.WriteLine("│           STATISTIQUES              │");
                Console.WriteLine("├─────────────────────────────────────┤");
                Console.WriteLine("│ 1. Livraisons par cuisinier         │");
                Console.WriteLine("│ 2. Commandes par période            │");
                Console.WriteLine("│ 3. Prix moyen des commandes         │");
                Console.WriteLine("│ 4. Comptes client moyens            │");
                Console.WriteLine("│ 5. Liste des commandes par client   │");
                Console.WriteLine("│ 0. Retour au menu principal         │");
                Console.WriteLine("└─────────────────────────────────────┘");
                Console.ResetColor();

                Console.Write("\nVotre choix: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DeliveriesPerCook();
                        break;
                    case "2":
                        OrdersByPeriod();
                        break;
                    case "3":
                        AverageOrderPrice();
                        break;
                    case "4":
                        AverageClientAccounts();
                        break;
                    case "5":
                        OrderListByClient();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        DisplayError("Option invalide. Veuillez réessayer.");
                        WaitForKey();
                        break;
                }
            }
        }

        private void DisplayMetroMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                DisplayHeader("GESTION DU RÉSEAU MÉTRO");

                Console.ForegroundColor = _menuColor;
                Console.WriteLine("┌─────────────────────────────────────┐");
                Console.WriteLine("│          MENU DU RÉSEAU MÉTRO       │");
                Console.WriteLine("├─────────────────────────────────────┤");
                Console.WriteLine("│ 1. Charger le réseau métro          │");
                Console.WriteLine("│ 2. Afficher les statistiques        │");
                Console.WriteLine("│ 3. Calculer un itinéraire           │");
                Console.WriteLine("│ 4. Générer une visualisation        │");
                Console.WriteLine("│ 5. Optimiser les chemins de livraison │");
                Console.WriteLine("│ 6. Optimiser le planning cuisiniers │");
                Console.WriteLine("│ 0. Retour au menu principal         │");
                Console.WriteLine("└─────────────────────────────────────┘");
                Console.ResetColor();

                Console.Write("\nVotre choix: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        LoadMetroNetwork();
                        break;
                    case "2":
                        DisplayMetroStats();
                        break;
                    case "3":
                        CalculateRoute();
                        break;
                    case "4":
                        GenerateVisualization();
                        break;
                    case "5":
                        DetermineDeliveryPath();
                        break;
                    case "6":
                        OptimiserPlanningCuisiniers();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        DisplayError("Option invalide. Veuillez réessayer.");
                        WaitForKey();
                        break;
                }
            }
        }

        private void DisplayImportExportMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                DisplayHeader("IMPORT/EXPORT DE DONNÉES");

                Console.ForegroundColor = _menuColor;
                Console.WriteLine("┌─────────────────────────────────────┐");
                Console.WriteLine("│       IMPORT/EXPORT DE DONNÉES      │");
                Console.WriteLine("├─────────────────────────────────────┤");
                Console.WriteLine("│ 1. Exporter les plats en JSON       │");
                Console.WriteLine("│ 2. Importer les plats depuis JSON   │");
                Console.WriteLine("│ 3. Exporter les plats en XML        │");
                Console.WriteLine("│ 4. Importer les plats depuis XML    │");
                Console.WriteLine("│ 5. Exporter les commandes en JSON   │");
                Console.WriteLine("│ 6. Importer les commandes depuis JSON│");
                Console.WriteLine("│ 0. Retour au menu principal         │");
                Console.WriteLine("└─────────────────────────────────────┘");
                Console.ResetColor();

                Console.Write("\nVotre choix: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ExportPlatsToJSON();
                        break;
                    case "2":
                        ImportPlatsFromJSON();
                        break;
                    case "3":
                        ExportPlatsToXML();
                        break;
                    case "4":
                        ImportPlatsFromXML();
                        break;
                    case "5":
                        ExportCommandesToJSON();
                        break;
                    case "6":
                        ImportCommandesFromJSON();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        DisplayError("Option invalide. Veuillez réessayer.");
                        WaitForKey();
                        break;
                }
            }
        }

        // Client operations
        private void AddClient()
        {
            Console.Clear();
            DisplayHeader("AJOUTER UN CLIENT");

            try
            {
                Console.Write("Nom: ");
                string lastName = Console.ReadLine() ?? "";

                Console.Write("Prénom: ");
                string firstName = Console.ReadLine() ?? "";

                Console.Write("Adresse: ");
                string address = Console.ReadLine() ?? "";

                Console.Write("Téléphone: ");
                string phone = Console.ReadLine() ?? "";

                Console.Write("Email: ");
                string email = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(email))
                {
                    DisplayError("L'email est obligatoire.");
                    WaitForKey();
                    return;
                }

                if (!email.Contains("@") || !email.Contains("."))
                {
                    DisplayError("Format d'email invalide.");
                    WaitForKey();
                    return;
                }

                Console.Write("Type (1 = individuel, 2 = entreprise locale): ");
                string typeInput = Console.ReadLine();
                int clientType = typeInput == "2" ? 1 : 0;  // 0 for Individuel, 1 for Entreprise

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string insertQuery = @"
                        INSERT INTO Clients (Nom, Prenom, Adresse, Telephone, Email, Type, DateInscription) 
                        VALUES (@nom, @prenom, @adresse, @telephone, @email, @typeClient, NOW())";

                    using (var command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@nom", lastName);
                        command.Parameters.AddWithValue("@prenom", firstName);
                        command.Parameters.AddWithValue("@adresse", address);
                        command.Parameters.AddWithValue("@telephone", phone);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@typeClient", clientType);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            DisplaySuccess("Client ajouté avec succès!");
                        }
                        else
                        {
                            DisplayError("Erreur lors de l'ajout du client.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void ModifyClient()
        {
            Console.Clear();
            DisplayHeader("MODIFIER UN CLIENT");

            Console.Write("ID du client à modifier: ");
            if (!int.TryParse(Console.ReadLine(), out int clientId))
            {
                DisplayError("ID invalide.");
                WaitForKey();
                return;
            }

            try
            {
                // First check if client exists
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string checkQuery = "SELECT * FROM Clients WHERE Id = @clientId";
                    using (var command = new MySqlCommand(checkQuery, connection))
                    {
                        command.Parameters.AddWithValue("@clientId", clientId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                DisplayError($"Aucun client trouvé avec l'ID {clientId}.");
                                WaitForKey();
                                return;
                            }

                            Console.WriteLine($"\nModification du client: {reader["Prenom"]} {reader["Nom"]}");
                        }
                    }

                    Console.Write("Nouveau nom (vide pour conserver la valeur actuelle): ");
                    string lastName = Console.ReadLine() ?? "";

                    Console.Write("Nouveau prénom: ");
                    string firstName = Console.ReadLine() ?? "";

                    Console.Write("Nouvelle adresse: ");
                    string address = Console.ReadLine() ?? "";

                    Console.Write("Nouveau téléphone: ");
                    string phone = Console.ReadLine() ?? "";

                    Console.Write("Nouvel email: ");
                    string email = Console.ReadLine() ?? "";

                    Console.Write("Nouveau type (1 = individuel, 2 = entreprise locale, vide pour ne pas changer): ");
                    string typeInput = Console.ReadLine();
                    string clientType = null;
                    if (!string.IsNullOrEmpty(typeInput))
                    {
                        clientType = typeInput == "2" ? "Entreprise" : "Individuel";
                    }

                    // Update only non-empty fields
                    string updateQuery = "UPDATE Clients SET ";
                    List<string> updateFields = new List<string>();

                    using (var command = new MySqlCommand())
                    {
                        command.Connection = connection;

                        if (!string.IsNullOrEmpty(lastName))
                        {
                            updateFields.Add("Nom = @nom");
                            command.Parameters.AddWithValue("@nom", lastName);
                        }

                        if (!string.IsNullOrEmpty(firstName))
                        {
                            updateFields.Add("Prenom = @prenom");
                            command.Parameters.AddWithValue("@prenom", firstName);
                        }

                        if (!string.IsNullOrEmpty(address))
                        {
                            updateFields.Add("Adresse = @adresse");
                            command.Parameters.AddWithValue("@adresse", address);
                        }

                        if (!string.IsNullOrEmpty(phone))
                        {
                            updateFields.Add("Telephone = @telephone");
                            command.Parameters.AddWithValue("@telephone", phone);
                        }

                        if (!string.IsNullOrEmpty(email))
                        {
                            updateFields.Add("Email = @email");
                            command.Parameters.AddWithValue("@email", email);
                        }

                        if (!string.IsNullOrEmpty(clientType))
                        {
                            updateFields.Add("Type = @typeClient");
                            command.Parameters.AddWithValue("@typeClient", clientType);
                        }

                        if (updateFields.Count == 0)
                        {
                            DisplayError("Aucune modification effectuée.");
                            WaitForKey();
                            return;
                        }

                        updateQuery += string.Join(", ", updateFields);
                        updateQuery += " WHERE Id = @clientId";
                        command.Parameters.AddWithValue("@clientId", clientId);

                        command.CommandText = updateQuery;
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            DisplaySuccess("Client modifié avec succès!");
                        }
                        else
                        {
                            DisplayError("Erreur lors de la modification du client.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void DeleteClient()
        {
            Console.Clear();
            DisplayHeader("SUPPRIMER UN CLIENT");

            Console.Write("ID du client à supprimer: ");
            if (!int.TryParse(Console.ReadLine(), out int clientId))
            {
                DisplayError("ID invalide.");
                WaitForKey();
                return;
            }

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // First check if client exists and display info
                    string checkQuery = "SELECT Nom, Prenom FROM Clients WHERE Id = @clientId";
                    string clientName = "";

                    using (var command = new MySqlCommand(checkQuery, connection))
                    {
                        command.Parameters.AddWithValue("@clientId", clientId);
                        var result = command.ExecuteScalar();

                        if (result == null)
                        {
                            DisplayError($"Aucun client trouvé avec l'ID {clientId}.");
                            WaitForKey();
                            return;
                        }

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                clientName = $"{reader["Prenom"]} {reader["Nom"]}";
                            }
                        }
                    }

                    Console.Write($"\nÊtes-vous sûr de vouloir supprimer le client {clientName}? (O/N): ");
                    string confirm = Console.ReadLine().ToUpper();

                    if (confirm != "O" && confirm != "OUI")
                    {
                        Console.WriteLine("Suppression annulée.");
                        WaitForKey();
                        return;
                    }

                    // Check for related orders
                    string checkOrdersQuery = "SELECT COUNT(*) FROM Orders WHERE ClientId = @clientId";
                    int orderCount = 0;

                    using (var command = new MySqlCommand(checkOrdersQuery, connection))
                    {
                        command.Parameters.AddWithValue("@clientId", clientId);
                        orderCount = Convert.ToInt32(command.ExecuteScalar());
                    }

                    if (orderCount > 0)
                    {
                        DisplayError($"Impossible de supprimer le client car il a {orderCount} commandes associées.");
                        WaitForKey();
                        return;
                    }

                    // Delete client
                    string deleteQuery = "DELETE FROM Clients WHERE Id = @clientId";
                    using (var command = new MySqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@clientId", clientId);
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            DisplaySuccess("Client supprimé avec succès!");
                        }
                        else
                        {
                            DisplayError("Erreur lors de la suppression du client.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void ListClientsByName()
        {
            Console.Clear();
            DisplayHeader("LISTE DES CLIENTS (ORDRE ALPHABÉTIQUE)");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT Id, Nom, Prenom, TypeClient, Telephone FROM Clients ORDER BY Nom, Prenom";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                Console.WriteLine("Aucun client trouvé.");
                                WaitForKey();
                                return;
                            }

                            Console.WriteLine("\n{0,-5} {1,-15} {2,-15} {3,-15} {4,-15}", "ID", "Nom", "Prénom", "Type", "Téléphone");
                            Console.WriteLine(new string('-', 70));

                            while (reader.Read())
                            {
                                string clientType = reader["TypeClient"].ToString() == "individual" ? "Individuel" : "Entreprise";
                                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-15} {4,-15}",
                                    reader["Id"],
                                    reader["Nom"],
                                    reader["Prenom"],
                                    clientType,
                                    reader["Telephone"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void ListClientsByStreet()
        {
            Console.Clear();
            DisplayHeader("LISTE DES CLIENTS (PAR RUE)");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // Extract street name from address and order by it
                    string query = @"
                        SELECT 
                            Id, 
                            Nom, 
                            Prenom, 
                            Adresse,
                            SUBSTRING_INDEX(SUBSTRING_INDEX(Adresse, ',', 1), ' ', -1) AS Rue
                        FROM Clients 
                        ORDER BY Rue, Nom, Prenom";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                Console.WriteLine("Aucun client trouvé.");
                                WaitForKey();
                                return;
                            }

                            Console.WriteLine("\n{0,-5} {1,-15} {2,-15} {3,-30}", "ID", "Nom", "Prénom", "Adresse");
                            Console.WriteLine(new string('-', 70));

                            while (reader.Read())
                            {
                                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-30}",
                                    reader["Id"],
                                    reader["Nom"],
                                    reader["Prenom"],
                                    reader["Adresse"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void ListClientsByPurchase()
        {
            Console.Clear();
            DisplayHeader("LISTE DES CLIENTS (PAR MONTANT D'ACHAT)");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            c.Id, 
                            c.Nom, 
                            c.Prenom, 
                            c.TypeClient,
                            COALESCE(SUM(o.PrixTotal), 0) AS TotalAchat
                        FROM 
                            Clients c
                        LEFT JOIN 
                            Commandes o ON c.Id = o.ClientId
                        GROUP BY 
                            c.Id, c.Nom, c.Prenom, c.TypeClient
                        ORDER BY 
                            TotalAchat DESC";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                Console.WriteLine("Aucun client trouvé.");
                                WaitForKey();
                                return;
                            }

                            Console.WriteLine("\n{0,-5} {1,-15} {2,-15} {3,-15} {4,-15}",
                                "ID", "Nom", "Prénom", "Type", "Total Achats (€)");
                            Console.WriteLine(new string('-', 70));

                            while (reader.Read())
                            {
                                string clientType = reader["TypeClient"].ToString() == "individual" ? "Individuel" : "Entreprise";
                                decimal totalPurchase = Convert.ToDecimal(reader["TotalAchat"]);

                                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-15} {4,-15:F2}",
                                    reader["Id"],
                                    reader["Nom"],
                                    reader["Prenom"],
                                    clientType,
                                    totalPurchase);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        // Cook operations and other methods would be implemented similarly
        private void AddCook()
        {
            Console.Clear();
            DisplayHeader("AJOUTER UN CUISINIER");

            try
            {
                Console.Write("Nom: ");
                string lastName = Console.ReadLine() ?? "";

                Console.Write("Prénom: ");
                string firstName = Console.ReadLine() ?? "";

                Console.Write("Adresse: ");
                string address = Console.ReadLine() ?? "";

                Console.Write("Téléphone: ");
                string phone = Console.ReadLine() ?? "";

                Console.Write("Email: ");
                string email = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(email))
                {
                    DisplayError("L'email est obligatoire.");
                    WaitForKey();
                    return;
                }

                if (!email.Contains("@") || !email.Contains("."))
                {
                    DisplayError("Format d'email invalide.");
                    WaitForKey();
                    return;
                }

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string insertQuery = @"
                        INSERT INTO Cooks (LastName, FirstName, Address, Phone, Email, RegistrationDate) 
                        VALUES (@nom, @prenom, @adresse, @telephone, @email, NOW())";

                    using (var command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@nom", lastName);
                        command.Parameters.AddWithValue("@prenom", firstName);
                        command.Parameters.AddWithValue("@adresse", address);
                        command.Parameters.AddWithValue("@telephone", phone);
                        command.Parameters.AddWithValue("@email", email);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            DisplaySuccess("Cuisinier ajouté avec succès!");
                        }
                        else
                        {
                            DisplayError("Erreur lors de l'ajout du cuisinier.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void ModifyCook()
        {
            Console.Clear();
            DisplayHeader("MODIFIER UN CUISINIER");

            Console.Write("ID du cuisinier à modifier: ");
            if (!int.TryParse(Console.ReadLine(), out int cookId))
            {
                DisplayError("ID invalide.");
                WaitForKey();
                return;
            }

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string checkQuery = "SELECT * FROM Cuisiniers WHERE Id = @cookId";
                    using (var command = new MySqlCommand(checkQuery, connection))
                    {
                        command.Parameters.AddWithValue("@cookId", cookId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                DisplayError($"Aucun cuisinier trouvé avec l'ID {cookId}.");
                                WaitForKey();
                                return;
                            }

                            Console.WriteLine($"\nModification du cuisinier: {reader["Prenom"]} {reader["Nom"]}");
                        }
                    }

                    Console.Write("Nouveau nom (vide pour conserver la valeur actuelle): ");
                    string lastName = Console.ReadLine() ?? "";

                    Console.Write("Nouveau prénom: ");
                    string firstName = Console.ReadLine() ?? "";

                    Console.Write("Nouvelle adresse: ");
                    string address = Console.ReadLine() ?? "";

                    Console.Write("Nouveau téléphone: ");
                    string phone = Console.ReadLine() ?? "";

                    Console.Write("Nouvel email: ");
                    string email = Console.ReadLine() ?? "";

                    if (!string.IsNullOrEmpty(email) && (!email.Contains("@") || !email.Contains(".")))
                    {
                        DisplayError("Format d'email invalide.");
                        WaitForKey();
                        return;
                    }

                    string updateQuery = "UPDATE Cuisiniers SET ";
                    List<string> updateFields = new List<string>();

                    using (var command = new MySqlCommand())
                    {
                        command.Connection = connection;

                        if (!string.IsNullOrEmpty(lastName))
                        {
                            updateFields.Add("Nom = @nom");
                            command.Parameters.AddWithValue("@nom", lastName);
                        }

                        if (!string.IsNullOrEmpty(firstName))
                        {
                            updateFields.Add("Prenom = @prenom");
                            command.Parameters.AddWithValue("@prenom", firstName);
                        }

                        if (!string.IsNullOrEmpty(address))
                        {
                            updateFields.Add("Adresse = @adresse");
                            command.Parameters.AddWithValue("@adresse", address);
                        }

                        if (!string.IsNullOrEmpty(phone))
                        {
                            updateFields.Add("Telephone = @telephone");
                            command.Parameters.AddWithValue("@telephone", phone);
                        }

                        if (!string.IsNullOrEmpty(email))
                        {
                            updateFields.Add("Email = @email");
                            command.Parameters.AddWithValue("@email", email);
                        }

                        if (updateFields.Count == 0)
                        {
                            DisplayError("Aucune modification effectuée.");
                            WaitForKey();
                            return;
                        }

                        updateQuery += string.Join(", ", updateFields);
                        updateQuery += " WHERE Id = @cookId";
                        command.Parameters.AddWithValue("@cookId", cookId);

                        command.CommandText = updateQuery;
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            DisplaySuccess("Cuisinier modifié avec succès!");
                        }
                        else
                        {
                            DisplayError("Erreur lors de la modification du cuisinier.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void DeleteCook()
        {
            Console.Clear();
            DisplayHeader("SUPPRIMER UN CUISINIER");

            Console.Write("ID du cuisinier à supprimer: ");
            if (!int.TryParse(Console.ReadLine(), out int cookId))
            {
                DisplayError("ID invalide.");
                WaitForKey();
                return;
            }

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // First check if cook exists and display info
                    string checkQuery = "SELECT Nom, Prenom FROM Cuisiniers WHERE Id = @cookId";
                    string cookName = "";

                    using (var command = new MySqlCommand(checkQuery, connection))
                    {
                        command.Parameters.AddWithValue("@cookId", cookId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                DisplayError($"Aucun cuisinier trouvé avec l'ID {cookId}.");
                                WaitForKey();
                                return;
                            }

                            cookName = $"{reader["Prenom"]} {reader["Nom"]}";
                        }
                    }

                    Console.Write($"\nÊtes-vous sûr de vouloir supprimer le cuisinier {cookName}? (O/N): ");
                    string confirm = Console.ReadLine().ToUpper();

                    if (confirm != "O" && confirm != "OUI")
                    {
                        Console.WriteLine("Suppression annulée.");
                        WaitForKey();
                        return;
                    }

                    // Check for related meals
                    string checkMealsQuery = "SELECT COUNT(*) FROM Plats WHERE CuisinierId = @cookId";
                    int mealCount = 0;

                    using (var command = new MySqlCommand(checkMealsQuery, connection))
                    {
                        command.Parameters.AddWithValue("@cookId", cookId);
                        mealCount = Convert.ToInt32(command.ExecuteScalar());
                    }

                    if (mealCount > 0)
                    {
                        DisplayError($"Impossible de supprimer le cuisinier car il a {mealCount} plats associés.");
                        WaitForKey();
                        return;
                    }

                    // Delete cook
                    string deleteQuery = "DELETE FROM Cuisiniers WHERE Id = @cookId";
                    using (var command = new MySqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@cookId", cookId);
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            DisplaySuccess("Cuisinier supprimé avec succès!");
                        }
                        else
                        {
                            DisplayError("Erreur lors de la suppression du cuisinier.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void DisplayClientHistory()
        {
            Console.Clear();
            DisplayHeader("HISTORIQUE DES CLIENTS");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            c.Id,
                            c.Nom,
                            c.Prenom,
                            COUNT(DISTINCT cmd.Id) as NombreCommandes,
                            SUM(cmd.PrixTotal) as MontantTotal,
                            AVG(n.Note) as NoteMoyenne
                        FROM Clients c
                        LEFT JOIN Commandes cmd ON c.Id = cmd.ClientId
                        LEFT JOIN Notations n ON c.Id = n.NoteID AND n.Type = 1
                        GROUP BY c.Id, c.Nom, c.Prenom
                        ORDER BY NombreCommandes DESC";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Aucun client trouvé.");
                            WaitForKey();
                            return;
                        }

                        Console.WriteLine("\n{0,-5} {1,-15} {2,-15} {3,-15} {4,-15} {5,-15}",
                            "ID", "Nom", "Prénom", "Commandes", "Total (€)", "Note");
                        Console.WriteLine(new string('-', 80));

                        while (reader.Read())
                        {
                            Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-15} {4,-15:F2} {5,-15:F1}",
                                reader["Id"],
                                reader["Nom"],
                                reader["Prenom"],
                                reader["NombreCommandes"],
                                reader["MontantTotal"],
                                reader["NoteMoyenne"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void DisplayDishesByFrequency()
        {
            Console.Clear();
            DisplayHeader("PLATS PAR FRÉQUENCE");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            p.Nom,
                            COUNT(ci.Id) as NombreCommandes,
                            SUM(ci.Quantite) as TotalPortions,
                            AVG(ci.PrixUnitaire) as PrixMoyen
                        FROM Plats p
                        LEFT JOIN CommandeItems ci ON p.Id = ci.PlatId
                        GROUP BY p.Id, p.Nom
                        ORDER BY NombreCommandes DESC";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Aucun plat trouvé.");
                            WaitForKey();
                            return;
                        }

                        Console.WriteLine("\n{0,-30} {1,-15} {2,-15} {3,-15}",
                            "Plat", "Commandes", "Portions", "Prix Moyen");
                        Console.WriteLine(new string('-', 75));

                        while (reader.Read())
                        {
                            Console.WriteLine("{0,-30} {1,-15} {2,-15} {3,-15:C}",
                                reader["Nom"],
                                reader["NombreCommandes"],
                                reader["TotalPortions"],
                                reader["PrixMoyen"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void DisplayDishOfTheDay()
        {
            Console.Clear();
            DisplayHeader("PLAT DU JOUR");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            p.Nom,
                            p.PrixParPersonne,
                            c.Nom as CuisinierNom,
                            c.Prenom as CuisinierPrenom,
                            COUNT(ci.Id) as NombreCommandes
                        FROM Plats p
                        JOIN Cuisiniers c ON p.CuisinierId = c.Id
                        LEFT JOIN CommandeItems ci ON p.Id = ci.PlatId
                        WHERE p.DateExpiration >= CURDATE()
                        GROUP BY p.Id, p.Nom, p.PrixParPersonne, c.Nom, c.Prenom
                        ORDER BY NombreCommandes DESC
                        LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            Console.WriteLine("Aucun plat du jour disponible.");
                            WaitForKey();
                            return;
                        }

                        Console.WriteLine("\nPlat du jour:");
                        Console.WriteLine($"Nom: {reader["Nom"]}");
                        Console.WriteLine($"Prix: {reader["PrixParPersonne"]:C}");
                        Console.WriteLine($"Cuisinier: {reader["CuisinierPrenom"]} {reader["CuisinierNom"]}");
                        Console.WriteLine($"Nombre de commandes: {reader["NombreCommandes"]}");
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        // Order operations
        private void CreateOrder()
        {
            Console.Clear();
            DisplayHeader("CRÉATION D'UNE COMMANDE");

            try
            {
                /// 1. Sélectionner un client
                Console.WriteLine("Sélection du client:");

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    /// Récupérer la liste des clients
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
                        DisplayError("Aucun client n'est disponible. Impossible de créer une commande.");
                        WaitForKey();
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
                        DisplayError("Choix de client invalide.");
                        WaitForKey();
                        return;
                    }

                    var clientSelectionne = clients[indexClient - 1];

                    /// 2. Sélectionner la station de métro la plus proche pour la livraison
                    Console.WriteLine("\nSélection de la station de métro pour la livraison:");
                    var stations = ChargerStationsMetro();

                    /// Afficher les 5 stations les plus proches
                    var stationsProches = stations.OrderBy(s =>
                        CalculerDistance(s.Latitude, s.Longitude, 48.8566, 2.3522)) /// Coordonnées du centre de Paris
                        .Take(10)
                        .ToList();

                    for (int i = 0; i < stationsProches.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {stationsProches[i].Nom}");
                    }

                    Console.Write("\nChoisissez la station de métro la plus proche de votre adresse: ");
                    if (!int.TryParse(Console.ReadLine(), out int indexStation) ||
                        indexStation < 1 || indexStation > stationsProches.Count)
                    {
                        DisplayError("Choix de station invalide.");
                        WaitForKey();
                        return;
                    }

                    var stationLivraison = stationsProches[indexStation - 1];

                    /// 3. Charger le graphe du métro et afficher le chemin optimal
                    var grapheMetro = ChargerGrapheMetro();
                    if (grapheMetro != null)
                    {
                        Console.WriteLine("\nCalcul du chemin de livraison optimal...");

                        /// Trouver la station la plus proche du restaurant (simulé ici avec Châtelet comme point central)
                        var stationDepart = "Châtelet";

                        /// Calculer le chemin optimal avec Dijkstra
                        var distances = grapheMetro.Dijkstra(stationDepart);
                        var predecesseurs = new Dictionary<string, string>();

                        if (!distances.ContainsKey(stationLivraison.Nom) || distances[stationLivraison.Nom] == double.PositiveInfinity)
                        {
                            DisplayError("Aucun chemin trouvé entre ces stations.");
                            WaitForKey();
                            return;
                        }

                        var chemin = grapheMetro.ReconstruireChemin(stationDepart, stationLivraison.Nom, predecesseurs);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nChemin optimal trouvé ({Math.Round(distances[stationLivraison.Nom])} minutes):");
                        Console.WriteLine(string.Join(" → ", chemin));
                        Console.ResetColor();
                    }

                    /// 4. Sélectionner les plats
                    Console.WriteLine("\nSélection des plats:");
                    var plats = new List<(int Id, string Nom, decimal Prix)>();
                    using (var cmd = new MySqlCommand("SELECT Id, Nom, PrixParPersonne FROM Plats WHERE DateExpiration > NOW()", connection))
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
                        DisplayError("Aucun plat disponible.");
                        WaitForKey();
                        return;
                    }

                    var commandeItems = new List<(int PlatId, int Quantite, decimal PrixUnitaire)>();
                    decimal totalPrix = 0;

                    while (true)
                    {
                        Console.WriteLine("\nPlats disponibles:");
                        for (int i = 0; i < plats.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {plats[i].Nom} - {plats[i].Prix:C} par personne");
                        }
                        Console.WriteLine("0. Terminer la sélection");

                        Console.Write("\nVotre choix: ");
                        if (!int.TryParse(Console.ReadLine(), out int choixPlat) || choixPlat < 0 || choixPlat > plats.Count)
                        {
                            DisplayError("Choix invalide.");
                            continue;
                        }

                        if (choixPlat == 0)
                            break;

                        Console.Write("Nombre de personnes: ");
                        if (!int.TryParse(Console.ReadLine(), out int quantite) || quantite <= 0)
                        {
                            DisplayError("Quantité invalide.");
                            continue;
                        }

                        var platSelectionne = plats[choixPlat - 1];
                        commandeItems.Add((platSelectionne.Id, quantite, platSelectionne.Prix));
                        totalPrix += platSelectionne.Prix * quantite;

                        Console.WriteLine($"\nPlat ajouté: {platSelectionne.Nom} pour {quantite} personne(s)");
                        Console.WriteLine($"Sous-total: {platSelectionne.Prix * quantite:C}");
                    }

                    if (commandeItems.Count == 0)
                    {
                        DisplayError("Aucun plat sélectionné.");
                        WaitForKey();
                        return;
                    }

                    /// 5. Confirmer la commande
                    Console.WriteLine($"\nTotal de la commande: {totalPrix:C}");
                    Console.WriteLine($"Point de livraison: Station {stationLivraison.Nom}");
                    Console.Write("Confirmer la commande? (O/N): ");

                    if (Console.ReadLine()?.Trim().ToUpper().StartsWith("O") == true)
                    {
                        /// Créer la commande
                        string insertOrderQuery = @"
                            INSERT INTO Commandes (ClientId, DateCommande, DateLivraisonPrevue, PrixTotal, AdresseLivraison, Statut) 
                            VALUES (@clientId, NOW(), DATE_ADD(NOW(), INTERVAL 2 HOUR), @totalPrice, @deliveryAddress, 0)";

                        int orderId;
                        using (var cmd = new MySqlCommand(insertOrderQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@clientId", clientSelectionne.Id);
                            cmd.Parameters.AddWithValue("@totalPrice", totalPrix);
                            cmd.Parameters.AddWithValue("@deliveryAddress", $"Station de métro: {stationLivraison.Nom}");
                            cmd.ExecuteNonQuery();
                            orderId = (int)cmd.LastInsertedId;
                        }

                        /// Ajouter les items de la commande
                        string insertOrderItemQuery = @"
                            INSERT INTO CommandeItems (CommandeId, PlatId, Quantite, PrixUnitaire) 
                            VALUES (@orderId, @platId, @quantity, @unitPrice)";

                        foreach (var item in commandeItems)
                        {
                            using (var cmd = new MySqlCommand(insertOrderItemQuery, connection))
                            {
                                cmd.Parameters.AddWithValue("@orderId", orderId);
                                cmd.Parameters.AddWithValue("@platId", item.PlatId);
                                cmd.Parameters.AddWithValue("@quantity", item.Quantite);
                                cmd.Parameters.AddWithValue("@unitPrice", item.PrixUnitaire);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        DisplaySuccess($"Commande créée avec succès (ID: {orderId})");
                    }
                    else
                    {
                        Console.WriteLine("Création de commande annulée.");
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de la création de la commande: {ex.Message}");
            }

            WaitForKey();
        }

        private void ModifyOrder()
        {
            Console.Clear();
            DisplayHeader("MODIFICATION D'UNE COMMANDE");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // 1. Afficher la liste des commandes
                    string query = @"
                        SELECT c.Id, c.DateCommande, c.Statut, cl.Nom, cl.Prenom, c.AdresseLivraison
                    FROM Commandes c
                        JOIN Clients cl ON c.ClientId = cl.Id
                        ORDER BY c.DateCommande DESC";

                    var commandes = new List<(int Id, DateTime Date, string Status, string Nom, string Prenom, string Adresse)>();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sbyte statutValue = reader.GetSByte("Statut");
                                string statut = statutValue switch
                                {
                                    0 => "En attente",
                                    1 => "Confirmée",
                                    2 => "Livrée",
                                    3 => "Annulée",
                                    _ => "Inconnu"
                                };

                                commandes.Add((
                                    reader.GetInt32("Id"),
                                    reader.GetDateTime("DateCommande"),
                                    statut,
                                    reader.GetString("Nom"),
                                    reader.GetString("Prenom"),
                                    reader.GetString("AdresseLivraison")
                                ));
                            }
                        }
                    }

                    if (commandes.Count == 0)
                    {
                        DisplayError("Aucune commande disponible.");
                        WaitForKey();
                        return;
                    }

                    // Afficher les commandes
                    Console.WriteLine("Liste des commandes:");
                    for (int i = 0; i < commandes.Count; i++)
                    {
                        var cmd = commandes[i];
                        Console.WriteLine($"{i + 1}. Commande #{cmd.Id} - {cmd.Date:dd/MM/yyyy HH:mm} - {cmd.Nom} {cmd.Prenom} - {cmd.Status}");
                    }

                    // 2. Sélectionner une commande
                    Console.Write("\nEntrez le numéro de la commande à modifier (0 pour annuler): ");
                    if (!int.TryParse(Console.ReadLine(), out int selection) || selection < 0 || selection > commandes.Count)
                    {
                        DisplayError("Sélection invalide.");
                        WaitForKey();
                        return;
                    }

                    if (selection == 0) return;

                    var commandeSelectionnee = commandes[selection - 1];

                    // 3. Afficher les statuts disponibles
                    Console.WriteLine("\nStatuts disponibles:");
                    Console.WriteLine("1. En attente");
                    Console.WriteLine("2. Confirmée");
                    Console.WriteLine("3. Livrée");
                    Console.WriteLine("4. Annulée");

                    // 4. Sélectionner le nouveau statut
                    Console.Write("\nEntrez le nouveau statut (0 pour annuler): ");
                    if (!int.TryParse(Console.ReadLine(), out int statutSelection) || statutSelection < 0 || statutSelection > 4)
                    {
                        DisplayError("Statut invalide.");
                        WaitForKey();
                        return;
                    }

                    if (statutSelection == 0) return;

                    // Convertir le choix en valeur TINYINT
                    sbyte nouveauStatut = (sbyte)(statutSelection - 1);

                    // 5. Mettre à jour le statut
                    string updateQuery = "UPDATE Commandes SET Statut = @Statut WHERE Id = @Id";
                    using (var cmd = new MySqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Statut", nouveauStatut);
                        cmd.Parameters.AddWithValue("@Id", commandeSelectionnee.Id);
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            DisplaySuccess($"Statut de la commande #{commandeSelectionnee.Id} mis à jour avec succès!");

                            // Si la commande est livrée, proposer d'ajouter une notation
                            if (nouveauStatut == 2) // 2 = Livrée
                            {
                                Console.Write("\nVoulez-vous ajouter une notation pour cette commande ? (O/N): ");
                                if (Console.ReadLine()?.Trim().ToUpper().StartsWith("O") == true)
                                {
                                    AjouterNotation(commandeSelectionnee.Id);
                                }
                            }
                        }
                        else
                        {
                            DisplayError("Erreur lors de la mise à jour du statut.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void AjouterNotation(int commandeId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // Récupérer les informations de la commande
                    string query = @"
                        SELECT c.Id, c.ClientId, p.CuisinierId, cl.Nom as ClientNom, cl.Prenom as ClientPrenom,
                               cu.Nom as CuisinierNom, cu.Prenom as CuisinierPrenom
                        FROM Commandes c
                        JOIN Clients cl ON c.ClientId = cl.Id
                        JOIN CommandeItems ci ON c.Id = ci.CommandeId
                        JOIN Plats p ON ci.PlatId = p.Id
                        JOIN Cuisiniers cu ON p.CuisinierId = cu.Id
                        WHERE c.Id = @CommandeId
                        LIMIT 1";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CommandeId", commandeId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                DisplayError("Impossible de trouver les informations de la commande.");
                                return;
                            }

                            int clientId = reader.GetInt32("ClientId");
                            int cuisinierId = reader.GetInt32("CuisinierId");
                            string clientNom = $"{reader.GetString("ClientPrenom")} {reader.GetString("ClientNom")}";
                            string cuisinierNom = $"{reader.GetString("CuisinierPrenom")} {reader.GetString("CuisinierNom")}";

                            // Demander la notation du client pour le cuisinier
                            Console.WriteLine($"\nNotation pour le cuisinier {cuisinierNom}:");
                            Console.Write("Note (1-5 étoiles): ");
                            if (!int.TryParse(Console.ReadLine(), out int noteClient) || noteClient < 1 || noteClient > 5)
                            {
                                DisplayError("Note invalide. La notation doit être entre 1 et 5.");
                                return;
                            }

                            Console.Write("Commentaire (optionnel): ");
                            string commentaireClient = Console.ReadLine() ?? "";

                            // Insérer la notation du client
                            string insertNotationQuery = @"
                                INSERT INTO Notations (CommandeID, NoteurID, NoteID, Type, Note, Commentaire, DateNotation)
                                VALUES (@CommandeId, @NoteurId, @NoteId, @Type, @Note, @Commentaire, NOW())";

                            using (var insertCmd = new MySqlCommand(insertNotationQuery, connection))
                            {
                                insertCmd.Parameters.AddWithValue("@CommandeId", commandeId);
                                insertCmd.Parameters.AddWithValue("@NoteurId", clientId);
                                insertCmd.Parameters.AddWithValue("@NoteId", cuisinierId);
                                insertCmd.Parameters.AddWithValue("@Type", (int)TypeNotation.ClientVersCuisinier);
                                insertCmd.Parameters.AddWithValue("@Note", noteClient);
                                insertCmd.Parameters.AddWithValue("@Commentaire", commentaireClient);
                                insertCmd.ExecuteNonQuery();
                            }

                            // Demander la notation du cuisinier pour le client
                            Console.WriteLine($"\nNotation pour le client {clientNom}:");
                            Console.Write("Note (1-5 étoiles): ");
                            if (!int.TryParse(Console.ReadLine(), out int noteCuisinier) || noteCuisinier < 1 || noteCuisinier > 5)
                            {
                                DisplayError("Note invalide. La notation doit être entre 1 et 5.");
                                return;
                            }

                            Console.Write("Commentaire (optionnel): ");
                            string commentaireCuisinier = Console.ReadLine() ?? "";

                            // Insérer la notation du cuisinier
                            using (var insertCmd = new MySqlCommand(insertNotationQuery, connection))
                            {
                                insertCmd.Parameters.AddWithValue("@CommandeId", commandeId);
                                insertCmd.Parameters.AddWithValue("@NoteurId", cuisinierId);
                                insertCmd.Parameters.AddWithValue("@NoteId", clientId);
                                insertCmd.Parameters.AddWithValue("@Type", (int)TypeNotation.CuisinierVersClient);
                                insertCmd.Parameters.AddWithValue("@Note", noteCuisinier);
                                insertCmd.Parameters.AddWithValue("@Commentaire", commentaireCuisinier);
                                insertCmd.ExecuteNonQuery();
                            }

                            // Mettre à jour les notes moyennes
                            string updateClientRatingQuery = @"
                                UPDATE Clients 
                                SET NoteMoyenne = (
                                    SELECT AVG(Note) 
                                    FROM Notations 
                                    WHERE Type = @TypeClient AND NoteID = @ClientId
                                )
                                WHERE Id = @ClientId";

                            string updateCuisinierRatingQuery = @"
                                UPDATE Cuisiniers 
                                SET NoteMoyenne = (
                                    SELECT AVG(Note) 
                                    FROM Notations 
                                    WHERE Type = @TypeCuisinier AND NoteID = @CuisinierId
                                )
                                WHERE Id = @CuisinierId";

                            using (var updateCmd = new MySqlCommand(updateClientRatingQuery, connection))
                            {
                                updateCmd.Parameters.AddWithValue("@TypeClient", (int)TypeNotation.CuisinierVersClient);
                                updateCmd.Parameters.AddWithValue("@ClientId", clientId);
                                updateCmd.ExecuteNonQuery();
                            }

                            using (var updateCmd = new MySqlCommand(updateCuisinierRatingQuery, connection))
                            {
                                updateCmd.Parameters.AddWithValue("@TypeCuisinier", (int)TypeNotation.ClientVersCuisinier);
                                updateCmd.Parameters.AddWithValue("@CuisinierId", cuisinierId);
                                updateCmd.ExecuteNonQuery();
                            }

                            DisplaySuccess("Notations ajoutées avec succès!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de l'ajout des notations: {ex.Message}");
            }
        }

        private void DisplayMetroStats()
        {
            Console.Clear();
            DisplayHeader("STATISTIQUES DU RÉSEAU MÉTRO");

            try
            {
                // Extract connection string parameters
                string server = GetConnectionStringParameter(_connectionString, "Server");
                string database = GetConnectionStringParameter(_connectionString, "Database");
                string uid = GetConnectionStringParameter(_connectionString, "Uid");
                string password = GetConnectionStringParameter(_connectionString, "Pwd");

                var metroNetwork = new Metro.MetroNetworkDatabase(server, database, uid, password);
                metroNetwork.PrintNetworkStats();
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de l'affichage des statistiques: {ex.Message}");
            }

            WaitForKey();
        }

        private void CalculateRoute()
        {
            Console.Clear();
            DisplayHeader("CALCULER UN ITINÉRAIRE");

            try
            {
                Console.Write("Station de départ: ");
                string fromStation = Console.ReadLine();

                Console.Write("Station d'arrivée: ");
                string toStation = Console.ReadLine();

                if (string.IsNullOrEmpty(fromStation) || string.IsNullOrEmpty(toStation))
                {
                    DisplayError("Stations invalides.");
                    WaitForKey();
                    return;
                }

                // Extract connection string parameters
                string server = GetConnectionStringParameter(_connectionString, "Server");
                string database = GetConnectionStringParameter(_connectionString, "Database");
                string uid = GetConnectionStringParameter(_connectionString, "Uid");
                string password = GetConnectionStringParameter(_connectionString, "Pwd");

                var metroNetwork = new Metro.MetroNetworkDatabase(server, database, uid, password);
                metroNetwork.CalculateRoute(fromStation, toStation);
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors du calcul de l'itinéraire: {ex.Message}");
            }

            WaitForKey();
        }

        private void GenerateVisualization(List<string> highlightedPath = null)
        {
            Console.Clear();
            DisplayHeader("GÉNÉRER UNE VISUALISATION");

            try
            {
                Console.Write("Nom du fichier de sortie (PNG): ");
                string outputFile = Console.ReadLine();

                if (string.IsNullOrEmpty(outputFile))
                {
                    outputFile = "metro_visualization.png";
                }

                if (!outputFile.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    outputFile += ".png";
                }

                using (var surface = SKSurface.Create(new SKImageInfo(1920, 1080)))
                {
                    var canvas = surface.Canvas;
                    canvas.Clear(SKColors.White);

                    // Charger les stations et les connexions
                    var stations = ChargerStationsMetro();
                    var grapheMetro = ChargerGrapheMetro();

                    if (grapheMetro == null || stations.Count == 0)
                    {
                        throw new Exception("Impossible de charger les données du métro.");
                    }

                    // Calculer les positions des stations
                    var stationPositions = CalculerPositionsStations(stations);

                    // Dessiner les connexions
                    using (var paint = new SKPaint
                    {
                        Color = SKColors.Gray,
                        StrokeWidth = 2,
                        IsAntialias = true
                    })
                    {
                        foreach (var lien in grapheMetro.Liens)
                        {
                            if (stationPositions.TryGetValue(lien.Noeud1.Id, out var posDepart) &&
                                stationPositions.TryGetValue(lien.Noeud2.Id, out var posArrivee))
                            {
                                // Si le chemin est surligné, utiliser une couleur différente
                                if (highlightedPath != null &&
                                    highlightedPath.Contains(lien.Noeud1.Id) &&
                                    highlightedPath.Contains(lien.Noeud2.Id))
                                {
                                    paint.Color = SKColors.Red;
                                    paint.StrokeWidth = 4;
                                }
                                else
                                {
                                    paint.Color = SKColors.Gray;
                                    paint.StrokeWidth = 2;
                                }

                                canvas.DrawLine(posDepart.X, posDepart.Y, posArrivee.X, posArrivee.Y, paint);
                            }
                        }
                    }

                    // Dessiner les stations
                    using (var paint = new SKPaint
                    {
                        Color = SKColors.Blue,
                        IsAntialias = true
                    })
                    {
                        using (var textPaint = new SKPaint
                        {
                            Color = SKColors.Black,
                            TextSize = 14,
                            IsAntialias = true
                        })
                        {
                            foreach (var station in stations)
                            {
                                if (stationPositions.TryGetValue(station.Nom, out var pos))
                                {
                                    // Dessiner le point de la station
                                    canvas.DrawCircle(pos.X, pos.Y, 6, paint);

                                    // Dessiner le nom de la station
                                    canvas.DrawText(station.Nom, pos.X + 10, pos.Y - 10, textPaint);
                                }
                            }
                        }
                    }

                    // Sauvegarder l'image
                    using (var image = surface.Snapshot())
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var stream = File.OpenWrite(outputFile))
                    {
                        data.SaveTo(stream);
                    }
                }

                DisplaySuccess($"Visualisation générée avec succès dans {outputFile}");
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de la génération de la visualisation: {ex.Message}");
            }

            WaitForKey();
        }

        private Dictionary<string, SKPoint> CalculerPositionsStations(List<Station> stations)
        {
            var positions = new Dictionary<string, SKPoint>();
            const float margin = 100;
            const float width = 1920 - 2 * margin;
            const float height = 1080 - 2 * margin;

            // Trouver les limites des coordonnées
            float minLat = float.MaxValue, maxLat = float.MinValue;
            float minLon = float.MaxValue, maxLon = float.MinValue;

            foreach (var station in stations)
            {
                if (station.Latitude < minLat) minLat = station.Latitude;
                if (station.Latitude > maxLat) maxLat = station.Latitude;
                if (station.Longitude < minLon) minLon = station.Longitude;
                if (station.Longitude > maxLon) maxLon = station.Longitude;
            }

            // Calculer les positions relatives
            foreach (var station in stations)
            {
                float x = margin + (station.Longitude - minLon) / (maxLon - minLon) * width;
                float y = margin + (maxLat - station.Latitude) / (maxLat - minLat) * height;
                positions[station.Nom] = new SKPoint(x, y);
            }

            return positions;
        }

        // Helper method to extract connection string parameters
        private string GetConnectionStringParameter(string connectionString, string parameterName)
        {
            string[] parameters = connectionString.Split(';');
            foreach (string parameter in parameters)
            {
                string[] keyValue = parameter.Split('=');
                if (keyValue.Length == 2 && keyValue[0].Trim() == parameterName)
                {
                    return keyValue[1].Trim();
                }
            }
            return string.Empty;
        }

        // Helper methods for the UI
        private void DisplayHeader(string title)
        {
            Console.ForegroundColor = _headerColor;
            Console.WriteLine(new string('=', 50));
            Console.WriteLine(title.PadLeft(25 + title.Length / 2));
            Console.WriteLine(new string('=', 50));
            Console.ResetColor();
            Console.WriteLine();
        }

        private void DisplayError(string message)
        {
            Console.ForegroundColor = _errorColor;
            Console.WriteLine($"\n[ERREUR] {message}");
            Console.ResetColor();
        }

        private void DisplaySuccess(string message)
        {
            Console.ForegroundColor = _successColor;
            Console.WriteLine($"\n[SUCCÈS] {message}");
            Console.ResetColor();
        }

        private void DisplayNotImplemented()
        {
            Console.Clear();
            DisplayHeader("FONCTIONNALITÉ NON IMPLÉMENTÉE");
            Console.WriteLine("Cette fonctionnalité sera disponible dans une version ultérieure.");
            WaitForKey();
        }

        private void WaitForKey()
        {
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        // Helper method to convert XML format if necessary
        private string ConvertXmlFormat(string inputPath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(inputPath);

                Console.WriteLine("Analyse du fichier XML...");

                /// Vérifier la structure pour déterminer si une conversion est nécessaire
                var stations = doc.SelectNodes("//stations/station");
                if (stations == null || stations.Count == 0)
                {
                    /// Essayer un autre XPath
                    stations = doc.SelectNodes("//station");
                }

                Console.WriteLine($"Trouvé {stations?.Count ?? 0} stations dans le XML.");

                /// Si le format est déjà correct (a des attributs), pas besoin de conversion
                if (stations != null && stations.Count > 0 &&
                    stations[0].Attributes["id"] != null &&
                    stations[0].Attributes["name"] != null)
                {
                    Console.WriteLine("Format XML déjà compatible.");
                    return inputPath;
                }

                /// Créer un nouveau document XML avec le format basé sur les attributs
                XmlDocument newDoc = new XmlDocument();
                XmlElement root = newDoc.CreateElement("metro");
                newDoc.AppendChild(root);

                Console.WriteLine("Conversion du XML au format attendu...");

                /// Traiter chaque station
                if (stations != null)
                {
                    foreach (XmlNode station in stations)
                    {
                        XmlElement newStation = newDoc.CreateElement("station");

                        /// Obtenir l'ID à partir de l'attribut ou de l'élément enfant
                        string id = station.Attributes?["id"]?.Value;
                        if (string.IsNullOrEmpty(id))
                        {
                            /// Essayer d'obtenir l'id à partir de l'élément enfant
                            XmlNode idNode = station.SelectSingleNode("id");
                            if (idNode != null)
                            {
                                id = idNode.InnerText;
                            }
                            else
                            {
                                /// Utiliser l'attribut directement
                                id = station.InnerText;
                            }
                        }

                        /// Si toujours pas d'ID trouvé, en générer un
                        if (string.IsNullOrEmpty(id))
                        {
                            id = "s" + Guid.NewGuid().ToString("N").Substring(0, 8);
                        }

                        newStation.SetAttribute("id", id);

                        /// Obtenir le nom à partir de l'élément enfant "nom"
                        string name = null;
                        XmlNode nameNode = station.SelectSingleNode("nom");
                        if (nameNode != null && !string.IsNullOrEmpty(nameNode.InnerText))
                        {
                            name = nameNode.InnerText;
                        }

                        /// S'assurer que le nom n'est pas null
                        if (string.IsNullOrEmpty(name))
                        {
                            name = "Station " + id;
                        }

                        newStation.SetAttribute("name", name);
                        Console.WriteLine($"Station {id}: nom = {name}");

                        /// Obtenir les coordonnées
                        double x = 0, y = 0;
                        XmlNode latNode = station.SelectSingleNode("latitude");
                        XmlNode lonNode = station.SelectSingleNode("longitude");

                        if (latNode != null && lonNode != null &&
                            !string.IsNullOrEmpty(latNode.InnerText) &&
                            !string.IsNullOrEmpty(lonNode.InnerText))
                        {
                            double.TryParse(latNode.InnerText.Replace(',', '.'),
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out double lat);

                            double.TryParse(lonNode.InnerText.Replace(',', '.'),
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out double lon);

                            x = lon; /// Utiliser la longitude comme X
                            y = lat; /// Utiliser la latitude comme Y
                        }

                        newStation.SetAttribute("x", x.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        newStation.SetAttribute("y", y.ToString(System.Globalization.CultureInfo.InvariantCulture));

                        /// Définir l'attribut de ligne par défaut
                        newStation.SetAttribute("line", "1");

                        root.AppendChild(newStation);
                    }

                    Console.WriteLine($"Converti {stations.Count} stations avec succès.");

                    /// Ajouter les connexions (exemple simplifié - connecter les stations consécutives)
                    int stationCount = stations.Count;
                    int connectionCount = 0;

                    /// Créer des connexions pour chaque ligne (simplifié - juste connecter les stations séquentielles)
                    for (int i = 0; i < stationCount - 1; i++)
                    {
                        XmlNode station1 = stations[i];
                        XmlNode station2 = stations[i + 1];

                        /// Obtenir les IDs
                        string id1 = station1.Attributes?["id"]?.Value;
                        if (string.IsNullOrEmpty(id1))
                        {
                            XmlNode idNode = station1.SelectSingleNode("id");
                            id1 = idNode != null ? idNode.InnerText : null;
                        }

                        string id2 = station2.Attributes?["id"]?.Value;
                        if (string.IsNullOrEmpty(id2))
                        {
                            XmlNode idNode = station2.SelectSingleNode("id");
                            id2 = idNode != null ? idNode.InnerText : null;
                        }

                        /// Si les IDs sont trouvés, créer la connexion
                        if (!string.IsNullOrEmpty(id1) && !string.IsNullOrEmpty(id2))
                        {
                            /// Calculer la distance si les coordonnées sont disponibles
                            double distance = 0.5; /// Distance par défaut

                            XmlNode lat1Node = station1.SelectSingleNode("latitude");
                            XmlNode lon1Node = station1.SelectSingleNode("longitude");
                            XmlNode lat2Node = station2.SelectSingleNode("latitude");
                            XmlNode lon2Node = station2.SelectSingleNode("longitude");

                            if (lat1Node != null && lon1Node != null && lat2Node != null && lon2Node != null &&
                                !string.IsNullOrEmpty(lat1Node.InnerText) && !string.IsNullOrEmpty(lon1Node.InnerText) &&
                                !string.IsNullOrEmpty(lat2Node.InnerText) && !string.IsNullOrEmpty(lon2Node.InnerText))
                            {
                                double.TryParse(lat1Node.InnerText.Replace(',', '.'),
                                    System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    out double lat1);

                                double.TryParse(lon1Node.InnerText.Replace(',', '.'),
                                    System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    out double lon1);

                                double.TryParse(lat2Node.InnerText.Replace(',', '.'),
                                    System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    out double lat2);

                                double.TryParse(lon2Node.InnerText.Replace(',', '.'),
                                    System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    out double lon2);

                                /// Calcul simple de la distance (euclidienne)
                                distance = Math.Sqrt(Math.Pow(lon2 - lon1, 2) + Math.Pow(lat2 - lat1, 2)) * 111; /// Conversion approximative en km
                            }

                            XmlElement connection = newDoc.CreateElement("connection");
                            connection.SetAttribute("from", id1);
                            connection.SetAttribute("to", id2);
                            connection.SetAttribute("distance", distance.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
                            root.AppendChild(connection);
                            connectionCount++;
                        }
                    }

                    Console.WriteLine($"Ajouté {connectionCount} connexions entre stations.");
                }

                /// Sauvegarder dans un fichier temporaire
                string tempPath = Path.Combine(Path.GetTempPath(), "metro_converted.xml");
                newDoc.Save(tempPath);

                Console.WriteLine($"Fichier XML converti sauvegardé à {tempPath}");

                /// Debug: Valider le fichier de sortie
                try
                {
                    XmlDocument validateDoc = new XmlDocument();
                    validateDoc.Load(tempPath);

                    var convertedStations = validateDoc.SelectNodes("//station");
                    Console.WriteLine($"Vérification: {convertedStations?.Count ?? 0} stations dans le fichier converti.");

                    if (convertedStations != null && convertedStations.Count > 0)
                    {
                        XmlNode firstStation = convertedStations[0];
                        Console.WriteLine($"Premier station: id={firstStation.Attributes["id"]?.Value}, " +
                                         $"name={firstStation.Attributes["name"]?.Value}, " +
                                         $"x={firstStation.Attributes["x"]?.Value}, " +
                                         $"y={firstStation.Attributes["y"]?.Value}, " +
                                         $"line={firstStation.Attributes["line"]?.Value}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la validation: {ex.Message}");
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la conversion du XML: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return inputPath; /// Retourner le chemin original si la conversion échoue
            }
        }

        private List<Station> ChargerStationsMetro()
        {
            var stations = new List<Station>();
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load("public/metro.xml");

                var stationNodes = xmlDoc.SelectNodes("//station");
                if (stationNodes != null)
                {
                    foreach (XmlNode station in stationNodes)
                    {
                        var id = station.Attributes?["id"]?.Value ?? station.SelectSingleNode("id")?.InnerText ?? "unknown";
                        var nom = station.SelectSingleNode("nom")?.InnerText ?? "Station inconnue";
                        var adresse = station.SelectSingleNode("adresse")?.InnerText ?? "";
                        var latNode = station.SelectSingleNode("latitude");
                        var lonNode = station.SelectSingleNode("longitude");

                        if (nom != null && latNode != null && lonNode != null &&
                            float.TryParse(latNode.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out float lat) &&
                            float.TryParse(lonNode.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out float lon))
                        {
                            stations.Add(new Station(id, nom, adresse, lat, lon));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors du chargement des stations: {ex.Message}");
            }

            return stations;
        }

        private double CalculerDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; /// Rayon de la Terre en km
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRad(double deg)
        {
            return deg * Math.PI / 180;
        }

        private void ExportPlatsToJSON()
        {
            Console.Clear();
            DisplayHeader("EXPORTER LES PLATS EN JSON");

            try
            {
                Console.Write("Chemin du fichier de sortie: ");
                string outputPath = Console.ReadLine() ?? "plats.json";

                if (!outputPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    outputPath += ".json";
                }

                /// Récupérer les plats depuis la base de données
                var plats = new List<Plat>();
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Meals";
                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            plats.Add(new Plat
                            {
                                Id = reader.GetInt32("Id"),
                                Nom = reader.GetString("Name"),
                                PrixParPersonne = reader.GetDecimal("PricePerPerson"),
                                /// Ajouter d'autres propriétés selon votre modèle
                            });
                        }
                    }
                }

                /// Exporter en JSON
                SerializationService.ExporterPlatsJSON(plats, outputPath);
                DisplaySuccess($"Plats exportés avec succès dans {outputPath}");
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de l'export: {ex.Message}");
            }

            WaitForKey();
        }

        private void ImportPlatsFromJSON()
        {
            Console.Clear();
            DisplayHeader("IMPORTER LES PLATS DEPUIS JSON");

            try
            {
                Console.Write("Chemin du fichier JSON: ");
                string? inputPath = Console.ReadLine() ?? "";

                if (string.IsNullOrEmpty(inputPath))
                {
                    DisplayError("Chemin de fichier invalide.");
                    WaitForKey();
                    return;
                }

                /// Importer depuis JSON
                var plats = SerializationService.ImporterPlatsJSON(inputPath);

                /// Insérer dans la base de données
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    foreach (var plat in plats)
                    {
                        string query = @"
                            INSERT INTO Meals (Name, PricePerPerson) 
                            VALUES (@name, @price)
                            ON DUPLICATE KEY UPDATE 
                            PricePerPerson = @price";

                        using (var cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@name", plat.Nom);
                            cmd.Parameters.AddWithValue("@price", plat.PrixParPersonne);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                DisplaySuccess($"{plats.Count} plats importés avec succès.");
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de l'import: {ex.Message}");
            }

            WaitForKey();
        }

        private void ExportPlatsToXML()
        {
            Console.Clear();
            DisplayHeader("EXPORTER LES PLATS EN XML");

            try
            {
                Console.Write("Chemin du fichier de sortie: ");
                string outputPath = Console.ReadLine() ?? "plats.xml";

                if (!outputPath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    outputPath += ".xml";
                }

                /// Récupérer les plats depuis la base de données
                var plats = new List<Plat>();
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Meals";
                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            plats.Add(new Plat
                            {
                                Id = reader.GetInt32("Id"),
                                Nom = reader.GetString("Name"),
                                PrixParPersonne = reader.GetDecimal("PricePerPerson"),
                                /// Ajouter d'autres propriétés selon votre modèle
                            });
                        }
                    }
                }

                /// Exporter en XML
                SerializationService.ExporterPlatsXML(plats, outputPath);
                DisplaySuccess($"Plats exportés avec succès dans {outputPath}");
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de l'export: {ex.Message}");
            }

            WaitForKey();
        }

        private void ImportPlatsFromXML()
        {
            Console.Clear();
            DisplayHeader("IMPORTER LES PLATS DEPUIS XML");

            try
            {
                Console.Write("Chemin du fichier XML: ");
                string? inputPath = Console.ReadLine() ?? "";

                if (string.IsNullOrEmpty(inputPath))
                {
                    DisplayError("Chemin de fichier invalide.");
                    WaitForKey();
                    return;
                }

                /// Importer depuis XML
                var plats = SerializationService.ImporterPlatsXML(inputPath);

                /// Insérer dans la base de données
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    foreach (var plat in plats)
                    {
                        string query = @"
                            INSERT INTO Meals (Name, PricePerPerson) 
                            VALUES (@name, @price)
                            ON DUPLICATE KEY UPDATE 
                            PricePerPerson = @price";

                        using (var cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@name", plat.Nom);
                            cmd.Parameters.AddWithValue("@price", plat.PrixParPersonne);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                DisplaySuccess($"{plats.Count} plats importés avec succès.");
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de l'import: {ex.Message}");
            }

            WaitForKey();
        }

        private void ExportCommandesToJSON()
        {
            Console.Clear();
            DisplayHeader("EXPORTER LES COMMANDES EN JSON");

            try
            {
                Console.Write("Chemin du fichier de sortie: ");
                string outputPath = Console.ReadLine() ?? "commandes.json";

                if (!outputPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    outputPath += ".json";
                }

                /// Récupérer les commandes depuis la base de données
                var commandes = new List<Commande>();
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Orders";
                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            commandes.Add(new Commande
                            {
                                Id = reader.GetInt32("Id"),
                                ClientID = reader.GetInt32("ClientId"),
                                PrixTotal = reader.GetDecimal("TotalPrice"),
                                /// Ajouter d'autres propriétés selon votre modèle
                            });
                        }
                    }
                }

                /// Exporter en JSON
                SerializationService.ExporterCommandesJSON(commandes, outputPath);
                DisplaySuccess($"Commandes exportées avec succès dans {outputPath}");
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de l'export: {ex.Message}");
            }

            WaitForKey();
        }

        private void ImportCommandesFromJSON()
        {
            Console.Clear();
            DisplayHeader("IMPORTER LES COMMANDES DEPUIS JSON");

            try
            {
                Console.Write("Chemin du fichier JSON: ");
                string? inputPath = Console.ReadLine() ?? "";

                if (string.IsNullOrEmpty(inputPath))
                {
                    DisplayError("Chemin de fichier invalide.");
                    WaitForKey();
                    return;
                }

                /// Importer depuis JSON
                var commandes = SerializationService.ImporterCommandesJSON(inputPath);

                /// Insérer dans la base de données
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    foreach (var commande in commandes)
                    {
                        string query = @"
                            INSERT INTO Orders (ClientId, TotalPrice) 
                            VALUES (@clientId, @totalPrice)
                            ON DUPLICATE KEY UPDATE 
                            TotalPrice = @totalPrice";

                        using (var cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@clientId", commande.ClientID);
                            cmd.Parameters.AddWithValue("@totalPrice", commande.PrixTotal);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                DisplaySuccess($"{commandes.Count} commandes importées avec succès.");
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de l'import: {ex.Message}");
            }

            WaitForKey();
        }

        private void LoadMetroNetwork()
        {
            Console.Clear();
            DisplayHeader("CHARGER LE RÉSEAU MÉTRO");

            try
            {
                Console.Write("Entrez le chemin du fichier XML (ou laissez vide pour utiliser public/metro.xml): ");
                string? xmlFilePath = Console.ReadLine() ?? "public/metro.xml";

                if (string.IsNullOrEmpty(xmlFilePath))
                {
                    xmlFilePath = "public/metro.xml";
                    Console.WriteLine($"Utilisation du fichier par défaut: {xmlFilePath}");
                }

                if (!File.Exists(xmlFilePath))
                {
                    DisplayError($"Fichier non trouvé: {xmlFilePath}");
                    WaitForKey();
                    return;
                }

                // Vérifier le format XML et convertir si nécessaire
                string tempXmlPath = ConvertXmlFormat(xmlFilePath);

                // Extraire les paramètres de la chaîne de connexion
                string server = GetConnectionStringParameter(_connectionString, "Server");
                string database = GetConnectionStringParameter(_connectionString, "Database");
                string uid = GetConnectionStringParameter(_connectionString, "Uid");
                string password = GetConnectionStringParameter(_connectionString, "Pwd");

                var metroNetwork = new Metro.MetroNetworkDatabase(server, database, uid, password);
                metroNetwork.InitializeDatabase();
                metroNetwork.ImportFromXml(tempXmlPath);

                // Supprimer le fichier temporaire s'il a été créé
                if (tempXmlPath != xmlFilePath && File.Exists(tempXmlPath))
                {
                    File.Delete(tempXmlPath);
                }

                DisplaySuccess("Réseau métro chargé avec succès!");
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors du chargement du réseau: {ex.Message}");
            }

            WaitForKey();
        }

        private void DetermineDeliveryPath()
        {
            Console.Clear();
            DisplayHeader("DÉTERMINATION DU CHEMIN DE LIVRAISON");

            try
            {
                // Charger le graphe du métro
                var grapheMetro = ChargerGrapheMetro();
                if (grapheMetro == null)
                {
                    DisplayError("Impossible de charger le graphe du réseau métro.");
                    WaitForKey();
                    return;
                }

                // Lister les commandes en attente de livraison
                var commandesEnAttente = AfficherCommandesEnAttente();
                if (commandesEnAttente.Count == 0)
                {
                    DisplayError("Aucune commande en attente de livraison.");
                    WaitForKey();
                    return;
                }

                Console.Write("\nEntrez l'ID de la commande pour optimiser son chemin de livraison: ");
                if (!int.TryParse(Console.ReadLine(), out int commandeId) || !commandesEnAttente.Contains(commandeId))
                {
                    DisplayError("ID de commande invalide.");
                    WaitForKey();
                    return;
                }

                // Récupérer les adresses (origine et destination)
                var (adresseDepart, adresseArrivee) = RecupererAdressesCommande(commandeId);
                if (string.IsNullOrEmpty(adresseDepart) || string.IsNullOrEmpty(adresseArrivee))
                {
                    DisplayError("Impossible de récupérer les adresses pour cette commande.");
                    WaitForKey();
                    return;
                }

                // Convertir les adresses en stations de métro les plus proches
                var stationDepart = TrouverStationProche(adresseDepart);
                var stationArrivee = TrouverStationProche(adresseArrivee);

                Console.WriteLine($"\nStation de départ: {stationDepart}");
                Console.WriteLine($"Station d'arrivée: {stationArrivee}");

                // Calculer le chemin optimal avec Dijkstra
                Console.WriteLine("\nCalcul du chemin optimal...");
                var distances = grapheMetro.Dijkstra(stationDepart);

                if (!distances.ContainsKey(stationArrivee) || distances[stationArrivee] == double.PositiveInfinity)
                {
                    DisplayError("Aucun chemin trouvé entre ces stations.");
                    WaitForKey();
                    return;
                }

                var predecesseurs = new Dictionary<string, string>();
                var chemin = grapheMetro.ReconstruireChemin(stationDepart, stationArrivee, predecesseurs);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nChemin optimal trouvé ({Math.Round(distances[stationArrivee])} minutes):");
                Console.WriteLine(string.Join(" → ", chemin));
                Console.ResetColor();

                // Add visualization option
                Console.Write("\nVoulez-vous visualiser ce chemin sur une carte ? (O/N): ");
                if (Console.ReadLine()?.Trim().ToUpper().StartsWith("O") == true)
                {
                    GenerateVisualization(chemin);
                }

                // Enregistrer ce chemin dans la base de données
                EnregistrerItineraireLivraison(commandeId, string.Join(" → ", chemin));

                DisplaySuccess("Chemin de livraison déterminé avec succès!");
            }
            catch (Exception ex)
            {
                DisplayError($"Une erreur s'est produite: {ex.Message}");
            }

            WaitForKey();
        }

        private void OptimiserPlanningCuisiniers()
        {
            Console.Clear();
            DisplayHeader("OPTIMISATION DU PLANNING DES CUISINIERS");

            try
            {
                // Créer un graphe des cuisiniers où deux cuisiniers sont liés s'ils ne peuvent pas travailler en même temps
                var grapheCuisiniers = new Graphe<string>(RepresentationMode.Liste);

                // Récupérer la liste des cuisiniers
                var cuisiniers = new List<(int Id, string Nom)>();
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, CONCAT(Prenom, ' ', Nom) AS NomComplet FROM Cuisiniers";

                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("Id");
                            string nom = reader.GetString("NomComplet");
                            cuisiniers.Add((id, nom));
                            grapheCuisiniers.AjouterNoeud(nom);
                        }
                    }

                    // Ajouter des contraintes entre cuisiniers
                    for (int i = 0; i < cuisiniers.Count; i++)
                    {
                        for (int j = i + 1; j < cuisiniers.Count; j++)
                        {
                            // Simulation: deux cuisiniers ont une contrainte avec une probabilité de 40%
                            if (new Random().NextDouble() < 0.4)
                            {
                                grapheCuisiniers.AjouterLien(cuisiniers[i].Nom, cuisiniers[j].Nom);
                            }
                        }
                    }
                }

                if (cuisiniers.Count == 0)
                {
                    DisplayError("Aucun cuisinier trouvé dans la base de données.");
                    WaitForKey();
                    return;
                }

                Console.WriteLine($"\n{cuisiniers.Count} cuisiniers chargés.");
                Console.WriteLine("Application de l'algorithme de coloration de graphe pour l'optimisation du planning...\n");

                // Appliquer l'algorithme de coloration de graphe
                grapheCuisiniers.AfficherColoration();

                DisplaySuccess("\nPlanning optimisé avec succès!");
                Console.WriteLine("\nLes cuisiniers de la même couleur peuvent travailler en même temps.");
                Console.WriteLine("Cela permet de minimiser le nombre de créneaux horaires nécessaires.");
            }
            catch (Exception ex)
            {
                DisplayError($"Une erreur s'est produite: {ex.Message}");
            }

            WaitForKey();
        }

        private Graphe<string> ChargerGrapheMetro()
        {
            try
            {
                // Créer un graphe du métro parisien (simplifié)
                var graphe = new Graphe<string>(RepresentationMode.Liste);

                // Charger les stations et connexions depuis la base de données
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // Vérifier si la table des stations de métro existe
                    bool tableExists = false;
                    string checkTableQuery = "SHOW TABLES LIKE 'MetroStations'";
                    using (var checkCmd = new MySqlCommand(checkTableQuery, connection))
                    {
                        using (var reader = checkCmd.ExecuteReader())
                        {
                            tableExists = reader.HasRows;
                        }
                    }

                    if (!tableExists)
                    {
                        // Créer un graphe par défaut avec des données fictives
                        return CreerGrapheMetroDefaut();
                    }

                    // Charger les stations
                    string stationsQuery = "SELECT Nom FROM MetroStations";
                    using (var stationsCmd = new MySqlCommand(stationsQuery, connection))
                    {
                        using (var reader = stationsCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                graphe.AjouterNoeud(reader.GetString("Nom"));
                            }
                        }
                    }

                    // Charger les connexions
                    string connexionsQuery = "SELECT StationDepart, StationArrivee, Duree FROM MetroConnexions";
                    using (var connexionsCmd = new MySqlCommand(connexionsQuery, connection))
                    {
                        using (var reader = connexionsCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string depart = reader.GetString("StationDepart");
                                string arrivee = reader.GetString("StationArrivee");
                                double duree = reader.GetDouble("Duree");

                                graphe.AjouterLien(depart, arrivee, duree);
                            }
                        }
                    }
                }

                return graphe;
            }
            catch
            {
                return CreerGrapheMetroDefaut();
            }
        }

        private Graphe<string> CreerGrapheMetroDefaut()
        {
            // Créer un graphe par défaut avec des données fictives
            var graphe = new Graphe<string>(RepresentationMode.Liste);

            // Ajouter quelques stations du centre de Paris
            string[] stations = {
                "Châtelet", "Hôtel de Ville", "Saint-Paul", "Bastille",
                "République", "Arts et Métiers", "Réaumur-Sébastopol",
                "Louvre-Rivoli", "Palais Royal", "Opéra", "Saint-Michel",
                "Odéon", "Cité", "Les Halles", "Strasbourg-Saint-Denis"
            };

            foreach (var station in stations)
            {
                graphe.AjouterNoeud(station);
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
                ("Strasbourg-Saint-Denis", "Château d'Eau", 2.0),
                ("Louvre-Rivoli", "Palais Royal", 1.5),
                ("Palais Royal", "Opéra", 3.0),
                ("Saint-Michel", "Odéon", 2.0),
                ("Saint-Michel", "Cité", 1.5),
                ("Cité", "Châtelet", 2.0)
            };

            foreach (var (depart, arrivee, duree) in connexions)
            {
                graphe.AjouterLien(depart, arrivee, duree);
            }

            return graphe;
        }

        private List<int> AfficherCommandesEnAttente()
        {
            var commandesEnAttente = new List<int>();
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT c.Id, c.DateCommande, cl.Nom, cl.Prenom, c.AdresseLivraison
                        FROM Commandes c
                        JOIN Clients cl ON c.ClientId = cl.Id
                        WHERE c.Statut = 0
                        ORDER BY c.DateCommande";

                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Aucune commande en attente.");
                            return commandesEnAttente;
                        }

                        Console.WriteLine("\nCommandes en attente de livraison:");
                        Console.WriteLine("{0,-5} {1,-20} {2,-30}", "ID", "Client", "Adresse de livraison");
                        Console.WriteLine(new string('-', 60));

                        while (reader.Read())
                        {
                            int id = reader.GetInt32("Id");
                            string nomClient = $"{reader.GetString("Prenom")} {reader.GetString("Nom")}";
                            string adresse = reader.GetString("AdresseLivraison");
                            DateTime date = reader.GetDateTime("DateCommande");

                            Console.WriteLine("{0,-5} {1,-20} {2,-30}",
                                id,
                                nomClient,
                                adresse);

                            commandesEnAttente.Add(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de la récupération des commandes: {ex.Message}");
            }

            return commandesEnAttente;
        }

        private (string adresseDepart, string adresseArrivee) RecupererAdressesCommande(int commandeId)
        {
            string adresseDepart = string.Empty;
            string adresseArrivee = string.Empty;

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            c.AdresseLivraison,
                            cu.Adresse as AdresseCuisinier
                        FROM Commandes c
                        JOIN CommandeItems ci ON c.Id = ci.CommandeId
                        JOIN Plats p ON ci.PlatId = p.Id
                        JOIN Cuisiniers cu ON p.CuisinierId = cu.Id
                        WHERE c.Id = @CommandeId
                        LIMIT 1";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CommandeId", commandeId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                adresseArrivee = reader.GetString("AdresseLivraison");
                                adresseDepart = reader.GetString("AdresseCuisinier");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de la récupération des adresses: {ex.Message}");
            }

            return (adresseDepart, adresseArrivee);
        }

        private string TrouverStationProche(string adresse)
        {
            try
            {
                var stations = ChargerStationsMetro();
                if (stations.Count == 0)
                {
                    return "Châtelet"; // Station par défaut
                }

                // Extraire les coordonnées de l'adresse (simulation)
                // Dans une vraie application, on utiliserait un service de géocodage
                double lat = 48.8566; // Latitude de Paris
                double lon = 2.3522;  // Longitude de Paris

                // Trouver la station la plus proche
                var stationProche = stations
                    .OrderBy(s => CalculerDistance(s.Latitude, s.Longitude, lat, lon))
                    .FirstOrDefault();

                return stationProche?.Nom ?? "Châtelet";
            }
            catch
            {
                return "Châtelet"; // Station par défaut en cas d'erreur
            }
        }

        private void EnregistrerItineraireLivraison(int commandeId, string itineraire)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        UPDATE Commandes 
                        SET ItineraireLivraison = @Itineraire
                        WHERE Id = @CommandeId";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Itineraire", itineraire);
                        cmd.Parameters.AddWithValue("@CommandeId", commandeId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de l'enregistrement de l'itinéraire: {ex.Message}");
            }
        }

        private void CalculatePrice()
        {
            Console.Clear();
            DisplayHeader("CALCUL DU PRIX");

            try
            {
                Console.Write("ID de la commande: ");
                if (!int.TryParse(Console.ReadLine(), out int commandeId))
                {
                    DisplayError("ID invalide.");
                    WaitForKey();
                    return;
                }

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            c.Id,
                            cl.Nom,
                            cl.Prenom,
                            SUM(ci.Quantite * ci.PrixUnitaire) as PrixTotal
                        FROM Commandes c
                        JOIN Clients cl ON c.ClientId = cl.Id
                        JOIN CommandeItems ci ON c.Id = ci.CommandeId
                        WHERE c.Id = @CommandeId
                        GROUP BY c.Id, cl.Nom, cl.Prenom";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CommandeId", commandeId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                decimal prixTotal = reader.GetDecimal("PrixTotal");
                                string nomClient = $"{reader.GetString("Prenom")} {reader.GetString("Nom")}";
                                Console.WriteLine($"\nClient: {nomClient}");
                                Console.WriteLine($"Prix total: {prixTotal:C}");
                            }
                            else
                            {
                                DisplayError("Commande non trouvée.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void DeliveriesPerCook()
        {
            Console.Clear();
            DisplayHeader("LIVRAISONS PAR CUISINIER");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            cu.Nom,
                            cu.Prenom,
                            COUNT(DISTINCT c.Id) as NombreLivraisons
                        FROM Cuisiniers cu
                        JOIN Plats p ON cu.Id = p.CuisinierId
                        JOIN CommandeItems ci ON p.Id = ci.PlatId
                        JOIN Commandes c ON ci.CommandeId = c.Id
                        WHERE c.Statut = 2
                        GROUP BY cu.Id, cu.Nom, cu.Prenom
                        ORDER BY NombreLivraisons DESC";

                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Aucune livraison effectuée.");
                            WaitForKey();
                            return;
                        }

                        Console.WriteLine("\n{0,-20} {1,-20} {2,-15}", "Nom", "Prénom", "Livraisons");
                        Console.WriteLine(new string('-', 60));

                        while (reader.Read())
                        {
                            Console.WriteLine("{0,-20} {1,-20} {2,-15}",
                                reader["Nom"],
                                reader["Prenom"],
                                reader["NombreLivraisons"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void OrdersByPeriod()
        {
            Console.Clear();
            DisplayHeader("COMMANDES PAR PÉRIODE");

            try
            {
                Console.WriteLine("Choisissez la période:");
                Console.WriteLine("1. Aujourd'hui");
                Console.WriteLine("2. Cette semaine");
                Console.WriteLine("3. Ce mois");
                Console.WriteLine("4. Cette année");
                Console.Write("\nVotre choix: ");

                if (!int.TryParse(Console.ReadLine(), out int choix) || choix < 1 || choix > 4)
                {
                    DisplayError("Choix invalide.");
                    WaitForKey();
                    return;
                }

                string dateCondition = choix switch
                {
                    1 => "DATE(c.DateCommande) = CURDATE()",
                    2 => "YEARWEEK(c.DateCommande) = YEARWEEK(CURDATE())",
                    3 => "MONTH(c.DateCommande) = MONTH(CURDATE()) AND YEAR(c.DateCommande) = YEAR(CURDATE())",
                    4 => "YEAR(c.DateCommande) = YEAR(CURDATE())",
                    _ => "1=1"
                };

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = $@"
                        SELECT 
                            DATE(c.DateCommande) as Date,
                            COUNT(*) as NombreCommandes,
                            SUM(c.PrixTotal) as MontantTotal
                        FROM Commandes c
                        WHERE {dateCondition}
                        GROUP BY DATE(c.DateCommande)
                        ORDER BY Date";

                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Aucune commande trouvée pour cette période.");
                            WaitForKey();
                            return;
                        }

                        Console.WriteLine("\n{0,-12} {1,-15} {2,-15}", "Date", "Commandes", "Montant Total");
                        Console.WriteLine(new string('-', 45));

                        while (reader.Read())
                        {
                            Console.WriteLine("{0,-12:dd/MM/yyyy} {1,-15} {2,-15:C}",
                                reader["Date"],
                                reader["NombreCommandes"],
                                reader["MontantTotal"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void AverageOrderPrice()
        {
            Console.Clear();
            DisplayHeader("PRIX MOYEN DES COMMANDES");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            AVG(PrixTotal) as PrixMoyen,
                            MIN(PrixTotal) as PrixMin,
                            MAX(PrixTotal) as PrixMax,
                            COUNT(*) as NombreCommandes
                        FROM Commandes
                        WHERE Statut = 2";

                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal prixMoyen = reader.GetDecimal("PrixMoyen");
                            decimal prixMin = reader.GetDecimal("PrixMin");
                            decimal prixMax = reader.GetDecimal("PrixMax");
                            int nombreCommandes = reader.GetInt32("NombreCommandes");

                            Console.WriteLine($"\nNombre de commandes: {nombreCommandes}");
                            Console.WriteLine($"Prix moyen: {prixMoyen:C}");
                            Console.WriteLine($"Prix minimum: {prixMin:C}");
                            Console.WriteLine($"Prix maximum: {prixMax:C}");
                        }
                        else
                        {
                            Console.WriteLine("Aucune donnée disponible.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void AverageClientAccounts()
        {
            Console.Clear();
            DisplayHeader("COMPTES CLIENT MOYENS");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            TypeClient,
                            COUNT(*) as NombreClients,
                            AVG(NoteMoyenne) as NoteMoyenne,
                            AVG(SELECT COUNT(*) FROM Commandes WHERE ClientId = Clients.Id) as CommandesMoyennes
                        FROM Clients
                        GROUP BY TypeClient";

                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Aucune donnée disponible.");
                            WaitForKey();
                            return;
                        }

                        Console.WriteLine("\n{0,-15} {1,-15} {2,-15} {3,-15}",
                            "Type Client", "Nombre", "Note Moyenne", "Commandes Moy.");
                        Console.WriteLine(new string('-', 60));

                        while (reader.Read())
                        {
                            string typeClient = reader["TypeClient"].ToString() == "individual" ? "Individuel" : "Entreprise";
                            Console.WriteLine("{0,-15} {1,-15} {2,-15:F1} {3,-15:F1}",
                                typeClient,
                                reader["NombreClients"],
                                reader["NoteMoyenne"],
                                reader["CommandesMoyennes"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }

        private void OrderListByClient()
        {
            Console.Clear();
            DisplayHeader("LISTE DES COMMANDES PAR CLIENT");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // Afficher la liste des clients
                    string clientsQuery = "SELECT Id, Nom, Prenom FROM Clients ORDER BY Nom, Prenom";
                    var clients = new List<(int Id, string Nom)>();

                    using (var cmd = new MySqlCommand(clientsQuery, connection))
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

                    if (clients.Count == 0)
                    {
                        Console.WriteLine("Aucun client trouvé.");
                        WaitForKey();
                        return;
                    }

                    // Afficher la liste des clients
                    for (int i = 0; i < clients.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {clients[i].Nom}");
                    }

                    Console.Write("\nChoisissez un client (numéro): ");
                    if (!int.TryParse(Console.ReadLine(), out int choix) || choix < 1 || choix > clients.Count)
                    {
                        DisplayError("Choix invalide.");
                        WaitForKey();
                        return;
                    }

                    var clientSelectionne = clients[choix - 1];

                    // Afficher les commandes du client
                    string commandesQuery = @"
                        SELECT 
                            c.Id,
                            c.DateCommande,
                            c.PrixTotal,
                            c.Statut
                        FROM Commandes c
                        WHERE c.ClientId = @ClientId
                        ORDER BY c.DateCommande DESC";

                    using (var cmd = new MySqlCommand(commandesQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ClientId", clientSelectionne.Id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                Console.WriteLine($"\nAucune commande trouvée pour {clientSelectionne.Nom}");
                                WaitForKey();
                                return;
                            }

                            Console.WriteLine($"\nCommandes de {clientSelectionne.Nom}:");
                            Console.WriteLine("{0,-8} {1,-20} {2,-12} {3,-15}",
                                "ID", "Date", "Prix", "Statut");
                            Console.WriteLine(new string('-', 60));

                            while (reader.Read())
                            {
                                string statut = reader.GetSByte("Statut") switch
                                {
                                    0 => "En attente",
                                    1 => "Confirmée",
                                    2 => "Livrée",
                                    3 => "Annulée",
                                    _ => "Inconnu"
                                };

                                Console.WriteLine("{0,-8} {1,-20:dd/MM/yyyy HH:mm} {2,-12:C} {3,-15}",
                                    reader["Id"],
                                    reader["DateCommande"],
                                    reader["PrixTotal"],
                                    statut);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur: {ex.Message}");
            }

            WaitForKey();
        }
    }
}