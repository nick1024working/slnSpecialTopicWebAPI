USE [TeamA_Project]

/* ��� [BookSaleTags] */

ALTER TABLE [dbo].[BookSaleTags]
ADD Slug NVARCHAR(255) NOT NULL UNIQUE;