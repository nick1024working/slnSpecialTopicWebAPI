using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Fund.Dtos
{
    // 只存路徑用（非檔案上傳）
    public class ImageCreateDto
    {
        [Required]
        public int DonateProjectId { get; set; }

        // 二選一：主圖或相簿擇一提供
        public string? DonateImagePath { get; set; }      // 主圖路徑
        public string? ProjectGalleryPath { get; set; }   // 相簿路徑
        public bool? IsMain { get; set; }                 // 主圖請設 true
    }
}
