USE [TeamA_Project]

-- 01General_01_CreateLanguages

CREATE TABLE Languages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(10) NOT NULL UNIQUE
);

-- 01General_02_CreateDistricts_WithAllParentTable

CREATE TABLE Counties (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(10) NOT NULL UNIQUE
);

CREATE TABLE Districts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CountyId INT NOT NULL,
    Name NVARCHAR(10) NOT NULL,
    CONSTRAINT FK_Districts_Counties FOREIGN KEY (CountyId) REFERENCES Counties(Id),
    CONSTRAINT UQ_Districts_CountyId_Name UNIQUE (CountyId, Name)
);

--01General_03_CreateContentRatings

CREATE TABLE ContentRatings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(10) NOT NULL UNIQUE
);

-- 02Primary_02_UsedBook_CreateUsedBooks_WithAllParentTable

 CREATE TABLE BookBindings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(10) NOT NULL UNIQUE
);

CREATE TABLE BookConditionRatings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(10) NOT NULL UNIQUE,
    Description NVARCHAR(100) NOT NULL
);


CREATE TABLE UsedBooks (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    SellerId UNIQUEIDENTIFIER NOT NULL,     -- FK to Users
    SellerDistrictId INT NOT NULL,          -- FK to Districts
    SalePrice DECIMAL(10, 2) NOT NULL,

    Title NVARCHAR(100) NOT NULL,
    Authors NVARCHAR(200) NOT NULL,

    ConditionRatingId INT NOT NULL,         -- FK to BookConditionRatings
    ConditionDescription NVARCHAR(200) NULL,

    Edition NVARCHAR(20) NULL,
    Publisher NVARCHAR(100) NULL,
    PublicationDate DATE NULL,

    Isbn VARCHAR(13) NULL,
    BindingId INT NULL,                     -- FK to BookBindings

    LanguageId INT NULL,                    -- FK to Languages
    Pages INT NULL,

    ContentRatingId INT NOT NULL,           -- FK to ContentRatings

    IsOnShelf BIT NOT NULL DEFAULT(0),
    IsSold BIT NOT NULL DEFAULT(0),
    IsActive BIT NOT NULL DEFAULT(1),

    Slug NVARCHAR(255) NOT NULL UNIQUE,

    CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
    UpdatedAt DATETIME2 NOT NULL,

    FOREIGN KEY (SellerId) REFERENCES Users(UID),
    FOREIGN KEY (SellerDistrictId) REFERENCES Districts(Id),
    FOREIGN KEY (ConditionRatingId) REFERENCES BookConditionRatings(Id),
    FOREIGN KEY (BindingId) REFERENCES BookBindings(Id) ON DELETE SET NULL,
    FOREIGN KEY (LanguageId) REFERENCES Languages(Id) ON DELETE SET NULL,
    FOREIGN KEY (ContentRatingId) REFERENCES ContentRatings(Id)
);

-- 03Secondary_UsedBook_CreateBookCategories_WithJoinTable

CREATE TABLE BookCategories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE,
    ParentId INT NULL,
    IsActive BIT NOT NULL DEFAULT(1),
    DisplayOrder INT NOT NULL DEFAULT(0),
    FOREIGN KEY (ParentId) REFERENCES BookCategories(Id)
);

CREATE TABLE UsedBookCategories (
    BookId UNIQUEIDENTIFIER NOT NULL,
    CategoryId INT NOT NULL,
    PRIMARY KEY (BookId, CategoryId),
    FOREIGN KEY (BookId) REFERENCES UsedBooks(Id) ON DELETE CASCADE,
    FOREIGN KEY (CategoryId) REFERENCES BookCategories(Id) ON DELETE CASCADE
);

-- 03Secondary_UsedBook_CreateBookConditionDetails_WithJoinTable

CREATE TABLE BookConditionDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE UsedBookConditionDetails (
    BookId UNIQUEIDENTIFIER NOT NULL,
    ConditionId INT NOT NULL,
    PRIMARY KEY (BookId, ConditionId),
    FOREIGN KEY (BookId) REFERENCES UsedBooks(Id) ON DELETE CASCADE,
    FOREIGN KEY (ConditionId) REFERENCES BookConditionDetails(Id) ON DELETE CASCADE
);

-- 03Secondary_UsedBook_CreateBookSaleTags_WithJoinTable

CREATE TABLE BookSaleTags (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE,
	IsActive BIT NOT NULL DEFAULT(1),
    DisplayOrder INT NOT NULL DEFAULT(0)
);

CREATE TABLE UsedBookSaleTags (
    BookId UNIQUEIDENTIFIER NOT NULL,
    TagId INT NOT NULL,
    PRIMARY KEY (BookId, TagId),
    FOREIGN KEY (BookId) REFERENCES UsedBooks(Id) ON DELETE CASCADE,
    FOREIGN KEY (TagId) REFERENCES BookSaleTags(Id) ON DELETE CASCADE
);
