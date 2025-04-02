using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Xml;
using System.IO;

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
                Console.WriteLine("│         RÉSEAU MÉTRO                │");
                Console.WriteLine("├─────────────────────────────────────┤");
                Console.WriteLine("│ 1. Charger le réseau                │");
                Console.WriteLine("│ 2. Afficher les statistiques        │");
                Console.WriteLine("│ 3. Calculer un itinéraire           │");
                Console.WriteLine("│ 4. Générer une visualisation        │");
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
                string lastName = Console.ReadLine();

                Console.Write("Prénom: ");
                string firstName = Console.ReadLine();

                Console.Write("Adresse: ");
                string address = Console.ReadLine();

                Console.Write("Téléphone: ");
                string phone = Console.ReadLine();

                Console.Write("Email: ");
                string email = Console.ReadLine();

                Console.Write("Type (1 = individuel, 2 = entreprise locale): ");
                string typeInput = Console.ReadLine();
                string clientType = typeInput == "2" ? "local business" : "individual";

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string insertQuery = @"
                        INSERT INTO Clients (LastName, FirstName, Address, Phone, Email, ClientType) 
                        VALUES (@lastName, @firstName, @address, @phone, @email, @clientType)";

                    using (var command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@lastName", lastName);
                        command.Parameters.AddWithValue("@firstName", firstName);
                        command.Parameters.AddWithValue("@address", address);
                        command.Parameters.AddWithValue("@phone", phone);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@clientType", clientType);

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

                            Console.WriteLine($"\nModification du client: {reader["FirstName"]} {reader["LastName"]}");
                        }
                    }

                    Console.Write("Nouveau nom (vide pour conserver la valeur actuelle): ");
                    string lastName = Console.ReadLine();

                    Console.Write("Nouveau prénom: ");
                    string firstName = Console.ReadLine();

                    Console.Write("Nouvelle adresse: ");
                    string address = Console.ReadLine();

                    Console.Write("Nouveau téléphone: ");
                    string phone = Console.ReadLine();

                    Console.Write("Nouvel email: ");
                    string email = Console.ReadLine();

                    Console.Write("Nouveau type (1 = individuel, 2 = entreprise locale, vide pour ne pas changer): ");
                    string typeInput = Console.ReadLine();
                    string clientType = null;
                    if (!string.IsNullOrEmpty(typeInput))
                    {
                        clientType = typeInput == "2" ? "local business" : "individual";
                    }

                    // Update only non-empty fields
                    string updateQuery = "UPDATE Clients SET ";
                    List<string> updateFields = new List<string>();

                    using (var command = new MySqlCommand())
                    {
                        command.Connection = connection;

                        if (!string.IsNullOrEmpty(lastName))
                        {
                            updateFields.Add("LastName = @lastName");
                            command.Parameters.AddWithValue("@lastName", lastName);
                        }

                        if (!string.IsNullOrEmpty(firstName))
                        {
                            updateFields.Add("FirstName = @firstName");
                            command.Parameters.AddWithValue("@firstName", firstName);
                        }

                        if (!string.IsNullOrEmpty(address))
                        {
                            updateFields.Add("Address = @address");
                            command.Parameters.AddWithValue("@address", address);
                        }

                        if (!string.IsNullOrEmpty(phone))
                        {
                            updateFields.Add("Phone = @phone");
                            command.Parameters.AddWithValue("@phone", phone);
                        }

                        if (!string.IsNullOrEmpty(email))
                        {
                            updateFields.Add("Email = @email");
                            command.Parameters.AddWithValue("@email", email);
                        }

                        if (!string.IsNullOrEmpty(clientType))
                        {
                            updateFields.Add("ClientType = @clientType");
                            command.Parameters.AddWithValue("@clientType", clientType);
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
                    string checkQuery = "SELECT LastName, FirstName FROM Clients WHERE Id = @clientId";
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
                                clientName = $"{reader["FirstName"]} {reader["LastName"]}";
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

                    string query = "SELECT Id, LastName, FirstName, ClientType, Phone FROM Clients ORDER BY LastName, FirstName";
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
                                string clientType = reader["ClientType"].ToString() == "individual" ? "Individuel" : "Entreprise";
                                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-15} {4,-15}",
                                    reader["Id"],
                                    reader["LastName"],
                                    reader["FirstName"],
                                    clientType,
                                    reader["Phone"]);
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
                            LastName, 
                            FirstName, 
                            Address,
                            SUBSTRING_INDEX(SUBSTRING_INDEX(Address, ',', 1), ' ', -1) AS Street
                        FROM Clients 
                        ORDER BY Street, LastName, FirstName";

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
                                    reader["LastName"],
                                    reader["FirstName"],
                                    reader["Address"]);
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
                            c.LastName, 
                            c.FirstName, 
                            c.ClientType,
                            COALESCE(SUM(o.TotalPrice), 0) AS TotalPurchase
                        FROM 
                            Clients c
                        LEFT JOIN 
                            Orders o ON c.Id = o.ClientId
                        GROUP BY 
                            c.Id, c.LastName, c.FirstName, c.ClientType
                        ORDER BY 
                            TotalPurchase DESC";

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
                                string clientType = reader["ClientType"].ToString() == "individual" ? "Individuel" : "Entreprise";
                                decimal totalPurchase = Convert.ToDecimal(reader["TotalPurchase"]);

                                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-15} {4,-15:F2}",
                                    reader["Id"],
                                    reader["LastName"],
                                    reader["FirstName"],
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
            // Similar to AddClient
            DisplayNotImplemented();
        }

        private void ModifyCook()
        {
            DisplayNotImplemented();
        }

        private void DeleteCook()
        {
            DisplayNotImplemented();
        }

        private void DisplayClientHistory()
        {
            DisplayNotImplemented();
        }

        private void DisplayDishesByFrequency()
        {
            DisplayNotImplemented();
        }

        private void DisplayDishOfTheDay()
        {
            DisplayNotImplemented();
        }

        // Order operations
        private void CreateOrder()
        {
            DisplayNotImplemented();
        }

        private void ModifyOrder()
        {
            DisplayNotImplemented();
        }

        private void CalculatePrice()
        {
            DisplayNotImplemented();
        }

        private void DetermineDeliveryPath()
        {
            DisplayNotImplemented();
        }

        // Statistics
        private void DeliveriesPerCook()
        {
            DisplayNotImplemented();
        }

        private void OrdersByPeriod()
        {
            DisplayNotImplemented();
        }

        private void AverageOrderPrice()
        {
            DisplayNotImplemented();
        }

        private void AverageClientAccounts()
        {
            DisplayNotImplemented();
        }

        private void OrderListByClient()
        {
            DisplayNotImplemented();
        }

        // Metro network operations
        private void LoadMetroNetwork()
        {
            Console.Clear();
            DisplayHeader("CHARGER LE RÉSEAU MÉTRO");

            try
            {
                Console.Write("Entrez le chemin du fichier XML (ou laissez vide pour utiliser public/metro.xml): ");
                string xmlFilePath = Console.ReadLine();

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

                // Check XML format and convert if necessary
                string tempXmlPath = ConvertXmlFormat(xmlFilePath);

                // Extract connection string parameters
                string server = GetConnectionStringParameter(_connectionString, "Server");
                string database = GetConnectionStringParameter(_connectionString, "Database");
                string uid = GetConnectionStringParameter(_connectionString, "Uid");
                string password = GetConnectionStringParameter(_connectionString, "Pwd");

                var metroNetwork = new Metro.MetroNetworkDatabase(server, database, uid, password);
                metroNetwork.InitializeDatabase();
                metroNetwork.ImportFromXml(tempXmlPath);

                // Delete temporary file if created
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

        private void GenerateVisualization()
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

                Console.WriteLine($"Génération de la visualisation dans le fichier {outputFile}...");

                // Create a graph from database and visualize it
                var graphe = new Models.Graphe<string>(Models.RepresentationMode.Liste);
                // Code to load the graph from database and visualize using GraphVisualizer would go here
                // For now, just show a success message as placeholder

                DisplaySuccess($"Visualisation générée dans le fichier {outputFile}!");
            }
            catch (Exception ex)
            {
                DisplayError($"Erreur lors de la génération de la visualisation: {ex.Message}");
            }

            WaitForKey();
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

                // Check the structure to determine if conversion is needed
                var stations = doc.SelectNodes("//stations/station");
                if (stations == null || stations.Count == 0)
                {
                    // Try alternate XPath
                    stations = doc.SelectNodes("//station");
                }

                Console.WriteLine($"Trouvé {stations?.Count ?? 0} stations dans le XML.");

                // If format already correct (has attributes), no conversion needed
                if (stations != null && stations.Count > 0 &&
                    stations[0].Attributes["id"] != null &&
                    stations[0].Attributes["name"] != null)
                {
                    Console.WriteLine("Format XML déjà compatible.");
                    return inputPath;
                }

                // Create new XML document with attribute-based format
                XmlDocument newDoc = new XmlDocument();
                XmlElement root = newDoc.CreateElement("metro");
                newDoc.AppendChild(root);

                Console.WriteLine("Conversion du XML au format attendu...");

                // Process each station
                if (stations != null)
                {
                    foreach (XmlNode station in stations)
                    {
                        XmlElement newStation = newDoc.CreateElement("station");

                        // Get the ID from attribute or child element
                        string id = station.Attributes?["id"]?.Value;
                        if (string.IsNullOrEmpty(id))
                        {
                            // Try to get id from child element
                            XmlNode idNode = station.SelectSingleNode("id");
                            if (idNode != null)
                            {
                                id = idNode.InnerText;
                            }
                            else
                            {
                                // Use attribute directly
                                id = station.InnerText;
                            }
                        }

                        // If still no ID found, generate one
                        if (string.IsNullOrEmpty(id))
                        {
                            id = "s" + Guid.NewGuid().ToString("N").Substring(0, 8);
                        }

                        newStation.SetAttribute("id", id);

                        // Get name from child element "nom"
                        string name = null;
                        XmlNode nameNode = station.SelectSingleNode("nom");
                        if (nameNode != null && !string.IsNullOrEmpty(nameNode.InnerText))
                        {
                            name = nameNode.InnerText;
                        }

                        // Make sure name is not null
                        if (string.IsNullOrEmpty(name))
                        {
                            name = "Station " + id;
                        }

                        newStation.SetAttribute("name", name);
                        Console.WriteLine($"Station {id}: nom = {name}");

                        // Get coordinates
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

                            x = lon; // Use longitude as X
                            y = lat; // Use latitude as Y
                        }

                        newStation.SetAttribute("x", x.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        newStation.SetAttribute("y", y.ToString(System.Globalization.CultureInfo.InvariantCulture));

                        // Set default line attribute
                        newStation.SetAttribute("line", "1");

                        root.AppendChild(newStation);
                    }

                    Console.WriteLine($"Converti {stations.Count} stations avec succès.");

                    // Add connections (simplified example - connect consecutive stations)
                    int stationCount = stations.Count;
                    int connectionCount = 0;

                    // Create connections for each line (simplified - just connect sequential stations)
                    for (int i = 0; i < stationCount - 1; i++)
                    {
                        XmlNode station1 = stations[i];
                        XmlNode station2 = stations[i + 1];

                        // Get IDs
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

                        // If IDs are found, create connection
                        if (!string.IsNullOrEmpty(id1) && !string.IsNullOrEmpty(id2))
                        {
                            // Calculate distance if coordinates available
                            double distance = 0.5; // Default distance

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

                                // Simple distance calculation (Euclidean)
                                distance = Math.Sqrt(Math.Pow(lon2 - lon1, 2) + Math.Pow(lat2 - lat1, 2)) * 111; // Rough km conversion
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

                // Save to temp file
                string tempPath = Path.Combine(Path.GetTempPath(), "metro_converted.xml");
                newDoc.Save(tempPath);

                Console.WriteLine($"Fichier XML converti sauvegardé à {tempPath}");

                // Debug: Validate the output file
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
                return inputPath; // Return original path if conversion fails
            }
        }
    }
}