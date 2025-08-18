using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    // NOTE: 
    // 當無 Id 時，表示新增書圖片；
    // 當有 Id 時，表示僅更新。
    public class UpdateUsedBookImageRequest
    {
        [Display(Name = "圖片 ID")]
        public int? Id { get; set; }

        [Display(Name = "圖片檔案")]
        public IFormFile? Image { get; set; }
    }
}
