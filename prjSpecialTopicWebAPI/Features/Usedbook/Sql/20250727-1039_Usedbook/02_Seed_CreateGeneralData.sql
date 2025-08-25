USE [TeamA_Project]

INSERT INTO Languages (Name) VALUES (N'繁體中文');
INSERT INTO Languages (Name) VALUES (N'簡體中文');
INSERT INTO Languages (Name) VALUES (N'英文');
INSERT INTO Languages (Name) VALUES (N'日文');
INSERT INTO Languages (Name) VALUES (N'韓文');
INSERT INTO Languages (Name) VALUES (N'法文');
INSERT INTO Languages (Name) VALUES (N'德文');
INSERT INTO Languages (Name) VALUES (N'義大利文');
INSERT INTO Languages (Name) VALUES (N'西班牙文');
INSERT INTO Languages (Name) VALUES (N'越南文');
INSERT INTO Languages (Name) VALUES (N'泰文');
INSERT INTO Languages (Name) VALUES (N'印尼文');
INSERT INTO Languages (Name) VALUES (N'馬來文');
INSERT INTO Languages (Name) VALUES (N'俄文');
INSERT INTO Languages (Name) VALUES (N'阿拉伯文');
INSERT INTO Languages (Name) VALUES (N'葡萄牙文');
INSERT INTO Languages (Name) VALUES (N'客家語');
INSERT INTO Languages (Name) VALUES (N'台語');
INSERT INTO Languages (Name) VALUES (N'原住民族語');
INSERT INTO Languages (Name) VALUES (N'其他');

-- Counties
INSERT INTO Counties (Name) VALUES (N'基隆市');
INSERT INTO Counties (Name) VALUES (N'台北市');
INSERT INTO Counties (Name) VALUES (N'新北市');
INSERT INTO Counties (Name) VALUES (N'桃園市');
INSERT INTO Counties (Name) VALUES (N'新竹市');
INSERT INTO Counties (Name) VALUES (N'新竹縣');
INSERT INTO Counties (Name) VALUES (N'苗栗縣');
INSERT INTO Counties (Name) VALUES (N'台中市');
INSERT INTO Counties (Name) VALUES (N'彰化縣');
INSERT INTO Counties (Name) VALUES (N'南投縣');
INSERT INTO Counties (Name) VALUES (N'雲林縣');
INSERT INTO Counties (Name) VALUES (N'嘉義市');
INSERT INTO Counties (Name) VALUES (N'嘉義縣');
INSERT INTO Counties (Name) VALUES (N'台南市');
INSERT INTO Counties (Name) VALUES (N'高雄市');
INSERT INTO Counties (Name) VALUES (N'屏東縣');
INSERT INTO Counties (Name) VALUES (N'宜蘭縣');
INSERT INTO Counties (Name) VALUES (N'花蓮縣');
INSERT INTO Counties (Name) VALUES (N'台東縣');
INSERT INTO Counties (Name) VALUES (N'澎湖縣');
INSERT INTO Counties (Name) VALUES (N'金門縣');
INSERT INTO Counties (Name) VALUES (N'連江縣');

-- 基隆市 (CountyId = 1)
INSERT INTO Districts (CountyId, Name) VALUES (1, N'仁愛區');
INSERT INTO Districts (CountyId, Name) VALUES (1, N'信義區');
INSERT INTO Districts (CountyId, Name) VALUES (1, N'中正區');
INSERT INTO Districts (CountyId, Name) VALUES (1, N'中山區');
INSERT INTO Districts (CountyId, Name) VALUES (1, N'安樂區');
INSERT INTO Districts (CountyId, Name) VALUES (1, N'暖暖區');
INSERT INTO Districts (CountyId, Name) VALUES (1, N'七堵區');

INSERT INTO Districts (CountyId, Name) VALUES (2, N'中正區');
INSERT INTO Districts (CountyId, Name) VALUES (2, N'大同區');
INSERT INTO Districts (CountyId, Name) VALUES (2, N'中山區');
INSERT INTO Districts (CountyId, Name) VALUES (2, N'松山區');
INSERT INTO Districts (CountyId, Name) VALUES (2, N'大安區');
INSERT INTO Districts (CountyId, Name) VALUES (2, N'萬華區');
INSERT INTO Districts (CountyId, Name) VALUES (2, N'信義區');
INSERT INTO Districts (CountyId, Name) VALUES (2, N'士林區');
INSERT INTO Districts (CountyId, Name) VALUES (2, N'北投區');
INSERT INTO Districts (CountyId, Name) VALUES (2, N'內湖區');
INSERT INTO Districts (CountyId, Name) VALUES (2, N'南港區');
INSERT INTO Districts (CountyId, Name) VALUES (2, N'文山區');

