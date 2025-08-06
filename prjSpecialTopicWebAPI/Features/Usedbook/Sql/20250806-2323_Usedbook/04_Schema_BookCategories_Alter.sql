USE [TeamA_Project]

/* ��� [BookCategories] DROP Column + �� Name ���� + �s�W Slug */

/*[ParentId]*/

ALTER TABLE [dbo].[BookCategories]
DROP [FK__BookCateg__Paren__2116E6DF];

ALTER TABLE [dbo].[BookCategories]
DROP COLUMN [ParentId];

/*[Name]*/

ALTER TABLE [dbo].[BookCategories]
DROP CONSTRAINT [UQ__BookCate__737584F680CEF102];

ALTER TABLE [dbo].[BookCategories]
ALTER COLUMN [Name] NVARCHAR(10) NOT NULL;

ALTER TABLE [dbo].[BookCategories]
ADD CONSTRAINT UQ_BookCategories_Name UNIQUE (Name);

/*[Slug]*/

ALTER TABLE [dbo].[BookCategories]
ADD [Slug] NVARCHAR(255) NOT NULL UNIQUE;

/* ��� [UsedBookCategories] DROP TABLE */

DROP TABLE [dbo].[UsedBookCategories];