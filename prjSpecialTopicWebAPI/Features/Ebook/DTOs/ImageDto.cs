namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class ImageDto
    {
        public long ImageId { get; set; }
        public string ImagePath { get; set; } = null!;
        public bool IsPrimary { get; set; }
    }
}
