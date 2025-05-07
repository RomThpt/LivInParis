USE livinparis;

-- Rename columns in Cuisiniers table
ALTER TABLE Cuisiniers 
CHANGE COLUMN Nom LastName VARCHAR(100) NOT NULL,
CHANGE COLUMN Prenom FirstName VARCHAR(100) NOT NULL,
CHANGE COLUMN Adresse Address TEXT NOT NULL,
CHANGE COLUMN NoteMoyenne Rating FLOAT DEFAULT 0,
CHANGE COLUMN DateInscription RegistrationDate DATETIME NOT NULL;

-- Rename columns in Clients table
ALTER TABLE Clients 
CHANGE COLUMN Nom LastName VARCHAR(100) NOT NULL,
CHANGE COLUMN Prenom FirstName VARCHAR(100) NOT NULL,
CHANGE COLUMN Telephone Phone VARCHAR(20) NOT NULL,
CHANGE COLUMN Adresse Address TEXT NOT NULL,
CHANGE COLUMN Type ClientType TINYINT NOT NULL,
CHANGE COLUMN NoteMoyenne Rating FLOAT DEFAULT 0,
CHANGE COLUMN DateInscription RegistrationDate DATETIME NOT NULL;

-- Rename columns in Plats table
ALTER TABLE Plats 
CHANGE COLUMN Nom Name VARCHAR(150) NOT NULL,
CHANGE COLUMN Categorie Category TINYINT NOT NULL,
CHANGE COLUMN CuisinierID CookId INT NOT NULL,
CHANGE COLUMN NationaliteCuisine Nationality VARCHAR(100) NOT NULL,
CHANGE COLUMN NombrePersonnes PortionCount INT NOT NULL,
CHANGE COLUMN DatePreparation PreparationDate DATETIME NOT NULL,
CHANGE COLUMN DateExpiration ExpirationDate DATETIME NOT NULL,
CHANGE COLUMN PrixParPersonne PricePerPerson DECIMAL(10,2) NOT NULL,
CHANGE COLUMN PhotoUrl PhotoPath VARCHAR(255),
CHANGE COLUMN EstDisponible IsAvailable BOOLEAN DEFAULT TRUE;

-- Rename columns in IngredientsPlat table
ALTER TABLE IngredientsPlat 
CHANGE COLUMN PlatID MealId INT NOT NULL,
CHANGE COLUMN NomIngredient IngredientName VARCHAR(100) NOT NULL;

-- Rename columns in RegimesAlimentairesPlat table
ALTER TABLE RegimesAlimentairesPlat 
CHANGE COLUMN PlatID MealId INT NOT NULL,
CHANGE COLUMN TypeRegime DietType VARCHAR(100) NOT NULL;

-- Rename columns in Commandes table
ALTER TABLE Commandes 
CHANGE COLUMN ClientID ClientId INT NOT NULL,
CHANGE COLUMN DateCommande OrderDate DATETIME NOT NULL,
CHANGE COLUMN DateLivraisonPrevue DeliveryDate DATETIME NOT NULL,
CHANGE COLUMN PrixTotal TotalPrice DECIMAL(10,2) NOT NULL,
CHANGE COLUMN Statut Status TINYINT NOT NULL,
CHANGE COLUMN AdresseLivraison DeliveryAddress TEXT NOT NULL,
CHANGE COLUMN Commentaires Comments TEXT;

-- Rename columns in CommandeItems table
ALTER TABLE CommandeItems 
CHANGE COLUMN CommandeID OrderId INT NOT NULL,
CHANGE COLUMN PlatID MealId INT NOT NULL,
CHANGE COLUMN Quantite Quantity INT NOT NULL,
CHANGE COLUMN PrixUnitaire UnitPrice DECIMAL(10,2) NOT NULL;

-- Rename columns in Livraisons table
ALTER TABLE Livraisons 
CHANGE COLUMN CommandeID OrderId INT NOT NULL,
CHANGE COLUMN DateDepart DepartureDate DATETIME NOT NULL,
CHANGE COLUMN DateArrivee ArrivalDate DATETIME,
CHANGE COLUMN ItineraireSuggere SuggestedRoute TEXT,
CHANGE COLUMN Statut Status TINYINT NOT NULL,
CHANGE COLUMN CommentaireLivreur DeliveryComment TEXT;

-- Rename columns in Notations table
ALTER TABLE Notations 
CHANGE COLUMN CommandeID OrderId INT NOT NULL,
CHANGE COLUMN NoteurID RaterId INT NOT NULL,
CHANGE COLUMN NoteID RatedId INT NOT NULL,
CHANGE COLUMN Type RatingType TINYINT NOT NULL,
CHANGE COLUMN Note Rating TINYINT NOT NULL,
CHANGE COLUMN Commentaire Comment TEXT,
CHANGE COLUMN DateNotation RatingDate DATETIME NOT NULL;

-- Rename columns in HistoriqueCommandes table
ALTER TABLE HistoriqueCommandes 
CHANGE COLUMN CommandeID OrderId INT NOT NULL,
CHANGE COLUMN ActionDate ActionDate DATETIME NOT NULL,
CHANGE COLUMN Statut Status TINYINT NOT NULL,
CHANGE COLUMN Commentaire Comment TEXT;

DROP PROCEDURE IF EXISTS ObtenirCommandesParPeriode;

DELIMITER //

CREATE PROCEDURE ObtenirCommandesParPeriode(IN dateDebut DATETIME, IN dateFin DATETIME)
BEGIN
    SELECT 
        cmd.Id AS CommandeID,
        cmd.DateCommande,
        CONCAT(cl.FirstName, ' ', cl.LastName) AS NomClient,
        cmd.PrixTotal,
        cmd.Statut
    FROM Commandes cmd
    JOIN Clients cl ON cmd.ClientID = cl.Id
    WHERE cmd.DateCommande BETWEEN dateDebut AND dateFin
    ORDER BY cmd.DateCommande;
END //

DELIMITER ;

DROP PROCEDURE IF EXISTS ObtenirCommandesClient;

DELIMITER //

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
END //

DELIMITER ;

DROP PROCEDURE IF EXISTS MettreAJourNoteMoyenneClient;

DELIMITER //

CREATE PROCEDURE MettreAJourNoteMoyenneClient(IN clientID INT)
BEGIN
    UPDATE Clients 
    SET NoteMoyenne = (
        SELECT AVG(Note) 
        FROM Notations 
        WHERE Type = 1 AND NoteID = clientID
    )
    WHERE Id = clientID;
END //

DELIMITER ; 