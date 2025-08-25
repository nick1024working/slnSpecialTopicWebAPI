using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public record UpdateBookSaleTagRequest
    {
        [Display(Name = "書本 ID 清單")]
        [Required(ErrorMessage = "書本 ID 清單為必填欄位")]
        public List<Guid> BookIdList { get; set; } = [];

        [Display(Name = "促銷標籤 ID")]
        [Required(ErrorMessage = "促銷標籤 ID 為必填欄位")]
        public int TagId { get; set; }

        [Display(Name = "指派狀態")]
        [Required(ErrorMessage = "指派狀態為必填欄位")]
        public bool IsApply { get; set; }
    }
}
