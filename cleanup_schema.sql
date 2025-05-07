USE livinparis;

-- Drop foreign key constraints first
SET FOREIGN_KEY_CHECKS=0;

-- Drop duplicate tables
DROP TABLE IF EXISTS Cuisiniers;
DROP TABLE IF EXISTS Cuisinier;
DROP TABLE IF EXISTS Plats;
DROP TABLE IF EXISTS Plat;
DROP TABLE IF EXISTS Commandes;
DROP TABLE IF EXISTS Commande;
DROP TABLE IF EXISTS Commande_Plat;
DROP TABLE IF EXISTS Livraisons;
DROP TABLE IF EXISTS Livraison;
DROP TABLE IF EXISTS Notations;
DROP TABLE IF EXISTS Notation;
DROP TABLE IF EXISTS IngredientsPlat;
DROP TABLE IF EXISTS Ingredient;
DROP TABLE IF EXISTS Plat_Ingredient;
DROP TABLE IF EXISTS RegimesAlimentairesPlat;
DROP TABLE IF EXISTS Regime_Alimentaire;
DROP TABLE IF EXISTS Plat_Regime;
DROP TABLE IF EXISTS Categorie_Plat;
DROP TABLE IF EXISTS Type_Cuisine;
DROP TABLE IF EXISTS Statut;
DROP TABLE IF EXISTS Stations;
DROP TABLE IF EXISTS Utilisateur;
DROP TABLE IF EXISTS Client;
DROP TABLE IF EXISTS v_classementcuisiniers;
DROP TABLE IF EXISTS v_recommandationsplats;
DROP TABLE IF EXISTS v_statistiquescommandes;

SET FOREIGN_KEY_CHECKS=1; 