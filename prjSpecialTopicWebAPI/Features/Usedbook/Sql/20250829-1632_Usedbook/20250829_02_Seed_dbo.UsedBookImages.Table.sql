USE [TeamA_Project]
GO
SET IDENTITY_INSERT [dbo].[UsedBookImages] ON 
GO

INSERT [dbo].[UsedBookImages] ([Id], [BookId], [IsCover], [DisplayOrder], [StorageProvider], [ObjectKey], [Sha256], [UploadedAt]) VALUES (105, N'156ffa7f-d8fa-4cce-8a80-fc24751f9318', 1, 1, 1, N'abc571fa3ae24be1a9c6da82ad1993c4', 0x999A4E12B9B9B794753FACC4BD6F9DF99CD7716F1D1D9739D92F6D7DD9557856, CAST(N'2025-08-21T07:53:13.9655944' AS DateTime2))
GO
INSERT [dbo].[UsedBookImages] ([Id], [BookId], [IsCover], [DisplayOrder], [StorageProvider], [ObjectKey], [Sha256], [UploadedAt]) VALUES (106, N'2bc578b6-f7cd-41cb-befb-34a485e1a764', 1, 1, 1, N'38ce0383876f4be9a75847eaeccb1472', 0x999A4E12B9B9B794753FACC4BD6F9DF99CD7716F1D1D9739D92F6D7DD9557856, CAST(N'2025-08-21T08:07:28.2114070' AS DateTime2))
GO
INSERT [dbo].[UsedBookImages] ([Id], [BookId], [IsCover], [DisplayOrder], [StorageProvider], [ObjectKey], [Sha256], [UploadedAt]) VALUES (107, N'e96dc93f-32c1-448a-84fc-9fd373c2810f', 1, 1, 1, N'9f605f3a4feb446e86561e7b29d7ee6e', 0x999A4E12B9B9B794753FACC4BD6F9DF99CD7716F1D1D9739D92F6D7DD9557856, CAST(N'2025-08-21T08:09:46.4771956' AS DateTime2))
GO
INSERT [dbo].[UsedBookImages] ([Id], [BookId], [IsCover], [DisplayOrder], [StorageProvider], [ObjectKey], [Sha256], [UploadedAt]) VALUES (108, N'4729bc4b-a00f-408a-8212-300b4e69f8e1', 1, 1, 1, N'4cfb2a2133c247da95213db6ec9d46b1', 0x999A4E12B9B9B794753FACC4BD6F9DF99CD7716F1D1D9739D92F6D7DD9557856, CAST(N'2025-08-21T08:09:49.8839057' AS DateTime2))
GO

SET IDENTITY_INSERT [dbo].[UsedBookImages] OFF
GO