INSERT INTO Districts (CountyId, Name) VALUES (3, N'板橋區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'三重區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'中和區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'永和區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'新莊區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'新店區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'樹林區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'鶯歌區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'三峽區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'淡水區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'瑞芳區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'土城區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'蘆洲區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'五股區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'泰山區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'林口區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'深坑區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'石碇區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'坪林區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'三芝區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'石門區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'八里區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'平溪區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'雙溪區');
INSERT INTO Districts (CountyId, Name) VALUES (3, N'貢寮區');

INSERT INTO Districts (CountyId, Name) VALUES (4, N'桃園區');
INSERT INTO Districts (CountyId, Name) VALUES (4, N'中壢區');
INSERT INTO Districts (CountyId, Name) VALUES (4, N'平鎮區');
INSERT INTO Districts (CountyId, Name) VALUES (4, N'八德區');
INSERT INTO Districts (CountyId, Name) VALUES (4, N'楊梅區');
INSERT INTO Districts (CountyId, Name) VALUES (4, N'蘆竹區');
INSERT INTO Districts (CountyId, Name) VALUES (4, N'大溪區');
INSERT INTO Districts (CountyId, Name) VALUES (4, N'大園區');
INSERT INTO Districts (CountyId, Name) VALUES (4, N'龜山區');
INSERT INTO Districts (CountyId, Name) VALUES (4, N'龍潭區');
INSERT INTO Districts (CountyId, Name) VALUES (4, N'新屋區');
INSERT INTO Districts (CountyId, Name) VALUES (4, N'觀音區');
INSERT INTO Districts (CountyId, Name) VALUES (4, N'復興區');

-- 新竹市 (CountyId = 5)
INSERT INTO Districts (CountyId, Name) VALUES (5, N'東區');
INSERT INTO Districts (CountyId, Name) VALUES (5, N'北區');
INSERT INTO Districts (CountyId, Name) VALUES (5, N'香山區');

-- 新竹縣 (CountyId = 6)
INSERT INTO Districts (CountyId, Name) VALUES (6, N'竹北市');
INSERT INTO Districts (CountyId, Name) VALUES (6, N'竹東鎮');
INSERT INTO Districts (CountyId, Name) VALUES (6, N'新埔鎮');
INSERT INTO Districts (CountyId, Name) VALUES (6, N'關西鎮');
INSERT INTO Districts (CountyId, Name) VALUES (6, N'湖口鄉');
INSERT INTO Districts (CountyId, Name) VALUES (6, N'新豐鄉');
INSERT INTO Districts (CountyId, Name) VALUES (6, N'芎林鄉');
INSERT INTO Districts (CountyId, Name) VALUES (6, N'橫山鄉');
INSERT INTO Districts (CountyId, Name) VALUES (6, N'北埔鄉');
INSERT INTO Districts (CountyId, Name) VALUES (6, N'寶山鄉');
INSERT INTO Districts (CountyId, Name) VALUES (6, N'峨眉鄉');
INSERT INTO Districts (CountyId, Name) VALUES (6, N'尖石鄉');
INSERT INTO Districts (CountyId, Name) VALUES (6, N'五峰鄉');

-- 苗栗縣 (CountyId = 7)
INSERT INTO Districts (CountyId, Name) VALUES (7, N'苗栗市');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'苑裡鎮');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'通霄鎮');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'竹南鎮');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'頭份市');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'後龍鎮');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'卓蘭鎮');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'大湖鄉');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'公館鄉');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'銅鑼鄉');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'南庄鄉');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'頭屋鄉');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'三義鄉');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'西湖鄉');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'造橋鄉');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'三灣鄉');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'獅潭鄉');
INSERT INTO Districts (CountyId, Name) VALUES (7, N'泰安鄉');

INSERT INTO Districts (CountyId, Name) VALUES (8, N'中區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'東區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'西區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'南區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'北區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'西屯區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'南屯區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'北屯區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'豐原區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'東勢區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'大甲區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'清水區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'沙鹿區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'梧棲區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'后里區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'神岡區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'潭子區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'大雅區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'新社區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'石岡區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'外埔區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'大安區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'烏日區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'大肚區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'龍井區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'霧峰區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'太平區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'大里區');
INSERT INTO Districts (CountyId, Name) VALUES (8, N'和平區');


-- 彰化縣 (CountyId = 9)
INSERT INTO Districts (CountyId, Name) VALUES (9, N'彰化市');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'鹿港鎮');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'和美鎮');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'線西鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'伸港鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'福興鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'秀水鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'花壇鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'芬園鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'員林市');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'溪湖鎮');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'田中鎮');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'大村鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'埔鹽鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'埔心鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'永靖鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'社頭鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'二水鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'北斗鎮');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'二林鎮');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'田尾鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'埤頭鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'芳苑鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'大城鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'竹塘鄉');
INSERT INTO Districts (CountyId, Name) VALUES (9, N'溪州鄉');

