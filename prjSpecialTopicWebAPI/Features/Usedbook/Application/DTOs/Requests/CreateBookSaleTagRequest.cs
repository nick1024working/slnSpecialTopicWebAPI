using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public record CreateSaleTagRequest
    {
        [Display(Name = "促銷標籤名稱")]
        [Required(ErrorMessage = "促銷標籤名稱為必填欄位")]
        [StringLength(50, ErrorMessage = "不可超過 50 字")]
        public string SaleTagName { get; set; } = string.Empty;
    }
}
