using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Enums
{
    /// <summary>
    /// 鎖定帳戶的原因 (Lockout Reason)。
    /// </summary>
    public enum LockoutReason : byte
    {
        /// <summary>
        /// 未指定原因 (預設值)
        /// </summary>
        [Display(Name = "未指定")]
        Unknown = 0,

        /// <summary>
        /// 登入失敗超過允許次數
        /// </summary>
        [Display(Name = "登入失敗次數過多")]
        ReachAttempts = 1,

        /// <summary>
        /// 管理員手動鎖定
        /// </summary>
        [Display(Name = "管理員鎖定")]
        AdminAction = 2,

        /// <summary>
        /// 檢測到可疑行為
        /// </summary>
        [Display(Name = "可疑行為")]
        SuspiciousActivity = 3,

        /// <summary>
        /// 違反使用政策
        /// </summary>
        [Display(Name = "違反使用政策")]
        PolicyViolation = 4,

        /// <summary>
        /// 其他原因
        /// </summary>
        [Display(Name = "其他")]
        Other = 255
    }
}