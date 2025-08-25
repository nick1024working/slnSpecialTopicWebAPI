namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses
{
    public class UpdateBookPayloadDto
    {
        public List<BookImageDto> ImageList { get; set; } = [];

        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public decimal SalePrice { get; set; }
        public int CategoryId { get; set; }

        public int ConditionRatingId { get; set; }
        public string? ConditionDescription { get; set; }

        public string? Edition { get; set; }
        public string? Publisher { get; set; }
        public DateOnly? PublicationDate { get; set; }
        public string? Isbn { get; set; }

        public int? BindingId { get; set; }
        public int? LanguageId { get; set; }
        public int? Pages { get; set; }
        public int ContentRatingId { get; set; }

        public int SellerCountyId { get; set; }
        public int SellerDistrictId { get; set; }

        public bool IsOnShelf { get; set; }
    }
}
