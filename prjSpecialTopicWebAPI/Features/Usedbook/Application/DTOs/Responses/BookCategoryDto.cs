namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses
{
    public record BookCategoryDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
