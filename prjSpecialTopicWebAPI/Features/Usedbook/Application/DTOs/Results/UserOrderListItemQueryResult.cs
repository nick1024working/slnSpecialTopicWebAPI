namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results
{
    public class UserOrderListItemQueryResult
    {
        public string OrderNo { get; set; } = null!;
        public Guid BuyerId { get; set; }
        public Guid SellerId { get; set; }

        public byte OrderStatus { get; set; }
        public byte PaymentStatus { get; set; }
        public byte DeliveryStatus { get; set; }
        public string TransactionId { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;

        public byte PaymentMethod { get; set; }
        public byte DeliveryMethod { get; set; }

        public decimal Subtotal { get; set; }
        public decimal DiscountTotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal GrandTotal { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
