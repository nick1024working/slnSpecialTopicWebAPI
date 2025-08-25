using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Enums
{
    /// <summary>
    /// 表示使用者性別 (Gender)。
    /// </summary>
    public enum Gender : byte
    {
        /// <summary>
        /// 未指定性別或使用者未提供 (預設值)
        /// </summary>
        [Display(Name = "未指定")]
        Unknown = 0,

        /// <summary>
        /// 男性
        /// </summary>
        [Display(Name = "男性")]
        Male = 1,

        /// <summary>
        /// 女性
        /// </summary>
        [Display(Name = "女性")]
        Female = 2,

        /// <summary>
        /// 其他性別身份
        /// </summary>
        [Display(Name = "其他")]
        Other = 3,

        /// <summary>
        /// 不願透露
        /// </summary>
        [Display(Name = "不願透露")]
        PreferNotToSay = 255
    }
}
