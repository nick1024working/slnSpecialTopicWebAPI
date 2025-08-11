using prjSpecialTopicWebAPI.Features.Usedbook.Enums;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results
{
    public class UserBookListItemQueryResult
    {
        public StorageProvider CoverStorageProvider { get; set; }
        public string CoverObjectKey { get; set; } = string.Empty;

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
