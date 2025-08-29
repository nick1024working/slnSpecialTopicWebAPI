namespace prjSpecialTopicWebAPI.Features.Shared.Options
{
    public class LinePayOptions
    {
        public string BaseAddress { get; set; } = default!;
        public string ChannelId { get; set; } = default!;
        public string ChannelSecret { get; set; } = default!;
    }
}
