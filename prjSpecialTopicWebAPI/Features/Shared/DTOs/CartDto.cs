using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new();

        public decimal Subtotal { get; set; } = 0;

        public decimal DiscountTotal { get; set; } = 0;

        public decimal ShippingFee { get; set; } = 0;

        public decimal GrandTotal { get; set; } = 0;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
