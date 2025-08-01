using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public record UpdateBookSaleTagRequest
    {
        [Display(Name = "促銷標籤名稱")]
        [Required(ErrorMessage = "促銷標籤名稱為必填欄位")]
        [StringLength(50, ErrorMessage = "不可超過 50 字")]
        public string SaleTagName { get; set; } = string.Empty;

        [Display(Name = "顯示順序")]
        [Range(1, int.MaxValue, ErrorMessage = "顯示順序必須大於等於 1")]
        [Required(ErrorMessage = "顯示順序為必填欄位")]
        public int DisplayOrder { get; set; }
    }
}
