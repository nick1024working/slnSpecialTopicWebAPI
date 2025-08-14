USE [TeamA_Project]

/* ��� [BookConditionRatings] NOT NULL */

ALTER TABLE [dbo].[BookConditionRatings]
DROP [UQ__BookCond__737584F6B9C32403]

ALTER TABLE [dbo].[BookConditionRatings]
ALTER COLUMN [Name] NVARCHAR(5) NOT NULL;

ALTER TABLE [dbo].[BookConditionRatings]
ADD CONSTRAINT UQ_BookConditionRatings_Name UNIQUE (Name)

/* ��� [BookBindings] NOT NULL */

ALTER TABLE [dbo].[BookBindings]
DROP [UQ__BookBind__737584F6966C8B55]

ALTER TABLE [dbo].[BookBindings]
ALTER COLUMN [Name] NVARCHAR(5) NOT NULL;

ALTER TABLE [dbo].[BookBindings]
ADD CONSTRAINT UQ_BookBindings_Name UNIQUE (Name)

/* ��� [BookSaleTags] NOT NULL */

ALTER TABLE [dbo].[BookSaleTags]
DROP [UQ__BookSale__737584F662E2FB7D]

ALTER TABLE [dbo].[BookSaleTags]
ALTER COLUMN [Name] NVARCHAR(10) NOT NULL;

ALTER TABLE [dbo].[BookSaleTags]
ADD CONSTRAINT UQ_BookSaleTags_Name UNIQUE (Name)

/* ��� [BookConditionDetails] NOT NULL */

ALTER TABLE [dbo].[BookConditionDetails]
DROP [UQ__BookCond__737584F60F4372FA]

ALTER TABLE [dbo].[BookConditionDetails]
ALTER COLUMN [Name] NVARCHAR(10) NOT NULL;

ALTER TABLE [dbo].[BookConditionDetails]
ADD CONSTRAINT UQ_BookConditionDetails_Name UNIQUE (Name)

/* ��� [UsedBooks] NOT NULL */

ALTER TABLE [dbo].[UsedBooks]
ALTER COLUMN [Title] NVARCHAR(50) NOT NULL;

ALTER TABLE [dbo].[UsedBooks]
ALTER COLUMN [Authors] NVARCHAR(100) NOT NULL;