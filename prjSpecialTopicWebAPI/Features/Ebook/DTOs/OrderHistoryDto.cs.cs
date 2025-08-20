namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class OrderHistoryDto
    {
        public string OrderId { get; set; } = null!;
        public string OrderDate { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public List<OrderHistoryItemDto> Items { get; set; } = new();
    }
}
