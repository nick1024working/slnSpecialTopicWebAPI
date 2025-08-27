namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class EBookSummaryDto
    {
        public long EbookId { get; set; }
        public string EbookName { get; set; } = null!;
        public string Author { get; set; } = null!;
        public decimal FixedPrice { get; set; }
        public string? PrimaryCoverPath { get; set; }

        // [新增] 請將這一行加入到您的 class 中
        public bool IsReadable { get; set; }
        // [新增] 請將這一行加入到您的 class 中
        public decimal? ActualPrice { get; set; }

        // --- [新增] 在這裡補上缺少的屬性 ---
        public string? CategoryName { get; set; }
        public List<string> Labels { get; set; } = new();
    }
}
