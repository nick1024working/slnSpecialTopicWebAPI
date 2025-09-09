namespace prjSpecialTopicWebAPI.Features.Shared.Options
{
    public class PasswordResetOptions
    {
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public string Secret { get; set; } = "";         // 超過 32 字、很亂的 key
        public int ExpireMinutes { get; set; } = 15;     // 例如 15 分鐘
        public string FrontBaseUrl { get; set; } = "http://localhost:4200"; 
    }
}