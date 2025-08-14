USE [TeamA_Project]

/* 更改 [BookConditionRatings] Name, Description 長度 */

ALTER TABLE [dbo].[BookConditionRatings]
DROP [UQ__BookCond__737584F6B9C32403]

ALTER TABLE [dbo].[BookConditionRatings]
ALTER COLUMN [Name] NVARCHAR(5);

ALTER TABLE [dbo].[BookConditionRatings]
ALTER COLUMN [Description] NVARCHAR(50);

ALTER TABLE [dbo].[BookConditionRatings]
ADD CONSTRAINT UQ_BookConditionRatings_Name UNIQUE (Name)

/* 更改 [BookBindings] Name 長度 */

ALTER TABLE [dbo].[BookBindings]
DROP [UQ__BookBind__737584F6966C8B55]

ALTER TABLE [dbo].[BookBindings]
ALTER COLUMN [Name] NVARCHAR(5);

ALTER TABLE [dbo].[BookBindings]
ADD CONSTRAINT UQ_BookBindings_Name UNIQUE (Name)

/* 更改 [BookSaleTags] Name 長度 */

ALTER TABLE [dbo].[BookSaleTags]
DROP [UQ__BookSale__737584F662E2FB7D]

ALTER TABLE [dbo].[BookSaleTags]
ALTER COLUMN [Name] NVARCHAR(10);

ALTER TABLE [dbo].[BookSaleTags]
ADD CONSTRAINT UQ_BookSaleTags_Name UNIQUE (Name)

/* 更改 [BookConditionDetails] Name 長度 */

ALTER TABLE [dbo].[BookConditionDetails]
DROP [UQ__BookCond__737584F60F4372FA]

ALTER TABLE [dbo].[BookConditionDetails]
ALTER COLUMN [Name] NVARCHAR(10);

ALTER TABLE [dbo].[BookConditionDetails]
ADD CONSTRAINT UQ_BookConditionDetails_Name UNIQUE (Name)

/* 更改 [UsedBooks] Name 長度 */

ALTER TABLE [dbo].[UsedBooks]
ALTER COLUMN [Title] NVARCHAR(50);

ALTER TABLE [dbo].[UsedBooks]
ALTER COLUMN [Authors] NVARCHAR(100);

ALTER TABLE [dbo].[UsedBooks]
ALTER COLUMN [ConditionDescription] NVARCHAR(100);

ALTER TABLE [dbo].[UsedBooks]
ALTER COLUMN [Edition] NVARCHAR(10);

ALTER TABLE [dbo].[UsedBooks]
ALTER COLUMN [Publisher] NVARCHAR(50);