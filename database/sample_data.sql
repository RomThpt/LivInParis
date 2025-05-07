USE livinparis;

-- Insert sample cooks
INSERT INTO Cooks (LastName, FirstName, Address, Phone, Email, Rating) VALUES
('Dupont', 'Jean', '1 Rue de la Paix, 75001 Paris', '+33612345678', 'jean.dupont@email.com', 4.5),
('Martin', 'Sophie', '2 Avenue des Champs-Élysées, 75008 Paris', '+33623456789', 'sophie.martin@email.com', 4.8),
('Bernard', 'Pierre', '3 Rue de Rivoli, 75004 Paris', '+33634567890', 'pierre.bernard@email.com', 4.2);

-- Insert sample meals
INSERT INTO Meals (CookId, Name, Category, PortionCount, PreparationDate, ExpirationDate, PricePerPerson, Nationality, DietaryRequirements, MainIngredients) VALUES
(1, 'Soupe a l''oignon', 'starter', 4, NOW(), DATE_ADD(NOW(), INTERVAL 2 DAY), 8.50, 'French', 'Vegetarian', 'Onions, Beef broth, Cheese, Bread'),
(1, 'Coq au vin', 'main course', 4, NOW(), DATE_ADD(NOW(), INTERVAL 2 DAY), 15.00, 'French', '', 'Chicken, Wine, Bacon, Mushrooms'),
(2, 'Ratatouille', 'main course', 4, NOW(), DATE_ADD(NOW(), INTERVAL 2 DAY), 12.00, 'French', 'Vegetarian', 'Eggplant, Zucchini, Tomatoes, Bell peppers'),
(2, 'Creme brulee', 'dessert', 4, NOW(), DATE_ADD(NOW(), INTERVAL 2 DAY), 7.00, 'French', 'Vegetarian', 'Cream, Vanilla, Sugar, Eggs'),
(3, 'Salade nicoise', 'starter', 4, NOW(), DATE_ADD(NOW(), INTERVAL 2 DAY), 9.00, 'French', '', 'Tuna, Eggs, Tomatoes, Olives'); 