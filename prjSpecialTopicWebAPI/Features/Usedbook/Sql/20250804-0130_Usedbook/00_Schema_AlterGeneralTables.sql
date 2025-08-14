USE [TeamA_Project]

/* ��� [Counties] Name ���� */

ALTER TABLE [dbo].[Counties]
DROP UQ__Counties__737584F60AE764B6

ALTER TABLE [dbo].[Counties]
ALTER COLUMN [Name] NVARCHAR(5);

ALTER TABLE [dbo].[Counties]
ADD CONSTRAINT UQ_Counties_Name UNIQUE (Name)

/* ��� [Districts] Name ���� */

ALTER TABLE [dbo].[Districts]
DROP UQ_Districts_CountyId_Name

ALTER TABLE [dbo].[Districts]
ALTER COLUMN [Name] NVARCHAR(5);

ALTER TABLE [dbo].[Districts]
ADD CONSTRAINT UQ_Districts_CountyId_Name UNIQUE (CountyId, Name)

/* ��� [Languages] Name ���� */

ALTER TABLE [dbo].[Languages]
DROP [UQ__Language__737584F64B7B8C3D]

ALTER TABLE [dbo].[Languages]
ALTER COLUMN [Name] NVARCHAR(5);

ALTER TABLE [dbo].[Languages]
ADD CONSTRAINT UQ_Language_Name UNIQUE (Name)

/* ��� [ContentRatings] Name ���� */

ALTER TABLE [dbo].[ContentRatings]
DROP [UQ__ContentR__737584F6E59B0541]

ALTER TABLE [dbo].[ContentRatings]
ALTER COLUMN [Name] NVARCHAR(5);

ALTER TABLE [dbo].[ContentRatings]
ADD CONSTRAINT UQ_ContentRatings_Name UNIQUE (Name)