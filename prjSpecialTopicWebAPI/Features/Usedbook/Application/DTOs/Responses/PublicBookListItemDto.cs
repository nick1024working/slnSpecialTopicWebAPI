namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses
{
    public class PublicBookListItemDto
    {
        public string CoverImageUrl { get; set; } = string.Empty;

        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public decimal SalePrice { get; set; }
        public string ConditionRating { get; set; } = string.Empty;

        public required IdNameDto Category { get; set; }
        public IEnumerable<IdNameDto> SaleTagList { get; set; } = [];

        public string Slug { get; set; } = string.Empty;
    }
}
