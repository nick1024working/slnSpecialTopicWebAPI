using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public class SetBookCoverRequest
    {
        // HACK: 最好自訂 ValidationAttribute
        [Display(Name = "圖片 ID")]
        [Required(ErrorMessage = "圖片 ID 為必填欄位")]
        [Range(0, 100)]
        public int ImageId { get; set; }
    }
}
