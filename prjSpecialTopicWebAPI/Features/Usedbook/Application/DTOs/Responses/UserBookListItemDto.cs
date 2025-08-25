namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses
{
    public class UserBookListItemDto
    {
        public string CoverImageUrl { get; set; } = string.Empty;

        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal SalePrice { get; set; }
        public string ConditionRating { get; set; } = string.Empty;

        public bool IsOnShelf { get; set; }
        public bool IsSold { get; set; }
        public string Slug { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
