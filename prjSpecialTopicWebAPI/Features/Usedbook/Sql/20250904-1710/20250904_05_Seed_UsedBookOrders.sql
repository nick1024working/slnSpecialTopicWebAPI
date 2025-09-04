USE [TeamA_Project]
GO
SET IDENTITY_INSERT [dbo].[UsedBookOrders] ON 
GO
INSERT [dbo].[UsedBookOrders] ([Id], [OrderNo], [BuyerId], [SellerId], [OrderStatus], [PaymentStatus], [DeliveryStatus], [PaymentMethod], [DeliveryMethod], [TransactionId], [TrackingNumber], [Subtotal], [DiscountTotal], [DeliveryFee], [GrandTotal], [CreatedAt], [UpdatedAt]) VALUES (1, N'20250904071933892292', N'77f14905-7958-4d79-917a-0472653d42ab', N'ebb03874-054f-4fea-9ae8-02b8d05c4bb3', 2, 3, 0, 1, 0, N'2025090402303920910', NULL, CAST(456.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(456.00 AS Decimal(10, 2)), CAST(N'2025-09-04T07:19:33.8928954' AS DateTime2), CAST(N'2025-09-04T07:19:33.8928954' AS DateTime2))
GO
INSERT [dbo].[UsedBookOrders] ([Id], [OrderNo], [BuyerId], [SellerId], [OrderStatus], [PaymentStatus], [DeliveryStatus], [PaymentMethod], [DeliveryMethod], [TransactionId], [TrackingNumber], [Subtotal], [DiscountTotal], [DeliveryFee], [GrandTotal], [CreatedAt], [UpdatedAt]) VALUES (2, N'20250904072419490046', N'b322a195-58bd-4bd6-96b0-06b65dcada17', N'77f14905-7958-4d79-917a-0472653d42ab', 2, 3, 0, 1, 0, N'2025090402303921910', NULL, CAST(304.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(304.00 AS Decimal(10, 2)), CAST(N'2025-09-04T07:24:19.4900077' AS DateTime2), CAST(N'2025-09-04T07:24:19.4900077' AS DateTime2))
GO
INSERT [dbo].[UsedBookOrders] ([Id], [OrderNo], [BuyerId], [SellerId], [OrderStatus], [PaymentStatus], [DeliveryStatus], [PaymentMethod], [DeliveryMethod], [TransactionId], [TrackingNumber], [Subtotal], [DiscountTotal], [DeliveryFee], [GrandTotal], [CreatedAt], [UpdatedAt]) VALUES (3, N'20250904072540670316', N'b322a195-58bd-4bd6-96b0-06b65dcada17', N'77f14905-7958-4d79-917a-0472653d42ab', 2, 3, 0, 1, 0, N'2025090402303922610', NULL, CAST(381.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(381.00 AS Decimal(10, 2)), CAST(N'2025-09-04T07:25:40.6708989' AS DateTime2), CAST(N'2025-09-04T07:25:40.6708989' AS DateTime2))
GO
INSERT [dbo].[UsedBookOrders] ([Id], [OrderNo], [BuyerId], [SellerId], [OrderStatus], [PaymentStatus], [DeliveryStatus], [PaymentMethod], [DeliveryMethod], [TransactionId], [TrackingNumber], [Subtotal], [DiscountTotal], [DeliveryFee], [GrandTotal], [CreatedAt], [UpdatedAt]) VALUES (5, N'20250904073042606647', N'9d3cc4fd-63f2-464c-9746-04960f8a79a5', N'77f14905-7958-4d79-917a-0472653d42ab', 2, 3, 0, 1, 0, N'2025090402303923410', NULL, CAST(168.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(168.00 AS Decimal(10, 2)), CAST(N'2025-09-04T07:30:42.6068041' AS DateTime2), CAST(N'2025-09-04T07:30:42.6068041' AS DateTime2))
GO
INSERT [dbo].[UsedBookOrders] ([Id], [OrderNo], [BuyerId], [SellerId], [OrderStatus], [PaymentStatus], [DeliveryStatus], [PaymentMethod], [DeliveryMethod], [TransactionId], [TrackingNumber], [Subtotal], [DiscountTotal], [DeliveryFee], [GrandTotal], [CreatedAt], [UpdatedAt]) VALUES (6, N'20250904073333789584', N'ebb03874-054f-4fea-9ae8-02b8d05c4bb3', N'77f14905-7958-4d79-917a-0472653d42ab', 2, 3, 0, 1, 0, N'2025090402303923910', NULL, CAST(193.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(193.00 AS Decimal(10, 2)), CAST(N'2025-09-04T07:33:33.7895800' AS DateTime2), CAST(N'2025-09-04T07:33:33.7895800' AS DateTime2))
GO
INSERT [dbo].[UsedBookOrders] ([Id], [OrderNo], [BuyerId], [SellerId], [OrderStatus], [PaymentStatus], [DeliveryStatus], [PaymentMethod], [DeliveryMethod], [TransactionId], [TrackingNumber], [Subtotal], [DiscountTotal], [DeliveryFee], [GrandTotal], [CreatedAt], [UpdatedAt]) VALUES (11, N'20250904081636887220', N'5414b103-cb9f-4101-8b69-00d82d940729', N'77f14905-7958-4d79-917a-0472653d42ab', 2, 3, 0, 1, 1, N'2025090402303934410', NULL, CAST(129.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), CAST(249.00 AS Decimal(10, 2)), CAST(N'2025-09-04T08:16:36.8875919' AS DateTime2), CAST(N'2025-09-04T08:16:36.8875919' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[UsedBookOrders] OFF
GO
SET IDENTITY_INSERT [dbo].[UsedBookOrderItems] ON 
GO
INSERT [dbo].[UsedBookOrderItems] ([Id], [OrderId], [BookId], [Title], [UnitPrice], [Quantity]) VALUES (1, 1, N'7b20926b-17ed-4823-b24c-68f01c3918cc', N'證嚴上人衲履足跡. 二0二四年. 春之卷', CAST(219.00 AS Decimal(10, 2)), 1)
GO
INSERT [dbo].[UsedBookOrderItems] ([Id], [OrderId], [BookId], [Title], [UnitPrice], [Quantity]) VALUES (2, 1, N'bc22ecba-dc65-4660-8a93-d2a237651984', N'佛系貴女 (卷2)', CAST(237.00 AS Decimal(10, 2)), 1)
GO
INSERT [dbo].[UsedBookOrderItems] ([Id], [OrderId], [BookId], [Title], [UnitPrice], [Quantity]) VALUES (3, 2, N'dd0547fe-fb2a-429a-829b-550d91773eb1', N'現代財務管理', CAST(304.00 AS Decimal(10, 2)), 1)
GO
INSERT [dbo].[UsedBookOrderItems] ([Id], [OrderId], [BookId], [Title], [UnitPrice], [Quantity]) VALUES (4, 3, N'7ce93f97-5098-4ac5-b120-8d4938a85949', N'優雅進入更年期: 留住青春、延緩衰老', CAST(381.00 AS Decimal(10, 2)), 1)
GO
INSERT [dbo].[UsedBookOrderItems] ([Id], [OrderId], [BookId], [Title], [UnitPrice], [Quantity]) VALUES (6, 5, N'b309561c-3e73-4059-9345-7764a05a8434', N'萬界錢莊 (第6冊)', CAST(168.00 AS Decimal(10, 2)), 1)
GO
INSERT [dbo].[UsedBookOrderItems] ([Id], [OrderId], [BookId], [Title], [UnitPrice], [Quantity]) VALUES (7, 6, N'5c0616fd-8381-4b5c-a10e-01211a079ea9', N'發展障礙完全自立手冊. 商務篇', CAST(193.00 AS Decimal(10, 2)), 1)
GO
INSERT [dbo].[UsedBookOrderItems] ([Id], [OrderId], [BookId], [Title], [UnitPrice], [Quantity]) VALUES (12, 11, N'fe6af8a7-907b-4747-976b-165e10a3036f', N'愛情白皮書. 第2部 (第2冊)', CAST(129.00 AS Decimal(10, 2)), 1)
GO
SET IDENTITY_INSERT [dbo].[UsedBookOrderItems] OFF
GO
