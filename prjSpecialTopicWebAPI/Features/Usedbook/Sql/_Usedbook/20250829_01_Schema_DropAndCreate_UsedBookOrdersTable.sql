USE [TeamA_Project]

-- 移除
IF OBJECT_ID('dbo.UsedBookOrderItems', 'U') IS NOT NULL
    DROP TABLE dbo.UsedBookOrderItems;
IF OBJECT_ID('dbo.UsedBookOrders', 'U') IS NOT NULL
    DROP TABLE dbo.UsedBookOrders;

-- 新建 UsedBookOrders
CREATE TABLE UsedBookOrders (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	OrderNo NCHAR(20) NOT NULL,
	BuyerId UNIQUEIDENTIFIER NOT NULL,
	SellerId UNIQUEIDENTIFIER NOT NULL,

	OrderStatus TINYINT NOT NULL DEFAULT(0),
	PaymentStatus TINYINT NOT NULL DEFAULT(0),
	DeliveryStatus TINYINT NOT NULL DEFAULT(0),
	PaymentMethod TINYINT NOT NULL DEFAULT(0),
	DeliveryMethod TINYINT NOT NULL DEFAULT(0),

	TransactionId NVARCHAR(50) NULL,
	TrackingNumber NVARCHAR(50) NULL,

	Subtotal        DECIMAL(10,2) NOT NULL DEFAULT (0),
    DiscountTotal   DECIMAL(10,2) NOT NULL DEFAULT (0),
    DeliveryFee     DECIMAL(10,2) NOT NULL DEFAULT (0),
    GrandTotal      DECIMAL(10,2) NOT NULL DEFAULT (0),

	CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
    UpdatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),

	CONSTRAINT UQ_UsedBookOrders_OrderNo UNIQUE (OrderNo),
	CONSTRAINT FK_UsedBookOrders_BuyerId FOREIGN KEY (BuyerId) REFERENCES Users(UID),
	CONSTRAINT FK_UsedBookOrders_SellerId FOREIGN KEY (SellerId) REFERENCES Users(UID),
);

-- 新建 UsedBookOrderItems
CREATE TABLE UsedBookOrderItems (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	OrderId INT NOT NULL,
	BookId UNIQUEIDENTIFIER NOT NULL,
	Title NVARCHAR(50) NOT NULL,
	UnitPrice DECIMAL(10,2) NOT NULL,
    Quantity INT NOT NULL 

	CONSTRAINT FK_UsedBookOrderItems_OrderId FOREIGN KEY (OrderId) REFERENCES UsedBookOrders(Id),
	CONSTRAINT FK_UsedBookOrderItems_BookId FOREIGN KEY (BookId) REFERENCES UsedBooks(Id),
	CONSTRAINT UQ_UsedBookOrderItems_Order_Book UNIQUE (OrderId, BookId)
);