-- 南投縣 (CountyId = 10)
INSERT INTO Districts (CountyId, Name) VALUES (10, N'南投市');
INSERT INTO Districts (CountyId, Name) VALUES (10, N'中寮鄉');
INSERT INTO Districts (CountyId, Name) VALUES (10, N'草屯鎮');
INSERT INTO Districts (CountyId, Name) VALUES (10, N'國姓鄉');
INSERT INTO Districts (CountyId, Name) VALUES (10, N'埔里鎮');
INSERT INTO Districts (CountyId, Name) VALUES (10, N'仁愛鄉');
INSERT INTO Districts (CountyId, Name) VALUES (10, N'名間鄉');
INSERT INTO Districts (CountyId, Name) VALUES (10, N'集集鎮');
INSERT INTO Districts (CountyId, Name) VALUES (10, N'水里鄉');
INSERT INTO Districts (CountyId, Name) VALUES (10, N'魚池鄉');
INSERT INTO Districts (CountyId, Name) VALUES (10, N'信義鄉');
INSERT INTO Districts (CountyId, Name) VALUES (10, N'竹山鎮');
INSERT INTO Districts (CountyId, Name) VALUES (10, N'鹿谷鄉');

-- 雲林縣 (CountyId = 11)
INSERT INTO Districts (CountyId, Name) VALUES (11, N'斗南鎮');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'大埤鄉');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'虎尾鎮');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'土庫鎮');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'褒忠鄉');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'東勢鄉');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'台西鄉');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'崙背鄉');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'麥寮鄉');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'斗六市');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'林內鄉');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'古坑鄉');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'莿桐鄉');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'西螺鎮');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'二崙鄉');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'北港鎮');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'水林鄉');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'口湖鄉');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'四湖鄉');
INSERT INTO Districts (CountyId, Name) VALUES (11, N'元長鄉');

-- 嘉義市 (CountyId = 12)
INSERT INTO Districts (CountyId, Name) VALUES (12, N'東區');
INSERT INTO Districts (CountyId, Name) VALUES (12, N'西區');

-- 嘉義縣 (CountyId = 13)
INSERT INTO Districts (CountyId, Name) VALUES (13, N'太保市');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'朴子市');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'布袋鎮');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'大林鎮');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'民雄鄉');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'溪口鄉');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'新港鄉');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'六腳鄉');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'東石鄉');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'義竹鄉');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'鹿草鄉');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'水上鄉');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'中埔鄉');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'竹崎鄉');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'梅山鄉');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'番路鄉');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'大埔鄉');
INSERT INTO Districts (CountyId, Name) VALUES (13, N'阿里山鄉');

INSERT INTO Districts (CountyId, Name) VALUES (14, N'中西區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'東區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'南區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'北區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'安平區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'安南區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'永康區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'歸仁區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'新化區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'左鎮區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'玉井區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'楠西區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'南化區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'仁德區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'關廟區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'龍崎區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'官田區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'麻豆區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'佳里區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'西港區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'七股區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'將軍區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'學甲區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'北門區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'新營區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'後壁區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'白河區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'東山區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'六甲區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'下營區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'柳營區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'鹽水區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'善化區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'大內區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'山上區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'新市區');
INSERT INTO Districts (CountyId, Name) VALUES (14, N'安定區');

INSERT INTO Districts (CountyId, Name) VALUES (15, N'新興區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'前金區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'苓雅區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'鹽埕區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'鼓山區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'旗津區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'前鎮區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'三民區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'楠梓區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'小港區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'左營區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'仁武區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'大社區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'岡山區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'路竹區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'阿蓮區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'田寮區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'燕巢區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'橋頭區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'梓官區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'彌陀區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'永安區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'湖內區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'鳳山區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'大寮區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'林園區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'鳥松區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'大樹區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'旗山區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'美濃區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'六龜區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'內門區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'杉林區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'甲仙區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'桃源區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'那瑪夏區');
INSERT INTO Districts (CountyId, Name) VALUES (15, N'茄萣區');

-- 屏東縣 (CountyId = 16)
INSERT INTO Districts (CountyId, Name) VALUES (16, N'屏東市');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'潮州鎮');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'東港鎮');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'恆春鎮');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'萬丹鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'長治鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'麟洛鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'九如鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'里港鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'鹽埔鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'高樹鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'萬巒鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'內埔鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'竹田鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'新埤鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'枋寮鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'新園鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'崁頂鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'林邊鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'南州鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'佳冬鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'琉球鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'車城鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'滿州鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'枋山鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'三地門鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'霧台鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'瑪家鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'泰武鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'來義鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'春日鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'獅子鄉');
INSERT INTO Districts (CountyId, Name) VALUES (16, N'牡丹鄉');

