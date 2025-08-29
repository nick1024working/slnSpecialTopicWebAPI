namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public class UpdateOrderStatusRequest
    {
        public byte? OrderStatus { get; set; }
        public byte? PaymentStatus { get; set; }
        public byte? DeliveryStatus { get; set; }
    }
}
