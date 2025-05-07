USE livinparis;

-- Create Clients table
CREATE TABLE IF NOT EXISTS Clients (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    LastName VARCHAR(50) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    Address VARCHAR(200) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    TypeClient ENUM('individual', 'local business') NOT NULL,
    RegistrationDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Create Cooks table
CREATE TABLE IF NOT EXISTS Cooks (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    LastName VARCHAR(50) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    Address VARCHAR(200) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Rating DECIMAL(3,2) DEFAULT 0.0,
    RegistrationDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Create Meals table
CREATE TABLE IF NOT EXISTS Meals (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CookId INT NOT NULL,
    Name VARCHAR(100) NOT NULL,
    Category ENUM('starter', 'main course', 'dessert') NOT NULL,
    PortionCount INT NOT NULL,
    PreparationDate DATETIME NOT NULL,
    ExpirationDate DATETIME NOT NULL,
    PricePerPerson DECIMAL(10,2) NOT NULL,
    Nationality VARCHAR(50) NOT NULL,
    DietaryRequirements VARCHAR(200),
    MainIngredients VARCHAR(500) NOT NULL,
    PhotoPath VARCHAR(255),
    FOREIGN KEY (CookId) REFERENCES Cooks(Id)
);

-- Create Orders table
CREATE TABLE IF NOT EXISTS Orders (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClientId INT NOT NULL,
    OrderDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    TotalPrice DECIMAL(10,2) NOT NULL,
    DeliveryAddress VARCHAR(200) NOT NULL,
    Status ENUM('pending', 'confirmed', 'delivered', 'cancelled') DEFAULT 'pending',
    FOREIGN KEY (ClientId) REFERENCES Clients(Id)
);

-- Create OrderItems table
CREATE TABLE IF NOT EXISTS OrderItems (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    OrderId INT NOT NULL,
    MealId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (MealId) REFERENCES Meals(Id)
);

-- Create Ratings table
CREATE TABLE IF NOT EXISTS Ratings (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    OrderId INT NOT NULL,
    RaterId INT NOT NULL,
    RatedId INT NOT NULL,
    RatingType ENUM('client', 'cook') NOT NULL,
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment TEXT,
    RatingDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id)
); 