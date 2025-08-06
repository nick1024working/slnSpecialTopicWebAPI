using prjSpecialTopicWebAPI.Features.Usedbook.Enums;
using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    // NOTE: 
    // 當無 Id 時，表示新增書圖片；
    // 當有 Id 時，表示更新書圖片 IsCover 或 ImageIndex。
    [Obsolete]
    public class UpdateUsedBookImageRequest
    {
        [Display(Name = "圖片 ID")]
        public int? Id { get; set; }

        [Display(Name = "是否為封面圖片")]
        public bool? IsCover { get; set; }

        [Display(Name = "儲存服務")]
        [Required(ErrorMessage = "儲存服務為必填欄位")]
        [EnumDataType(typeof(StorageProvider))]
        public StorageProvider StorageProvider { get; set; }

        [Display(Name = "儲存服務的檔案識別鍵")]
        [Required(ErrorMessage = "儲存服務的檔案識別鍵為必填欄位")]
        [MaxLength(300)]
        public string ObjectKey { get; set; } = string.Empty;

        //[Display(Name = "檔案 SHA256（Base64 格式）")]
        //[Required]
        //[RegularExpression(@"^[A-Za-z0-9+/]{43}[A-Za-z0-9+/=]{1}$")]
        //public string Sha256 { get; set; } = string.Empty;
    }
}
