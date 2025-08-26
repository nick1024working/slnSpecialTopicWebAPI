using prjSpecialTopicWebAPI.Features.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    /// <summary>
    /// 更新或新增 CartItem 用，目前用於繫結前端來的請求。
    /// 若 Id 不存在當前購物車，將嘗試使用所有欄位進行新增。
    /// </summary>
    public sealed class UpsertCartItemRequest
    {
        [Required]
        public ProductProvider ProductProvider { get; set; }

        [Required]
        public string Id { get; set; } = string.Empty;

        public string? Name { get; set; }

        [Url]
        public string? ImageUrl { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? UnitPrice { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
