USE [TeamA_Project]
GO

CREATE TABLE UsedBookImages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    
    BookId UNIQUEIDENTIFIER NOT NULL,
    IsCover BIT NOT NULL DEFAULT(0),
    ImageIndex TINYINT NOT NULL,
    
    StorageProvider TINYINT NOT NULL DEFAULT(0),
    ObjectKey NVARCHAR(300) NOT NULL,
    Sha256 BINARY(32) NOT NULL,
    
    UploadedAt DATETIME2 NOT NULL DEFAULT(SYSUTCDATETIME()),

    FOREIGN KEY (BookId) REFERENCES UsedBooks(Id) ON DELETE CASCADE,
    UNIQUE (BookId, ImageIndex),
    UNIQUE (StorageProvider, ObjectKey),
	CHECK (ImageIndex >= 0)
);
GO