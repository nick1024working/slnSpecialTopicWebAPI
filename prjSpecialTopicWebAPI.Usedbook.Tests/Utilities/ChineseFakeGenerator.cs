using System;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Utilities
{
    public class ChineseFakeGenerator
    {
        private readonly Random _rand = new();
        const string CommonChars =
            "的一是在不了有人我他們這中大為上個國到說時要就出會可也你對生能而子那得於著下自之年過後作裡用道行所然家種事成方多經麼去法學如都同現當沒動面起看定天分還進好小部其些主樣理心她本前開但因只從想實日軍者無工使車點次手問應長兩常間比題機七萬四每公司社工業資料系統數據網路設計應用程式管理平台服務用戶操作登入帳號密碼名稱標題內容新增修改刪除搜尋分類群組顯示排序選擇清單狀態圖片上傳下載備註說明價格名稱數量總計商品交易訂單付款運送完成取消失敗地址電話聯絡方式姓名電子信箱城市區域學校老師學生課程教學語言閱讀寫作數學物理化學生物歷史地理文化藝術音樂體育電腦科技網站手機裝置硬體軟體版本更新升級測試錯誤修正紀錄資訊安全密鑰驗證授權管理角色權限後台系統管理者使用者設定環境參數記憶體處理器資料庫儲存空間連線網頁檔案影像影片音訊壓縮格式支援功能選項切換快速鍵導覽選單工具列分頁標籤視窗佈景主題通知訊息提示警告錯誤成功完成載入儲存返回重新開始結束確認取消送出建立清除篩選排序匯入匯出統計分析報表圖表趨勢平均總和百分比最大最小中位數標準差關聯性模型預測學習訓練測試分類辨識自然語言深度學習神經網路決策樹支援向量機回歸分類聚類資料探勘機器學習人工智慧基因演算法強化學習語音辨識影像辨識翻譯推薦系統搜尋引擎社群媒體電子商務金融科技加密貨幣區塊鏈智慧合約去中心化非同質化代幣錢包交易所驗證節點挖礦共識演算法治理代碼開源版本控制分支提交合併測試部署生產環境除錯除錯日誌監控指標報警事件追蹤排程自動化腳本命令介面程式碼邏輯函式變數常數類別物件介面模組封裝繼承多型泛型錯誤處理例外中斷續行重試佇列堆疊樹圖連結串列排序搜尋演算法時間空間複雜度效能最佳化快取同步非同步併發執行緒鎖競爭死結延遲容錯擴充性可用性可靠性一致性分散式微服務容器虛擬化雲端平台伺服器資料中心架構網路協定位址封包加密認證授權防火牆跨域存取日誌分析資料備份還原快照儀表板視覺化";


        private string GenerateWord(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "長度必須大於 0");

            return new string(Enumerable.Range(0, length)
                .Select(_ => CommonChars[_rand.Next(CommonChars.Length)])
                .ToArray());
        }

        public string Word(int minValue, int maxValue)
        {
            return GenerateWord(_rand.Next(minValue, maxValue));
        }

        public string Name()
        {
            return GenerateWord(3);
        }

        private string RandomString(int length, string chars)
        {
            return new string(Enumerable.Range(0, length)
                .Select(_ => chars[_rand.Next(chars.Length)]).ToArray());
        }

        public string Password(int length = 12)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            return RandomString(length, chars);
        }

        public string Email(string domain = "example.com")
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            string user = RandomString(_rand.Next(6, 12), chars);
            return $"{user}@{domain}";
        }

        public string Isbn()
        {
            string firstDigit = _rand.Next(1, 10).ToString();
            string rest = string.Concat(Enumerable.Range(0, 12).Select(_ => _rand.Next(0, 10)));
            return firstDigit + rest;
        }

        public string Phone()
        {
            string firstTwoDigit = "09";
            string rest = string.Concat(Enumerable.Range(0, 8).Select(_ => _rand.Next(0, 10)));
            return firstTwoDigit + rest;
        }
    }

}
