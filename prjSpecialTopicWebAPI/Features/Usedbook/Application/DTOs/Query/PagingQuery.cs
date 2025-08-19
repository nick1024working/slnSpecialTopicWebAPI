using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query
{
    public sealed class PagingQuery
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int PageIndex { get; init; } = 0;

        [Required]
        [Range(1, 100)]
        public int PageSize { get; init; } = 20;

        [RegularExpression("updated|created|price")]
        public string? SortBy { get; init; } = "updated";

        [RegularExpression("asc|desc")]
        public string? SortDir { get; init; } = "desc";
    }
}
