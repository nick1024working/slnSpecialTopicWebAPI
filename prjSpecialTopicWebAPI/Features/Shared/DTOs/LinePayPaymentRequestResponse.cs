using prjSpecialTopicWebAPI.Features.Shared.Converter;
using System.Text.Json.Serialization;

namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public class LinePayRequestResponseDto
    {
        public string ReturnCode { get; set; } = string.Empty;
        public string ReturnMessage { get; set; } = string.Empty;
        public LinePayRequestInfo? Info { get; set; }
    }

    public class LinePayRequestInfo
    {
        public LinePayPaymentUrl? PaymentUrl { get; set; }

        [JsonPropertyName("transactionId")]
        [JsonConverter(typeof(TxIdAsStringConverter))]
        public string TransactionId { get; set; } = string.Empty;

        public string? PaymentAccessToken { get; set; }
    }

    public class LinePayPaymentUrl
    {
        public string? Web { get; set; }
        public string? App { get; set; }
    }

}