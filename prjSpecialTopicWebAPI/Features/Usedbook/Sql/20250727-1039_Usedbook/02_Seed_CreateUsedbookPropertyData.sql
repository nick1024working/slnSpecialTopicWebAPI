USE [TeamA_Project]

INSERT INTO BookBindings (Name) VALUES (N'平裝');
INSERT INTO BookBindings (Name) VALUES (N'精裝');
INSERT INTO BookBindings (Name) VALUES (N'線裝');
INSERT INTO BookBindings (Name) VALUES (N'活頁裝');
INSERT INTO BookBindings (Name) VALUES (N'經摺裝');
INSERT INTO BookBindings (Name) VALUES (N'其他');

INSERT INTO BookConditionDetails (Name) VALUES (N'封面破損');
INSERT INTO BookConditionDetails (Name) VALUES (N'內頁破損');
INSERT INTO BookConditionDetails (Name) VALUES (N'潮濕水漬');
INSERT INTO BookConditionDetails (Name) VALUES (N'彎曲變形');
INSERT INTO BookConditionDetails (Name) VALUES (N'裝訂鬆動');
INSERT INTO BookConditionDetails (Name) VALUES (N'書斑霉斑');
INSERT INTO BookConditionDetails (Name) VALUES (N'蟲蛀');
INSERT INTO BookConditionDetails (Name) VALUES (N'曾為館藏書');

INSERT INTO BookConditionRatings (Name, Description) VALUES (N'近全新', N'書況與出版時幾乎一樣完美。');
INSERT INTO BookConditionRatings (Name, Description) VALUES (N'優良', N'能看出書籍曾經打開、閱讀或攜行，但書本本體、封面與內頁無任何顯著瑕疵或損壞。');
INSERT INTO BookConditionRatings (Name, Description) VALUES (N'良好', N'有明顯使用痕跡，但裝訂與紙張都保完整、內容可讀。外觀與品質已有磨損。');
INSERT INTO BookConditionRatings (Name, Description) VALUES (N'可接受', N'外觀與品質不良，但仍可閱讀的狀態。頁面可能有大量筆記，但不影響正文閱讀。');
INSERT INTO BookConditionRatings (Name, Description) VALUES (N'差', N'已影響閱讀上下文的狀態。可能有結構上瑕疵，如缺頁、嚴重撕裂或頁面脫落。');
