using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Enums
{
    /// <summary>
    /// 系統角色 (Role) 定義。
    /// </summary>
    public enum Role : byte
    {
        /// <summary>
        /// 一般使用者。
        /// </summary>
        [Display(Name = "一般使用者")]
        User = 0,

        /// <summary>
        /// 賣家。
        /// </summary>
        [Display(Name = "賣家")]
        Seller = 1,

        /// <summary>
        /// 系統管理員。
        /// </summary>
        [Display(Name = "系統管理員")]
        Admin = 9
    }
}
