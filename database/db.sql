-- Active: 1740651377371@@127.0.0.1@3306@livin

CREATE DATABASE IF NOT EXISTS LivInParis;
USE LivInParis;
CREATE TABLE Recettes(
   RecetteID INT,
   NomRecette___ VARCHAR(100) NOT NULL,
   Description__ VARCHAR(200),
   PRIMARY KEY(RecetteID)
);

CREATE TABLE Plats(
   PlatID____________ INT,
   NomPlat___________ VARCHAR(100) NOT NULL,
   TypePlat__________ VARCHAR(20) NOT NULL,
   NombrePersonnes___ INT NOT NULL,
   DateFabrication___ DATE NOT NULL,
   DatePeremption____ DATE NOT NULL,
   PrixParPersonne___ DECIMAL(10,2) NOT NULL,
   NationaliteCuisine VARCHAR(50) NOT NULL,
   RegimeAlimentaire_ VARCHAR(50),
   Ingredients_______ VARCHAR(500) NOT NULL,
   Photo_____________ VARCHAR(500),
   RecetteID INT,
   PRIMARY KEY(PlatID____________),
   FOREIGN KEY(RecetteID) REFERENCES Recettes(RecetteID)
);

CREATE TABLE Commandes(
   CommandesID INT,
   DateTransaction_ DATE NOT NULL,
   TotalPrix_______ DECIMAL(10,2),
   StatutCommande VARCHAR(50) NOT NULL,
   PRIMARY KEY(CommandesID)
);

CREATE TABLE LignesCommande(
   LigneCommandeID_ INT,
   DateLivraison___ DATE NOT NULL,
   Quantité INT,
   PrixUnitaire INT,
   LieuLivraison___ VARCHAR(200) NOT NULL,
   PlatID____________ INT NOT NULL,
   PRIMARY KEY(LigneCommandeID_),
   FOREIGN KEY(PlatID____________) REFERENCES Plats(PlatID____________)
);

CREATE TABLE Trajets(
   TrajetID________ INT,
   StationDepart VARCHAR(200) NOT NULL,
   StationArrivée VARCHAR(200) NOT NULL,
   Durée INT,
   Chemin__________ VARCHAR(500),
   Distance________ DECIMAL(10,2),
   PRIMARY KEY(TrajetID________)
);

CREATE TABLE Cuisiniers(
   CuisinierID_ INT,
   TypeCuisinier VARCHAR(50) NOT NULL,
   NomCuisinier VARCHAR(50),
   PrenomCuisinier VARCHAR(50),
   AdresseCuisinier VARCHAR(50),
   TelephoneCuisinier VARCHAR(50),
   CodePostalCuisinier VARCHAR(50),
   EmailCuisinier VARCHAR(50),
   PlatID____________ INT NOT NULL,
   PRIMARY KEY(CuisinierID_),
   FOREIGN KEY(PlatID____________) REFERENCES Plats(PlatID____________)
);

CREATE TABLE Avis(
   AvisID_______ INT,
   Commentaire__ VARCHAR(500),
   Note_________ INT,
   DateAvis_____ DATE NOT NULL,
   CuisinierID_ INT NOT NULL,
   PRIMARY KEY(AvisID_______),
   FOREIGN KEY(CuisinierID_) REFERENCES Cuisiniers(CuisinierID_)
);

CREATE TABLE Livraison(
   LivraisonID INT,
   AdresseArrivée VARCHAR(500) NOT NULL,
   StatutLivraison VARCHAR(50) NOT NULL,
   DatePrévue DATE NOT NULL,
   AdresseDépart VARCHAR(500) NOT NULL,
   CuisinierID_ INT NOT NULL,
   CommandesID INT NOT NULL,
   PRIMARY KEY(LivraisonID),
   FOREIGN KEY(CuisinierID_) REFERENCES Cuisiniers(CuisinierID_),
   FOREIGN KEY(CommandesID) REFERENCES Commandes(CommandesID)
);

CREATE TABLE Clients(
   ClientID INT,
   TypeClient BOOLEAN NOT NULL,
   NomClient VARCHAR(50) NOT NULL,
   PrenomClient VARCHAR(50) NOT NULL,
   AdresseClient VARCHAR(200) NOT NULL,
   TelephoneClient INT NOT NULL,
   CodePostalClient INT NOT NULL,
   EmailClient VARCHAR(100) NOT NULL,
   CuisinierID_ INT NOT NULL,
   PRIMARY KEY(ClientID),
   FOREIGN KEY(CuisinierID_) REFERENCES Cuisiniers(CuisinierID_)
);

CREATE TABLE passe(
   ClientID INT,
   CommandesID INT,
   PRIMARY KEY(ClientID, CommandesID),
   FOREIGN KEY(ClientID) REFERENCES Clients(ClientID),
   FOREIGN KEY(CommandesID) REFERENCES Commandes(CommandesID)
);

CREATE TABLE se_décompose_en_(
   CommandesID INT,
   LigneCommandeID_ INT,
   PRIMARY KEY(CommandesID, LigneCommandeID_),
   FOREIGN KEY(CommandesID) REFERENCES Commandes(CommandesID),
   FOREIGN KEY(LigneCommandeID_) REFERENCES LignesCommande(LigneCommandeID_)
);

CREATE TABLE donne(
   ClientID INT,
   AvisID_______ INT,
   PRIMARY KEY(ClientID, AvisID_______),
   FOREIGN KEY(ClientID) REFERENCES Clients(ClientID),
   FOREIGN KEY(AvisID_______) REFERENCES Avis(AvisID_______)
);

CREATE TABLE par(
   TrajetID________ INT,
   LivraisonID INT,
   PRIMARY KEY(TrajetID________, LivraisonID),
   FOREIGN KEY(TrajetID________) REFERENCES Trajets(TrajetID________),
   FOREIGN KEY(LivraisonID) REFERENCES Livraison(LivraisonID)
);