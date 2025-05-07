USE livinparis;

-- Disable foreign key checks to avoid dependency issues
SET FOREIGN_KEY_CHECKS=0;

-- Drop all views
DROP VIEW IF EXISTS v_classementcuisiniers;
DROP VIEW IF EXISTS v_recommandationsplats;
DROP VIEW IF EXISTS v_statistiquescommandes;

-- Drop all tables
DROP TABLE IF EXISTS Clients;
DROP TABLE IF EXISTS CommandeItems;
DROP TABLE IF EXISTS Connections;
DROP TABLE IF EXISTS Cooks;
DROP TABLE IF EXISTS HistoriqueCommandes;
DROP TABLE IF EXISTS Meals;
DROP TABLE IF EXISTS MetroConnexions;
DROP TABLE IF EXISTS MetroStations;
DROP TABLE IF EXISTS OrderItems;
DROP TABLE IF EXISTS Orders;
DROP TABLE IF EXISTS PlanningCuisiniers;

-- Re-enable foreign key checks
SET FOREIGN_KEY_CHECKS=1; 