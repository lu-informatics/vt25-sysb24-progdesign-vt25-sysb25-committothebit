CREATE TABLE Recipe (
   recipeId INT IDENTITY(1,1) PRIMARY KEY,
   name VARCHAR(255) NOT NULL,
   cookingTime INT CHECK (cookingTime > 0),
   servings INT CHECK (servings > 0),
   difficultyLevel VARCHAR(50) CHECK (difficultyLevel IN ('Easy', 'Medium', 'Hard')),
   data NVARCHAR(MAX) NULL CHECK (ISJSON(data) > 0) -- JSON field with validation
);

CREATE TABLE Ingredient (
   ingredientId INT IDENTITY(1,1) PRIMARY KEY,
   name VARCHAR(255) NOT NULL,
   Category VARCHAR(100),
   Unit VARCHAR(50),
   DietTag VARCHAR(255)
);

CREATE TABLE AppUser
(
AppUserID INT IDENTITY(1,1),
Username NVARCHAR(50) NOT NULL,
PasswordHash VARBINARY(64) NOT NULL,
Salt NVARCHAR(50) NOT NULL,


CONSTRAINT PK_AppUser_UserID PRIMARY KEY (AppUserID),
CONSTRAINT UQ_AppUser_Username UNIQUE (Username),
-- Password complexity requirements


CONSTRAINT CK_AppUser_PasswordComplexity CHECK (
LEN(PasswordHash) > 0 AND LEN(Salt) > 0
)
);

CREATE TABLE RecipeIngredient (
   recipeId INT,
   ingredientId INT,
   amount DECIMAL(10,2) NOT NULL,
   PRIMARY KEY (recipeId, ingredientId),
   FOREIGN KEY (recipeId) REFERENCES Recipe(recipeId) ON DELETE CASCADE,
   FOREIGN KEY (ingredientId) REFERENCES Ingredient(ingredientId) ON DELETE CASCADE
);

CREATE TABLE SavedRecipe (
   recipeId INT,
   AppUserId INT,
   rating INT,          -- Assuming rating is an integer; adjust if it's a different type
   dateAdded DATE,      -- Assuming you want just the date; use DATETIME or TIMESTAMP if you need time as well
   PRIMARY KEY (recipeId, AppUserId),  -- Composite primary key to ensure unique pairs of recipes and users
   FOREIGN KEY (recipeId) REFERENCES Recipe(recipeId),
   FOREIGN KEY (AppuserId) REFERENCES AppUser(AppUserId)
);


CREATE TABLE ShoppingList (
   AppUserId INT,
   ingredientId INT,
   Amount DECIMAL(10,2),  -- This could be in grams, liters, etc., depending on the unit
   PRIMARY KEY (AppUserId, ingredientId),  -- Ensures each user-ingredient combination is unique
   FOREIGN KEY (AppUserId) REFERENCES AppUser(AppUserId),
   FOREIGN KEY (ingredientId) REFERENCES Ingredient(ingredientId)
);


CREATE TABLE UserIngredient (
   AppUserId INT,
   ingredientId INT,
   Amount DECIMAL(10,2),  -- This should match the unit type specified in the Ingredient table
   PRIMARY KEY (AppUserId, ingredientId),  -- Ensures each user-ingredient combination is unique
   FOREIGN KEY (AppUserId) REFERENCES AppUser(AppUserId),
   FOREIGN KEY (ingredientId) REFERENCES Ingredient(ingredientId)
);
