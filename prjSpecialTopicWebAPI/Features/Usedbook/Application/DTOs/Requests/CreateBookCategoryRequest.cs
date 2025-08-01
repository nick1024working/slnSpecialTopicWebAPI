using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public record CreateBookCategoryRequest
    {
        [Display(Name = "主題類別名稱")]
        [Required(ErrorMessage = "主題類別名稱為必填欄位")]
        [StringLength(50, ErrorMessage = "不可超過 50 字")]
        public string CategoryName { get; set; } = string.Empty;
    }
}
