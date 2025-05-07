-- Update queries in Program.cs
UPDATE Program.cs
SET query = "SELECT Id, LastName, FirstName FROM Cooks ORDER BY LastName, FirstName"
WHERE query = "SELECT Id, Nom, Prenom FROM Cuisiniers ORDER BY Nom, Prenom";

UPDATE Program.cs
SET query = "SELECT Id, LastName, FirstName, Address FROM Clients ORDER BY LastName"
WHERE query = "SELECT Id, Nom, Prenom, Adresse FROM Clients ORDER BY Nom";

UPDATE Program.cs
SET query = "SELECT Id, Name, PricePerPerson FROM Meals WHERE IsAvailable = 1"
WHERE query = "SELECT Id, Nom, PrixParPersonne FROM Plats WHERE EstDisponible = 1";

UPDATE Program.cs
SET query = "SELECT Id, LastName, FirstName FROM Clients ORDER BY LastName"
WHERE query = "SELECT Id, Nom, Prenom FROM Clients ORDER BY Nom"; 