namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public class CheckoutDraftDto
    {
        public string ProductProvider { get; set; } = string.Empty;
        public string DeliveryOption { get; set; } = string.Empty;
        public string PaymentOption { get; set; } = string.Empty;

        public Guid? BuyerId { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string BuyerPhone { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;

        public int CountyId { get; set; }
        public int DistrictId { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
