namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results
{
    public class UserOrderListItemDto
    {
        public string OrderNo { get; set; } = null!;
        public Guid BuyerId { get; set; }
        public Guid SellerId { get; set; }

        public byte OrderStatus { get; set; }
        public byte PaymentStatus { get; set; }
        public byte DeliveryStatus { get; set; }

        public byte PaymentMethod { get; set; }
        public byte DeliveryMethod { get; set; }

        public decimal Subtotal { get; set; }
        public decimal DiscountTotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal GrandTotal { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
