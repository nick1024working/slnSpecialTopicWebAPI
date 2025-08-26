namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public class PaymentRawResult
    {
        public int StatusCode { get; init; }
        public string RawBody { get; init; } = string.Empty;
    }
}
