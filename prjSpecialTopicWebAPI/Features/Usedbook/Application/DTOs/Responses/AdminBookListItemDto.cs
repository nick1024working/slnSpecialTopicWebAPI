using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses
{
    public class AdminBookListItemDto
    {

        [Display(Name = "封面圖片URL")]
        public string CoverImageUrl { get; set; } = string.Empty;

        [Display(Name = "書本編號")]
        public Guid Id { get; set; }

        [Display(Name = "書名")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "賣家編號")]
        public Guid SellerId { get; set; }

        [Display(Name = "售價")]
        public decimal SalePrice { get; set; }

        [Display(Name = "上架")]
        public bool IsOnShelf { get; set; }

        [Display(Name = "啟用")]
        public bool IsActive { get; set; }

        [Display(Name = "售出")]
        public bool IsSold { get; set; }

        [Display(Name = "網址")]
        public string Slug { get; set; } = string.Empty;

        [Display(Name = "更新時間")]
        public DateTime UpdatedAt { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreatedAt { get; set; }

        // TODO:缺
        //public IEnumerable<BookSaleTagDto> SaleTags { get; set; } = [];
    }
}