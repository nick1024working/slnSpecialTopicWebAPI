namespace prjSpecialTopicWebAPI.Features.Usedbook.Utilities
{
    /// <summary>
    /// 代表無內容的佔位型別，類似函式語言中的 Unit 或 void。
    /// </summary>
    public sealed class Unit
    {
        // 全域唯一實體
        public static readonly Unit Value = new Unit();

        // 私有建構子，防止外部建立其他實例
        private Unit() { }

        // 為了除錯或日誌時顯示
        public override string ToString() => "()";
    }
}