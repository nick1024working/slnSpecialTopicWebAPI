namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results
{
    public class UsedBookDetailQueryResult
    {
        public Guid Id { get; set; }

        public Guid SellerId { get; set; }
        public string SellerCountyName { get; set; } = string.Empty;
        public string SellerDistrictName { get; set; } = string.Empty;
        public decimal SalePrice { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string ConditionRatingName { get; set; } = string.Empty;

        public string? ConditionDescription { get; set; }
        public string? Edition { get; set; }
        public string? Publisher { get; set; }
        public DateOnly? PublicationDate { get; set; }
        public string? Isbn { get; set; }
        public string? BindingName { get; set; }
        public string? LanguageName { get; set; }
        public int? Pages { get; set; }
        public string ContentRatingName { get; set; } = string.Empty;

        /* --- 狀態欄位 --- */
        public bool IsOnShelf { get; set; }
        public bool IsSold { get; set; }
        public bool IsActive { get; set; }
        public string Slug { get; set; } = string.Empty;

        /* --- 審計欄位 --- */
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
