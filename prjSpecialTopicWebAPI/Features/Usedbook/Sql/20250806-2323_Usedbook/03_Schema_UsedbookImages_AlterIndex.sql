USE [TeamA_Project]

/* зєзя [UsedBookImages] ImageIndex */
ALTER TABLE [dbo].[UsedBookImages]
DROP [CK__UsedBookI__Image__3CBF0154];

ALTER TABLE [dbo].[UsedBookImages]
DROP [UQ__UsedBook__D26AE13703BB75E8];

ALTER TABLE [dbo].[UsedBookImages]
ALTER COLUMN [ImageIndex] INT NOT NULL;

EXEC sp_rename 'UsedBookImages.ImageIndex', 'DisplayOrder', 'COLUMN';