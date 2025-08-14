// 檔案路徑: Features/Ebook/DTOs/EBookDetailDto.cs
namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class EBookDetailDto
    {
        public long EbookId { get; set; }
        public string EbookName { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string? Publisher { get; set; }
        public string? BookDescription { get; set; }
        public decimal FixedPrice { get; set; }
        public decimal? ActualPrice { get; set; }
        public string CategoryName { get; set; } = null!;
        public List<string> Labels { get; set; } = new List<string>();

        // [請確認或加入這兩行]
        public string? PrimaryCoverPath { get; set; }
        public List<string> ImagePaths { get; set; } = new List<string>();
    }
}