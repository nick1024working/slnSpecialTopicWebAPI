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

        // [新增] 加入更多書籍詳細資料欄位
        public string? Isbn { get; set; }
        public string? Eisbn { get; set; }
        public DateOnly? PublishedDate { get; set; }
        public string? Language { get; set; }
        public string? Translator { get; set; }
        public string? EBookDataType { get; set; }

        public long TotalSales { get; set; } // [新增] 加入總銷量欄位
    }
}