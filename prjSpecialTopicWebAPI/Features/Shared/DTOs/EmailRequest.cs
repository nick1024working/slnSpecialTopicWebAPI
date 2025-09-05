namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public class EmailRequest
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Arg1 { get; set; } = string.Empty;
        public string Arg2 { get; set; } = string.Empty;
    }
}
