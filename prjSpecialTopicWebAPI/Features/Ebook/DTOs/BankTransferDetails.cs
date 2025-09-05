namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class BankTransferDetails
    {
        public string OrderId { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string AccountNumber { get; set; }
        public int Amount { get; set; }
        public string PaymentDeadline { get; set; }
    }
}
