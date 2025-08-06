using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public record UpdatePartialBookCategoryRequest
    {
        [Display(Name = "主題類別名稱")]
        [StringLength(50, ErrorMessage = "不可超過 50 字")]
        public string? Name { get; set; }

        [Display(Name = "啟用狀態")]
        public bool? IsActive { get; set; }

        [Display(Name = "資源網址")]
        [StringLength(255, ErrorMessage = "不可超過 255 字")]
        public string? Slug { get; set; }
    }
}
