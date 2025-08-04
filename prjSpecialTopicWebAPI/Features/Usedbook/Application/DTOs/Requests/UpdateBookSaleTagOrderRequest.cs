using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public record UpdateBookSaleTagOrderRequest
    {
        [Display(Name = "促銷標籤 ID")]
        [Required(ErrorMessage = "促銷標籤 ID 為必填欄位")]
        public int Id { get; set; }
    }
}
