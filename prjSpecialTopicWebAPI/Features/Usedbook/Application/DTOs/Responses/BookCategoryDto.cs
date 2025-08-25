namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses
{
    public record BookCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Slug { get; set; } = string.Empty;
    }
}
