namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public class LinePayPaymentConfirmDto
    {
        public int Amount { get; set; }
        public string Currency { get; set; } = "TWD";
    }
}
