using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using LivInParis.Models;

namespace LivInParis.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string server, string database, string username, string password)
        {
            _connectionString = $"Server={server};Database={database};Uid={username};Pwd={password};";
        }

        // Connecter à la base de données
        private MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        #region Opérations sur les Plats

        // Insérer un nouveau plat
        public int InsererPlat(Plat plat)
        {
            using (var connection = GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    string query = @"INSERT INTO Plats (Nom, Categorie, CuisinierID, NationaliteCuisine, 
                                    NombrePersonnes, DatePreparation, DateExpiration, PrixParPersonne, 
                                    PhotoUrl, EstDisponible) 
                                    VALUES (@Nom, @Categorie, @CuisinierID, @NationaliteCuisine, 
                                    @NombrePersonnes, @DatePreparation, @DateExpiration, @PrixParPersonne, 
                                    @PhotoUrl, @EstDisponible);
                                    SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySqlCommand(query, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Nom", plat.Nom);
                        cmd.Parameters.AddWithValue("@Categorie", (int)plat.Categorie);
                        cmd.Parameters.AddWithValue("@CuisinierID", plat.CuisinierID);
                        cmd.Parameters.AddWithValue("@NationaliteCuisine", plat.NationaliteCuisine);
                        cmd.Parameters.AddWithValue("@NombrePersonnes", plat.NombrePersonnes);
                        cmd.Parameters.AddWithValue("@DatePreparation", plat.DatePreparation);
                        cmd.Parameters.AddWithValue("@DateExpiration", plat.DateExpiration);
                        cmd.Parameters.AddWithValue("@PrixParPersonne", plat.PrixParPersonne);
                        cmd.Parameters.AddWithValue("@PhotoUrl", plat.PhotoUrl ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@EstDisponible", plat.EstDisponible);

                        int platID = Convert.ToInt32(cmd.ExecuteScalar());

                        // Insérer les ingrédients du plat
                        if (plat.IngredientsPhares != null && plat.IngredientsPhares.Count > 0)
                        {
                            foreach (var ingredient in plat.IngredientsPhares)
                            {
                                InsererIngredient(platID, ingredient, connection, transaction);
                            }
                        }

                        // Insérer les régimes alimentaires du plat
                        if (plat.RegimesAlimentaires != null && plat.RegimesAlimentaires.Count > 0)
                        {
                            foreach (var regime in plat.RegimesAlimentaires)
                            {
                                InsererRegimeAlimentaire(platID, regime, connection, transaction);
                            }
                        }

                        transaction.Commit();
                        return platID;
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private void InsererIngredient(int platID, string ingredient, MySqlConnection connection, MySqlTransaction transaction)
        {
            string query = "INSERT INTO IngredientsPlat (PlatID, NomIngredient) VALUES (@PlatID, @NomIngredient)";
            using (var cmd = new MySqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@PlatID", platID);
                cmd.Parameters.AddWithValue("@NomIngredient", ingredient);
                cmd.ExecuteNonQuery();
            }
        }

        private void InsererRegimeAlimentaire(int platID, string regime, MySqlConnection connection, MySqlTransaction transaction)
        {
            string query = "INSERT INTO RegimesAlimentairesPlat (PlatID, TypeRegime) VALUES (@PlatID, @TypeRegime)";
            using (var cmd = new MySqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@PlatID", platID);
                cmd.Parameters.AddWithValue("@TypeRegime", regime);
                cmd.ExecuteNonQuery();
            }
        }

        // Rechercher des plats avec filtres
        public List<Plat> RechercherPlats(string filtre = null, CategoriePlat? categorie = null,
                                        string nationalite = null, decimal? prixMax = null,
                                        string ingredient = null, string regime = null)
        {
            using (var connection = GetConnection())
            {
                string query = @"
                    SELECT p.*, c.Nom as CuisinierNom, c.Prenom as CuisinierPrenom
                    FROM Plats p
                    JOIN Cuisiniers c ON p.CuisinierID = c.Id
                    WHERE 1=1";

                if (!string.IsNullOrEmpty(filtre))
                    query += " AND p.Nom LIKE @Filtre";
                if (categorie.HasValue)
                    query += " AND p.Categorie = @Categorie";
                if (!string.IsNullOrEmpty(nationalite))
                    query += " AND p.NationaliteCuisine LIKE @Nationalite";
                if (prixMax.HasValue)
                    query += " AND p.PrixParPersonne <= @PrixMax";
                if (!string.IsNullOrEmpty(ingredient))
                    query += @" AND p.Id IN (
                                SELECT PlatID FROM IngredientsPlat 
                                WHERE NomIngredient LIKE @Ingredient)";
                if (!string.IsNullOrEmpty(regime))
                    query += @" AND p.Id IN (
                                SELECT PlatID FROM RegimesAlimentairesPlat 
                                WHERE TypeRegime LIKE @Regime)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(filtre))
                        cmd.Parameters.AddWithValue("@Filtre", $"%{filtre}%");
                    if (categorie.HasValue)
                        cmd.Parameters.AddWithValue("@Categorie", (int)categorie.Value);
                    if (!string.IsNullOrEmpty(nationalite))
                        cmd.Parameters.AddWithValue("@Nationalite", $"%{nationalite}%");
                    if (prixMax.HasValue)
                        cmd.Parameters.AddWithValue("@PrixMax", prixMax.Value);
                    if (!string.IsNullOrEmpty(ingredient))
                        cmd.Parameters.AddWithValue("@Ingredient", $"%{ingredient}%");
                    if (!string.IsNullOrEmpty(regime))
                        cmd.Parameters.AddWithValue("@Regime", $"%{regime}%");

                    var resultats = new List<Plat>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var plat = MapperPlat(reader);

                            // Après avoir fermé ce reader, on chargera les ingrédients et régimes.
                            resultats.Add(plat);
                        }
                    }

                    // Pour chaque plat, on charge ses ingrédients et régimes
                    foreach (var plat in resultats)
                    {
                        plat.IngredientsPhares = ChargerIngredientsPlat(plat.Id, connection);
                        plat.RegimesAlimentaires = ChargerRegimesPlat(plat.Id, connection);
                    }

                    return resultats;
                }
            }
        }

        private List<string> ChargerIngredientsPlat(int platID, MySqlConnection connection)
        {
            var ingredients = new List<string>();
            string query = "SELECT NomIngredient FROM IngredientsPlat WHERE PlatID = @PlatID";
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@PlatID", platID);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ingredients.Add(reader.GetString("NomIngredient"));
                    }
                }
            }
            return ingredients;
        }

        private List<string> ChargerRegimesPlat(int platID, MySqlConnection connection)
        {
            var regimes = new List<string>();
            string query = "SELECT TypeRegime FROM RegimesAlimentairesPlat WHERE PlatID = @PlatID";
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@PlatID", platID);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        regimes.Add(reader.GetString("TypeRegime"));
                    }
                }
            }
            return regimes;
        }

        private Plat MapperPlat(MySqlDataReader reader)
        {
            return new Plat
            {
                Id = reader.GetInt32("Id"),
                Nom = reader.GetString("Nom"),
                Categorie = (CategoriePlat)reader.GetInt32("Categorie"),
                CuisinierID = reader.GetInt32("CuisinierID"),
                NationaliteCuisine = reader.GetString("NationaliteCuisine"),
                NombrePersonnes = reader.GetInt32("NombrePersonnes"),
                DatePreparation = reader.GetDateTime("DatePreparation"),
                DateExpiration = reader.GetDateTime("DateExpiration"),
                PrixParPersonne = reader.GetDecimal("PrixParPersonne"),
                PhotoUrl = reader.IsDBNull(reader.GetOrdinal("PhotoUrl")) ? null : reader.GetString("PhotoUrl"),
                EstDisponible = reader.GetBoolean("EstDisponible"),
                Cuisinier = new Cuisinier
                {
                    Nom = reader.GetString("CuisinierNom"),
                    Prenom = reader.GetString("CuisinierPrenom")
                }
            };
        }

        #endregion

        #region Opérations sur les Commandes

        // Créer une nouvelle commande
        public int CreerCommande(Commande commande)
        {
            using (var connection = GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // 1. Insérer la commande
                    string queryCommande = @"
                        INSERT INTO Commandes (ClientID, DateCommande, DateLivraisonPrevue, 
                                             PrixTotal, Statut, AdresseLivraison, Commentaires)
                        VALUES (@ClientID, @DateCommande, @DateLivraisonPrevue, 
                               @PrixTotal, @Statut, @AdresseLivraison, @Commentaires);
                        SELECT LAST_INSERT_ID();";

                    int commandeID;
                    using (var cmd = new MySqlCommand(queryCommande, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@ClientID", commande.ClientID);
                        cmd.Parameters.AddWithValue("@DateCommande", commande.DateCommande);
                        cmd.Parameters.AddWithValue("@DateLivraisonPrevue", commande.DateLivraisonPrevue);
                        cmd.Parameters.AddWithValue("@PrixTotal", commande.PrixTotal);
                        cmd.Parameters.AddWithValue("@Statut", (int)commande.Statut);
                        cmd.Parameters.AddWithValue("@AdresseLivraison", commande.AdresseLivraison);
                        cmd.Parameters.AddWithValue("@Commentaires", commande.Commentaires ?? (object)DBNull.Value);

                        commandeID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 2. Insérer les items de la commande
                    foreach (var item in commande.Items)
                    {
                        string queryItem = @"
                            INSERT INTO CommandeItems (CommandeID, PlatID, Quantite, PrixUnitaire)
                            VALUES (@CommandeID, @PlatID, @Quantite, @PrixUnitaire)";

                        using (var cmd = new MySqlCommand(queryItem, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@CommandeID", commandeID);
                            cmd.Parameters.AddWithValue("@PlatID", item.PlatID);
                            cmd.Parameters.AddWithValue("@Quantite", item.Quantite);
                            cmd.Parameters.AddWithValue("@PrixUnitaire", item.PrixUnitaire);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // 3. Mettre à jour l'historique
                    string queryHistorique = @"
                        INSERT INTO HistoriqueCommandes (CommandeID, ActionDate, Statut, Commentaire)
                        VALUES (@CommandeID, @ActionDate, @Statut, @Commentaire)";

                    using (var cmd = new MySqlCommand(queryHistorique, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@CommandeID", commandeID);
                        cmd.Parameters.AddWithValue("@ActionDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Statut", (int)commande.Statut);
                        cmd.Parameters.AddWithValue("@Commentaire", "Commande créée");
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return commandeID;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // Valider une commande et créer une livraison
        public void ValiderCommandeEtCreerLivraison(int commandeID, Livraison livraison)
        {
            using (var connection = GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // 1. Mettre à jour le statut de la commande
                    string queryCommande = @"
                        UPDATE Commandes 
                        SET Statut = @Statut
                        WHERE Id = @CommandeID";

                    using (var cmd = new MySqlCommand(queryCommande, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@CommandeID", commandeID);
                        cmd.Parameters.AddWithValue("@Statut", (int)StatutCommande.Confirmee);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Insérer la livraison
                    string queryLivraison = @"
                        INSERT INTO Livraisons (CommandeID, DateDepart, ItineraireSuggere, Statut)
                        VALUES (@CommandeID, @DateDepart, @ItineraireSuggere, @Statut);
                        SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySqlCommand(queryLivraison, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@CommandeID", commandeID);
                        cmd.Parameters.AddWithValue("@DateDepart", livraison.DateDepart);
                        cmd.Parameters.AddWithValue("@ItineraireSuggere", livraison.ItineraireSuggere ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Statut", (int)StatutLivraison.Planifiee);
                        int livraisonID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 3. Mettre à jour l'historique
                    string queryHistorique = @"
                        INSERT INTO HistoriqueCommandes (CommandeID, ActionDate, Statut, Commentaire)
                        VALUES (@CommandeID, @ActionDate, @Statut, @Commentaire)";

                    using (var cmd = new MySqlCommand(queryHistorique, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@CommandeID", commandeID);
                        cmd.Parameters.AddWithValue("@ActionDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Statut", (int)StatutCommande.Confirmee);
                        cmd.Parameters.AddWithValue("@Commentaire", "Commande validée et livraison planifiée");
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

        #endregion

        #region Notations

        // Ajouter une notation (cuisinier ou client)
        public void AjouterNotation(Notation notation)
        {
            using (var connection = GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // 1. Insérer la notation
                    string queryNotation = @"
                        INSERT INTO Notations (CommandeID, NoteurID, NoteID, Type, Note, Commentaire, DateNotation)
                        VALUES (@CommandeID, @NoteurID, @NoteID, @Type, @Note, @Commentaire, @DateNotation);";

                    using (var cmd = new MySqlCommand(queryNotation, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@CommandeID", notation.CommandeID);
                        cmd.Parameters.AddWithValue("@NoteurID", notation.NoteurID);
                        cmd.Parameters.AddWithValue("@NoteID", notation.NoteID);
                        cmd.Parameters.AddWithValue("@Type", (int)notation.Type);
                        cmd.Parameters.AddWithValue("@Note", notation.Note);
                        cmd.Parameters.AddWithValue("@Commentaire", notation.Commentaire ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DateNotation", notation.DateNotation);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Mettre à jour la note moyenne de la personne notée
                    if (notation.Type == TypeNotation.ClientVersCuisinier)
                    {
                        // Mettre à jour la note moyenne du cuisinier
                        using (var cmdProc = new MySqlCommand("MettreAJourNoteMoyenneCuisinier", connection, transaction))
                        {
                            cmdProc.CommandType = CommandType.StoredProcedure;
                            cmdProc.Parameters.AddWithValue("@CuisinierID", notation.NoteID);
                            cmdProc.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Mettre à jour la note moyenne du client
                        using (var cmdProc = new MySqlCommand("MettreAJourNoteMoyenneClient", connection, transaction))
                        {
                            cmdProc.CommandType = CommandType.StoredProcedure;
                            cmdProc.Parameters.AddWithValue("@ClientID", notation.NoteID);
                            cmdProc.ExecuteNonQuery();
                        }
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

        #endregion

        #region Statistiques

        // Obtenir les livraisons par cuisinier
        public DataTable ObtenirLivraisonsParCuisinier()
        {
            using (var connection = GetConnection())
            {
                using (var cmdProc = new MySqlCommand("ObtenirLivraisonsParCuisinier", connection))
                {
                    cmdProc.CommandType = CommandType.StoredProcedure;

                    var dataTable = new DataTable();
                    using (var adapter = new MySqlDataAdapter(cmdProc))
                    {
                        adapter.Fill(dataTable);
                    }
                    return dataTable;
                }
            }
        }

        // Obtenir les commandes par période
        public DataTable ObtenirCommandesParPeriode(DateTime debut, DateTime fin)
        {
            using (var connection = GetConnection())
            {
                using (var cmdProc = new MySqlCommand("ObtenirCommandesParPeriode", connection))
                {
                    cmdProc.CommandType = CommandType.StoredProcedure;
                    cmdProc.Parameters.AddWithValue("@DateDebut", debut);
                    cmdProc.Parameters.AddWithValue("@DateFin", fin);

                    var dataTable = new DataTable();
                    using (var adapter = new MySqlDataAdapter(cmdProc))
                    {
                        adapter.Fill(dataTable);
                    }
                    return dataTable;
                }
            }
        }

        // Obtenir la liste des commandes d'un client
        public DataTable ObtenirCommandesClient(int clientID)
        {
            using (var connection = GetConnection())
            {
                using (var cmdProc = new MySqlCommand("ObtenirCommandesClient", connection))
                {
                    cmdProc.CommandType = CommandType.StoredProcedure;
                    cmdProc.Parameters.AddWithValue("@ClientID", clientID);

                    var dataTable = new DataTable();
                    using (var adapter = new MySqlDataAdapter(cmdProc))
                    {
                        adapter.Fill(dataTable);
                    }
                    return dataTable;
                }
            }
        }

        #endregion
    }
}