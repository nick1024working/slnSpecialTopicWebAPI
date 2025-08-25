namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses
{
    public class ImageFileDto
    {
        public string Id { get; set; } = string.Empty;
        public string MainUrl { get; set; } = string.Empty;
        public string ThumbUrl { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
