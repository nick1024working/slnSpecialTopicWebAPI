namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class PaginatedResponseDto<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        // 直接在屬性宣告時，給予一個空的 List 作為初始值
        public List<T> Items { get; set; } = new List<T>();
    }
}
