USE [TeamA_Project]

/* зєзя [BookSaleTags] */

ALTER TABLE [dbo].[BookSaleTags]
ADD Slug NVARCHAR(255) NOT NULL UNIQUE;