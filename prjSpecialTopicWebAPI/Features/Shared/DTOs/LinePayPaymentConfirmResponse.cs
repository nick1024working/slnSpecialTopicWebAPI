namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public class LinePayPaymentConfirmResponse
    {
        public string ReturnCode { get; set; } = string.Empty;
        public string ReturnMessage { get; set; } = string.Empty;
        public LinePayResponseInfo? Info { get; set; }
    }

    public class LinePayResponseInfo
    {
        public string OrderId { get; set; } = string.Empty;
        public long TransactionId { get; set; }
        public List<LinePayPayInfo> PayInfo { get; set; } = new();
    }

    public class LinePayPayInfo
    {
        public string Method { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
