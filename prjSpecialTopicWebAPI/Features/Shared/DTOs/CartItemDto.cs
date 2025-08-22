namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public class CartItemDto
    {
        public string? ImageUrl { get; set; }
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Dictionary<string, string>? Props { get; set; }
    }
}
