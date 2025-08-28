using prjSpecialTopicWebAPI.Features.Usedbook.Enums;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results
{
    public class UserOrderListItemDto
    {
        public string OrderNo { get; set; } = null!;
        public byte OrderStatus { get; set; }
        public byte PaymentStatus { get; set; }
        public byte DeliveryStatus { get; set; }
        public byte PaymentMethod { get; set; }
        public byte DeliveryMethod { get; set; }
        public Guid BuyerId { get; set; }
        public Guid SellerId { get; set; }
        public Guid BookId { get; set; }
        public string Title { get; set; } = null!;
        public decimal SalePrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
