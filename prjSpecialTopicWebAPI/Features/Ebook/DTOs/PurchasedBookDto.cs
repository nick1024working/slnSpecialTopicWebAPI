namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class PurchasedBookDto
    {
        public long EbookId { get; set; }
        public string EbookName { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string? PrimaryCoverPath { get; set; }
        public bool IsReadable { get; set; }
        public string? ReadingProgress { get; set; } // [新增] 閱讀進度
    }
}
