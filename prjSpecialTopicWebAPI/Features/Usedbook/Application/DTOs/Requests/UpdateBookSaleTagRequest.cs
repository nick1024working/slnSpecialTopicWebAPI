using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public record UpdateBookSaleTagRequest
    {
        [Display(Name = "促銷標籤 ID")]
        public int? Id { get; set; }

        [Display(Name = "促銷標籤名稱")]
        [StringLength(50, ErrorMessage = "不可超過 50 字")]
        [Required(ErrorMessage = "促銷標籤名稱為必填欄位")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "啟用狀態")]
        [Required(ErrorMessage = "啟用狀態為必填欄位")]
        public bool IsActive { get; set; }
    }
}
