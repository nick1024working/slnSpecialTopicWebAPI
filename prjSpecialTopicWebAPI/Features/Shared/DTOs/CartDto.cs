namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal DiscountTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
