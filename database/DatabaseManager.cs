using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace LivInParis.database
{
    public class DatabaseManager
    {
        private readonly string _connectionString;
        private readonly string _server;
        private readonly string _adminUser;
        private readonly string _adminPassword;

        public DatabaseManager(string server, string adminUser, string adminPassword)
        {
            _server = server;
            _adminUser = adminUser;
            _adminPassword = adminPassword;
            _connectionString = $"Server={server};Uid={adminUser};Pwd={adminPassword};";
        }

        public void InitializeDatabase(string dbName)
        {
            try
            {
                // Connect to MySQL server
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connexion au serveur MySQL réussie.");

                    // Create database if not exists
                    string createDbQuery = $"CREATE DATABASE IF NOT EXISTS {dbName}";
                    using (var command = new MySqlCommand(createDbQuery, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine($"Base de données '{dbName}' créée ou déjà existante.");
                    }

                    // Use the database
                    string useDbQuery = $"USE {dbName}";
                    using (var command = new MySqlCommand(useDbQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Create tables
                    CreateTables(connection);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'initialisation de la base de données: {ex.Message}");
                throw;
            }
        }

        private void CreateTables(MySqlConnection connection)
        {
            // Create Clients table
            string createClientsTable = @"
                CREATE TABLE IF NOT EXISTS Clients (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    LastName VARCHAR(50) NOT NULL,
                    FirstName VARCHAR(50) NOT NULL,
                    Address VARCHAR(200) NOT NULL,
                    Phone VARCHAR(20) NOT NULL,
                    Email VARCHAR(100) NOT NULL,
                    ClientType ENUM('individual', 'local business') NOT NULL,
                    RegistrationDate DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            // Create Cooks table
            string createCooksTable = @"
                CREATE TABLE IF NOT EXISTS Cooks (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    LastName VARCHAR(50) NOT NULL,
                    FirstName VARCHAR(50) NOT NULL,
                    Address VARCHAR(200) NOT NULL,
                    Phone VARCHAR(20) NOT NULL,
                    Email VARCHAR(100) NOT NULL,
                    Rating DECIMAL(3,2) DEFAULT 0.0,
                    RegistrationDate DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            // Create Meals table
            string createMealsTable = @"
                CREATE TABLE IF NOT EXISTS Meals (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    CookId INT NOT NULL,
                    Name VARCHAR(100) NOT NULL,
                    Category ENUM('starter', 'main course', 'dessert') NOT NULL,
                    PortionCount INT NOT NULL,
                    PreparationDate DATETIME NOT NULL,
                    ExpirationDate DATETIME NOT NULL,
                    PricePerPerson DECIMAL(10,2) NOT NULL,
                    Nationality VARCHAR(50) NOT NULL,
                    DietaryRequirements VARCHAR(200),
                    MainIngredients VARCHAR(500) NOT NULL,
                    PhotoPath VARCHAR(255),
                    FOREIGN KEY (CookId) REFERENCES Cooks(Id)
                )";

            // Create Orders table
            string createOrdersTable = @"
                CREATE TABLE IF NOT EXISTS Orders (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    ClientId INT NOT NULL,
                    OrderDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    TotalPrice DECIMAL(10,2) NOT NULL,
                    DeliveryAddress VARCHAR(200) NOT NULL,
                    Status ENUM('pending', 'confirmed', 'delivered', 'cancelled') DEFAULT 'pending',
                    FOREIGN KEY (ClientId) REFERENCES Clients(Id)
                )";

            // Create OrderItems table
            string createOrderItemsTable = @"
                CREATE TABLE IF NOT EXISTS OrderItems (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    OrderId INT NOT NULL,
                    MealId INT NOT NULL,
                    Quantity INT NOT NULL,
                    UnitPrice DECIMAL(10,2) NOT NULL,
                    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
                    FOREIGN KEY (MealId) REFERENCES Meals(Id)
                )";

            using (var command = new MySqlCommand(createClientsTable, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Table 'Clients' créée ou déjà existante.");
            }

            using (var command = new MySqlCommand(createCooksTable, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Table 'Cooks' créée ou déjà existante.");
            }

            using (var command = new MySqlCommand(createMealsTable, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Table 'Meals' créée ou déjà existante.");
            }

            using (var command = new MySqlCommand(createOrdersTable, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Table 'Orders' créée ou déjà existante.");
            }

            using (var command = new MySqlCommand(createOrderItemsTable, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Table 'OrderItems' créée ou déjà existante.");
            }
        }

        public void CreateDatabaseUser(string username, string password, string dbName, string permissions)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // Create user if not exists
                    string createUserQuery = $"CREATE USER IF NOT EXISTS '{username}'@'localhost' IDENTIFIED BY '{password}'";
                    using (var command = new MySqlCommand(createUserQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Grant permissions
                    string grantQuery = $"GRANT {permissions} ON {dbName}.* TO '{username}'@'localhost'";
                    using (var command = new MySqlCommand(grantQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Apply changes
                    string flushQuery = "FLUSH PRIVILEGES";
                    using (var command = new MySqlCommand(flushQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    Console.WriteLine($"Utilisateur '{username}' créé avec les permissions: {permissions}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création de l'utilisateur: {ex.Message}");
                throw;
            }
        }

        public void PopulateSampleData(string dbName)
        {
            string dbConnectionString = $"Server={_server};Database={dbName};Uid={_adminUser};Pwd={_adminPassword};";

            try
            {
                using (var connection = new MySqlConnection(dbConnectionString))
                {
                    connection.Open();

                    // Start transaction for better performance
                    using (var transaction = connection.BeginTransaction())
                    {
                        // Insert sample clients
                        for (int i = 1; i <= 50; i++)
                        {
                            string clientType = i % 5 == 0 ? "local business" : "individual";
                            string insertClientQuery = @"
                                INSERT INTO Clients (LastName, FirstName, Address, Phone, Email, ClientType) 
                                VALUES (@lastName, @firstName, @address, @phone, @email, @clientType)";

                            using (var command = new MySqlCommand(insertClientQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@lastName", $"Nom{i}");
                                command.Parameters.AddWithValue("@firstName", $"Prénom{i}");
                                command.Parameters.AddWithValue("@address", $"{i} Rue de Paris, 75001 Paris");
                                command.Parameters.AddWithValue("@phone", $"+3361234{i:D4}");
                                command.Parameters.AddWithValue("@email", $"client{i}@example.com");
                                command.Parameters.AddWithValue("@clientType", clientType);
                                command.ExecuteNonQuery();
                            }
                        }
                        Console.WriteLine("50 clients ajoutés");

                        // Insert sample cooks
                        for (int i = 1; i <= 20; i++)
                        {
                            string insertCookQuery = @"
                                INSERT INTO Cooks (LastName, FirstName, Address, Phone, Email, Rating) 
                                VALUES (@lastName, @firstName, @address, @phone, @email, @rating)";

                            using (var command = new MySqlCommand(insertCookQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@lastName", $"Chef{i}");
                                command.Parameters.AddWithValue("@firstName", $"Pierre{i}");
                                command.Parameters.AddWithValue("@address", $"{i + 100} Av. des Champs-Élysées, 75008 Paris");
                                command.Parameters.AddWithValue("@phone", $"+3367890{i:D4}");
                                command.Parameters.AddWithValue("@email", $"chef{i}@cuisine.fr");
                                command.Parameters.AddWithValue("@rating", Math.Round(3.0 + (i % 20) / 10.0, 2));
                                command.ExecuteNonQuery();
                            }
                        }
                        Console.WriteLine("20 cuisiniers ajoutés");

                        // Insert sample meals (for each cook)
                        string[] mealCategories = { "starter", "main course", "dessert" };
                        string[] nationalities = { "French", "Italian", "Japanese", "Chinese", "Mexican", "Indian", "Lebanese" };

                        for (int cookId = 1; cookId <= 20; cookId++)
                        {
                            for (int j = 1; j <= 5; j++)
                            {
                                string category = mealCategories[j % 3];
                                string nationality = nationalities[(cookId + j) % nationalities.Length];

                                string insertMealQuery = @"
                                    INSERT INTO Meals (CookId, Name, Category, PortionCount, PreparationDate, 
                                    ExpirationDate, PricePerPerson, Nationality, DietaryRequirements, MainIngredients) 
                                    VALUES (@cookId, @name, @category, @portionCount, @preparationDate, 
                                    @expirationDate, @pricePerPerson, @nationality, @dietaryRequirements, @mainIngredients)";

                                using (var command = new MySqlCommand(insertMealQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@cookId", cookId);
                                    command.Parameters.AddWithValue("@name", $"Plat {j} du chef {cookId}");
                                    command.Parameters.AddWithValue("@category", category);
                                    command.Parameters.AddWithValue("@portionCount", 4 + (j % 4));
                                    command.Parameters.AddWithValue("@preparationDate", DateTime.Now.AddHours(-j));
                                    command.Parameters.AddWithValue("@expirationDate", DateTime.Now.AddDays(2));
                                    command.Parameters.AddWithValue("@pricePerPerson", Math.Round(8.5 + (j * 1.5), 2));
                                    command.Parameters.AddWithValue("@nationality", nationality);
                                    command.Parameters.AddWithValue("@dietaryRequirements", j % 2 == 0 ? "Vegetarian" : "");
                                    command.Parameters.AddWithValue("@mainIngredients", $"Ingredient1, Ingredient2, Ingredient{j + 3}");
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                        Console.WriteLine("100 plats ajoutés");

                        // Insert sample orders
                        Random random = new Random();
                        for (int i = 1; i <= 100; i++)
                        {
                            int clientId = random.Next(1, 51);
                            string[] statuses = { "pending", "confirmed", "delivered", "cancelled" };
                            string status = statuses[random.Next(statuses.Length)];

                            string insertOrderQuery = @"
                                INSERT INTO Orders (ClientId, TotalPrice, DeliveryAddress, Status) 
                                VALUES (@clientId, @totalPrice, @deliveryAddress, @status)";

                            decimal totalPrice = 0;

                            using (var command = new MySqlCommand(insertOrderQuery, connection, transaction))
                            {
                                string addressQuery = "SELECT Address FROM Clients WHERE Id = @clientId";
                                string address = "";

                                using (var addrCommand = new MySqlCommand(addressQuery, connection, transaction))
                                {
                                    addrCommand.Parameters.AddWithValue("@clientId", clientId);
                                    address = (string)addrCommand.ExecuteScalar();
                                }

                                command.Parameters.AddWithValue("@clientId", clientId);
                                command.Parameters.AddWithValue("@totalPrice", 0); // Will update later
                                command.Parameters.AddWithValue("@deliveryAddress", address);
                                command.Parameters.AddWithValue("@status", status);
                                command.ExecuteNonQuery();
                            }

                            // Get the inserted order ID
                            int orderId = 0;
                            using (var command = new MySqlCommand("SELECT LAST_INSERT_ID()", connection, transaction))
                            {
                                orderId = Convert.ToInt32(command.ExecuteScalar());
                            }

                            // Add 1-3 items to each order
                            int itemCount = random.Next(1, 4);
                            for (int j = 0; j < itemCount; j++)
                            {
                                int mealId = random.Next(1, 101);
                                int quantity = random.Next(1, 4);
                                decimal unitPrice = 0;

                                // Get meal price
                                string priceQuery = "SELECT PricePerPerson FROM Meals WHERE Id = @mealId";
                                using (var command = new MySqlCommand(priceQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@mealId", mealId);
                                    unitPrice = Convert.ToDecimal(command.ExecuteScalar());
                                }

                                string insertOrderItemQuery = @"
                                    INSERT INTO OrderItems (OrderId, MealId, Quantity, UnitPrice) 
                                    VALUES (@orderId, @mealId, @quantity, @unitPrice)";

                                using (var command = new MySqlCommand(insertOrderItemQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@orderId", orderId);
                                    command.Parameters.AddWithValue("@mealId", mealId);
                                    command.Parameters.AddWithValue("@quantity", quantity);
                                    command.Parameters.AddWithValue("@unitPrice", unitPrice);
                                    command.ExecuteNonQuery();
                                }

                                totalPrice += unitPrice * quantity;
                            }

                            // Update total price in order
                            string updateOrderPriceQuery = "UPDATE Orders SET TotalPrice = @totalPrice WHERE Id = @orderId";
                            using (var command = new MySqlCommand(updateOrderPriceQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@totalPrice", totalPrice);
                                command.Parameters.AddWithValue("@orderId", orderId);
                                command.ExecuteNonQuery();
                            }
                        }
                        Console.WriteLine("100 commandes ajoutées avec leurs détails");

                        // Commit all changes
                        transaction.Commit();
                        Console.WriteLine("Base de données peuplée avec succès");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du peuplement de la base de données: {ex.Message}");
                throw;
            }
        }
    }
}