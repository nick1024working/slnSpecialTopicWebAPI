using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public class UpdateOrderByIdRequest
    {
        [Display(Name = "ID 清單")]
        [Required(ErrorMessage = "ID 清單為必填欄位")]
        public IList<int> IdList { get; set; } = [];
    }
}
