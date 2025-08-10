using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public record UpdatePartialBookSaleTagRequest
    {
        [Display(Name = "促銷標籤名稱")]
        [StringLength(10, ErrorMessage = "不可超過 10 字")]
        public string? Name { get; set; }

        [Display(Name = "啟用狀態")]
        public bool? IsActive { get; set; }

        [Display(Name = "資源網址")]
        [StringLength(255, ErrorMessage = "不可超過 255 字")]
        public string? Slug { get; set; }
    }
}
