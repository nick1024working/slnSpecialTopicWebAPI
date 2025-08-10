using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public record UpdateStatusRequest
    {
        [Display(Name = "狀態布林值")]
        [Required(ErrorMessage = "狀態布林值為必填欄位")]
        public bool Value { get; set; }
    }
}
