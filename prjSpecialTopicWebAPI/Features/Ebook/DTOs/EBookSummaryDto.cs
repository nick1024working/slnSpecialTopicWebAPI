namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class EBookSummaryDto
    {
        public long EbookId { get; set; }
        public string EbookName { get; set; } = null!;
        public string Author { get; set; } = null!;
        public decimal FixedPrice { get; set; }
        public string? PrimaryCoverPath { get; set; }
    }
}