-- 宜蘭縣 (CountyId = 17)
INSERT INTO Districts (CountyId, Name) VALUES (17, N'宜蘭市');
INSERT INTO Districts (CountyId, Name) VALUES (17, N'羅東鎮');
INSERT INTO Districts (CountyId, Name) VALUES (17, N'蘇澳鎮');
INSERT INTO Districts (CountyId, Name) VALUES (17, N'頭城鎮');
INSERT INTO Districts (CountyId, Name) VALUES (17, N'礁溪鄉');
INSERT INTO Districts (CountyId, Name) VALUES (17, N'壯圍鄉');
INSERT INTO Districts (CountyId, Name) VALUES (17, N'員山鄉');
INSERT INTO Districts (CountyId, Name) VALUES (17, N'冬山鄉');
INSERT INTO Districts (CountyId, Name) VALUES (17, N'五結鄉');
INSERT INTO Districts (CountyId, Name) VALUES (17, N'三星鄉');
INSERT INTO Districts (CountyId, Name) VALUES (17, N'大同鄉');
INSERT INTO Districts (CountyId, Name) VALUES (17, N'南澳鄉');

-- 花蓮縣 (CountyId = 18)
INSERT INTO Districts (CountyId, Name) VALUES (18, N'花蓮市');
INSERT INTO Districts (CountyId, Name) VALUES (18, N'鳳林鎮');
INSERT INTO Districts (CountyId, Name) VALUES (18, N'玉里鎮');
INSERT INTO Districts (CountyId, Name) VALUES (18, N'新城鄉');
INSERT INTO Districts (CountyId, Name) VALUES (18, N'吉安鄉');
INSERT INTO Districts (CountyId, Name) VALUES (18, N'壽豐鄉');
INSERT INTO Districts (CountyId, Name) VALUES (18, N'光復鄉');
INSERT INTO Districts (CountyId, Name) VALUES (18, N'豐濱鄉');
INSERT INTO Districts (CountyId, Name) VALUES (18, N'瑞穗鄉');
INSERT INTO Districts (CountyId, Name) VALUES (18, N'萬榮鄉');
INSERT INTO Districts (CountyId, Name) VALUES (18, N'卓溪鄉');

-- 台東縣 (CountyId = 19)
INSERT INTO Districts (CountyId, Name) VALUES (19, N'台東市');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'成功鎮');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'關山鎮');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'卑南鄉');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'鹿野鄉');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'池上鄉');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'東河鄉');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'長濱鄉');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'太麻里鄉');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'大武鄉');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'綠島鄉');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'蘭嶼鄉');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'延平鄉');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'金峰鄉');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'達仁鄉');
INSERT INTO Districts (CountyId, Name) VALUES (19, N'海端鄉');

-- 澎湖縣 (CountyId = 20)
INSERT INTO Districts (CountyId, Name) VALUES (20, N'馬公市');
INSERT INTO Districts (CountyId, Name) VALUES (20, N'西嶼鄉');
INSERT INTO Districts (CountyId, Name) VALUES (20, N'望安鄉');
INSERT INTO Districts (CountyId, Name) VALUES (20, N'七美鄉');
INSERT INTO Districts (CountyId, Name) VALUES (20, N'白沙鄉');
INSERT INTO Districts (CountyId, Name) VALUES (20, N'湖西鄉');

-- 金門縣 (CountyId = 21)
INSERT INTO Districts (CountyId, Name) VALUES (21, N'金城鎮');
INSERT INTO Districts (CountyId, Name) VALUES (21, N'金沙鎮');
INSERT INTO Districts (CountyId, Name) VALUES (21, N'金湖鎮');
INSERT INTO Districts (CountyId, Name) VALUES (21, N'金寧鄉');
INSERT INTO Districts (CountyId, Name) VALUES (21, N'烈嶼鄉');
INSERT INTO Districts (CountyId, Name) VALUES (21, N'烏坵鄉');

-- 連江縣 (CountyId = 22)
INSERT INTO Districts (CountyId, Name) VALUES (22, N'南竿鄉');
INSERT INTO Districts (CountyId, Name) VALUES (22, N'北竿鄉');
INSERT INTO Districts (CountyId, Name) VALUES (22, N'莒光鄉');
INSERT INTO Districts (CountyId, Name) VALUES (22, N'東引鄉');

INSERT INTO ContentRatings (Name) VALUES (N'普遍級');
INSERT INTO ContentRatings (Name) VALUES (N'限制級');
