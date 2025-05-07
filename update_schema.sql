USE livinparis;

RENAME TABLE Cuisiniers TO Cooks;
RENAME TABLE Plats TO Meals;
RENAME TABLE Commandes TO Orders;
RENAME TABLE CommandeItems TO OrderItems;
RENAME TABLE Livraisons TO Deliveries;
RENAME TABLE Notations TO Ratings;
RENAME TABLE HistoriqueCommandes TO OrderHistory;
RENAME TABLE IngredientsPlat TO MealIngredients;
RENAME TABLE RegimesAlimentairesPlat TO MealDiets; 