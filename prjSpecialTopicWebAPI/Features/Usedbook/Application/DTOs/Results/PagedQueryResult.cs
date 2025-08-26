namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results
{
    public sealed class PagedResult<T>
    {
        public required IReadOnlyList<T> Items { get; init; }
        public required int PageIndex { get; init; }
        public required int PageSize { get; init; }
        public required int TotalRows { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRows / Math.Max(PageSize, 1));
        public bool HasNextPage => (PageIndex + 1) * PageSize < TotalRows;
    }
}
