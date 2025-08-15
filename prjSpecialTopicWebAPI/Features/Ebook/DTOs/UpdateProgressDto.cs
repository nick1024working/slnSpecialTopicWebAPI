namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class UpdateProgressDto
    {
        public long EbookId { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
