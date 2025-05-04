USE livinparis;

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