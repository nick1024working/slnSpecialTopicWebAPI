namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class OrderHistoryItemDto
    {
        public long EbookId { get; set; }
        public string EbookName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? PrimaryCoverPath { get; set; }
    }
}
