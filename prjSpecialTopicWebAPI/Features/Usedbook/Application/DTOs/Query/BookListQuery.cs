using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query
{
    /// <summary>
    /// 查詢書籍清單 (PLP) 的條件。
    /// </summary>
    public class BookListQuery
    {
        /// <summary>書本狀態過濾條件（all, onshelf, unsold）。</summary>
        [RegularExpression("all|onshelf|unsold")]
        public string BookStatus { get; init; } = "all";

        /// <summary>關鍵字搜尋 (書名 / 作者 / ISBN)。</summary>
        public string? Keyword { get; init; }

        /// <summary>主分類 ID；若為 null 表示全部。</summary>
        public int? CategoryId { get; init; }

        /// <summary>多重標籤 (tag) 篩選。以半形逗號分隔，如 &quot;1,2,3&quot;。</summary>
        public IReadOnlyList<int>? SaleTagIds { get; init; }

        /// <summary>價格下限。</summary>
        [Range(0, 999_999)]
        public decimal? MinPrice { get; init; }

        /// <summary>價格上限。</summary>
        [Range(0, 999_999)]
        public decimal? MaxPrice { get; init; }

        /// <summary>排序欄位。</summary>
        [RegularExpression("updated|created|price")]
        public string SortBy { get; init; } = "updated";

        /// <summary>排序方向。</summary>
        [RegularExpression("asc|desc")]
        public string SortDir { get; init; } = "desc";

        /// <summary>頁碼 (從 1 開始)。</summary>
        //[Range(1, int.MaxValue)]
        //public int Page { get; init; } = 1;

        /// <summary>每頁筆數。</summary>
        //[Range(1, 100)]
        //public int PageSize { get; init; } = 20;
    }
}
