namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses
{
    public record BookSaleTagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
