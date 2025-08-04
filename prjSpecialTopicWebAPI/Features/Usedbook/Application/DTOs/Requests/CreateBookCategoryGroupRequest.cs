using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public record CreateBookCategoryGroupRequest
    {
        [Display(Name = "主題類別群名稱")]
        [Required(ErrorMessage = "主題分類群名稱為必填欄位")]
        [StringLength(50, ErrorMessage = "不可超過 50 字")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "啟用狀態")]
        [Required(ErrorMessage = "啟用狀態為必填欄位")]
        public bool IsActive { get; set; }
    }
}
