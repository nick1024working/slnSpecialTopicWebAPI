using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses
{
    public class AdminBookListItemDto
    {

        public string CoverImageUrl { get; set; } = string.Empty;

        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid SellerId { get; set; }
        public decimal SalePrice { get; set; }

        public bool IsOnShelf { get; set; }
        public bool IsActive { get; set; }
        public bool IsSold { get; set; }

        public string Slug { get; set; } = string.Empty;

        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public IEnumerable<IdNameDto> SaleTagList { get; set; } = [];
    }
}