-- 1. Tables Principales
USE LivInParis;

-- Plats
INSERT INTO Plats(PlatID____________, NomPlat___________, TypePlat__________, NombrePersonnes___, 
    DateFabrication___, DatePeremption____, PrixParPersonne___, NationaliteCuisine, 
    RegimeAlimentaire_, Ingredients_______, Photo_____________, RecetteID)
VALUES
(1, 'Couscous', 'Plat', 4, '2025-01-10', '2025-01-15', 12.00, 'Marocaine', '',
 'semoule(200g), légumes(300g), viande(200g)', NULL, NULL);

-- Cuisiniers
INSERT INTO Cuisiniers(CuisinierID_, TypeCuisinier, NomCuisinier, PrenomCuisinier, AdresseCuisinier, 
    TelephoneCuisinier, CodePostalCuisinier, EmailCuisinier, PlatID____________)
VALUES(1, 'Pro', 'Dupond', 'Marie', '30 Rue de la République', '1234567890', '75011', 'Mdupond@gmail.com', 1);

-- Clients
INSERT INTO Clients(ClientID, TypeClient, NomClient, PrenomClient, AdresseClient, TelephoneClient, 
    CodePostalClient, EmailClient, CuisinierID_)
VALUES(1, TRUE, 'Durand', 'Medhy', '15 Rue Cardinet, 75017 Paris', 1234567890, 75017, 'Mdurand@gmail.com', 1);

-- Plats
INSERT INTO Plats(PlatID____________, NomPlat___________, TypePlat__________, NombrePersonnes___, 
    DateFabrication___, DatePeremption____, PrixParPersonne___, NationaliteCuisine, 
    RegimeAlimentaire_, Ingredients_______, Photo_____________, RecetteID)
VALUES
(3, 'Raclette', 'Plat', 6, '2025-01-10', '2025-01-15', 10.00, 'Française', '',
 'raclette fromage(250g), pommes_de_terre(200g), jambon(200g), cornichon(3p)', NULL, NULL),
(4, 'Salade de fruit', 'Dessert', 6, '2025-01-10', '2025-01-15', 5.00, 'Indifférent', 'Végétarien',
 'fraise(100g), kiwi(100g), sucre(10g)', NULL, NULL);

-- Commandes
INSERT INTO Commandes(CommandesID, DateTransaction_, TotalPrix_______, StatutCommande)
VALUES
(1, '2025-01-10', 60.00, 'Validée'),
(2, '2025-01-10', 30.00, 'En cours');

-- LignesCommande
INSERT INTO LignesCommande(LigneCommandeID_, DateLivraison___, Quantité, PrixUnitaire, 
    LieuLivraison___, PlatID____________)
VALUES
(1, '2025-01-10', 6, 10, '15 Rue Cardinet, 75017 Paris', 1),
(2, '2025-01-10', 6, 5, '15 Rue Cardinet, 75017 Paris', 3);

-- 2. Association Tables

-- passe (Client-Commande associations)
INSERT INTO passe(ClientID, CommandesID)
VALUES
(1, 1),
(1, 2);

-- se_décompose_en_ (Commande-LigneCommande associations)
INSERT INTO se_décompose_en_(CommandesID, LigneCommandeID_)
VALUES
(1, 1),
(2, 2);


-- 3. Test des donées

SELECT * FROM Cuisiniers;
SELECT * FROM Plats;
SELECT * FROM Clients;
SELECT * FROM Commandes;
SELECT * FROM LignesCommande;

-- Renvoie les commandes des clients
SELECT c.NomClient, c.PrenomClient, cmd.CommandesID, cmd.TotalPrix_______, cmd.StatutCommande
FROM Clients c
JOIN passe p ON c.ClientID = p.ClientID
JOIN Commandes cmd ON cmd.CommandesID = p.CommandesID;

-- Renvoie les plats commandés
SELECT c.CommandesID, lc.LigneCommandeID_, lc.Quantité, lc.PrixUnitaire, p.NomPlat___________
FROM se_décompose_en_ s
JOIN Commandes c ON c.CommandesID = s.CommandesID
JOIN LignesCommande lc ON lc.LigneCommandeID_ = s.LigneCommandeID_
JOIN Plats p ON p.PlatID____________ = lc.PlatID____________;

-- Renvoie les cuisiniers et les plats qu'ils préparent
SELECT c.NomCuisinier, c.PrenomCuisinier, p.NomPlat___________, p.TypePlat__________
FROM Cuisiniers c
JOIN Plats p ON c.PlatID____________ = p.PlatID____________;

-- Renvoie les clients et les commandes qu'ils ont passées
SELECT c.NomClient, cmd.CommandesID, lc.DateLivraison___, lc.LieuLivraison___
FROM Clients c
JOIN passe p ON c.ClientID = p.ClientID
JOIN Commandes cmd ON cmd.CommandesID = p.CommandesID
JOIN se_décompose_en_ s ON cmd.CommandesID = s.CommandesID
JOIN LignesCommande lc ON lc.LigneCommandeID_ = s.LigneCommandeID_;

