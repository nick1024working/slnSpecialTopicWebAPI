namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class RankingBookDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string? CoverImage { get; set; }
        public int? Price { get; set; }

        // [新增] 可選的定價屬性，用於前端計算折扣
        public int? FixedPrice { get; set; }
    }
}
