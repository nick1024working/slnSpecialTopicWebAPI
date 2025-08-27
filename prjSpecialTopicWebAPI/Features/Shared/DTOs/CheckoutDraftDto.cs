namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public class CheckoutDraftDto
    {
        public string ProductProvider { get; set; } = string.Empty;
        public string DeliveryOption { get; set; } = string.Empty;
        public string PaymentOption { get; set; } = string.Empty;
    }
}
