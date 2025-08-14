using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public record UpdateOrderByIdRequest
    {
        [Display(Name = "ID")]
        [Required(ErrorMessage = "ID 為必填欄位")]
        public int Id { get; set; }
    }
}
