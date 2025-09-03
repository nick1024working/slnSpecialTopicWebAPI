namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses
{
    public class OrderItemDto
    {
        public string CoverImageUrl { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal SalePrice { get; set; }
    }
}
