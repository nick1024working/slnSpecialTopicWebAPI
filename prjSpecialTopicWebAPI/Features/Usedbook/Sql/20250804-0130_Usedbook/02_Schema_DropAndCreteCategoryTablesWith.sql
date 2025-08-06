USE [TeamA_Project]

DROP TABLE [dbo].[UsedBookCategories]
DROP TABLE [dbo].[BookCategories]

CREATE TABLE BookCategoryGroups (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(10) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT(1),
    DisplayOrder INT NOT NULL DEFAULT(0),
	Slug NVARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE BookCategories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    GroupId INT NOT NULL,
    Name NVARCHAR(10) NOT NULL,
    IsActive BIT NOT NULL DEFAULT(1),
    DisplayOrder INT NOT NULL DEFAULT(0),
	Slug NVARCHAR(255) NOT NULL UNIQUE,

	CONSTRAINT UQ_BookCategories_GroupId_Name UNIQUE(GroupId, Name),
	CONSTRAINT FK_BookCategories_BookCategoryGroups_GroupId
        FOREIGN KEY (GroupId)
        REFERENCES BookCategoryGroups(Id)
		ON DELETE CASCADE
);

CREATE TABLE UsedBookCategories (
    BookId UNIQUEIDENTIFIER NOT NULL,
    CategoryId INT NOT NULL,

    PRIMARY KEY (BookId, CategoryId),
    CONSTRAINT FK_UsedBookCategories_UsedBooks_BookId
		FOREIGN KEY (BookId) REFERENCES UsedBooks(Id) ON DELETE CASCADE,
	CONSTRAINT FK_UsedBookCategories_BookCategories_CategoryId
		FOREIGN KEY (CategoryId) REFERENCES BookCategories(Id) ON DELETE CASCADE
);