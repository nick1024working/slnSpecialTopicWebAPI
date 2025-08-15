using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public record UpdateOrderByIdRequest
    {
        [Display(Name = "ID 清單")]
        [Required(ErrorMessage = "ID 清單為必填欄位")]
        public IReadOnlyList<int> IdList { get; set; } = [];
    }
}
