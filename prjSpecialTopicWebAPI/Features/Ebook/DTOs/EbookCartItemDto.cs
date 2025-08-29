namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class EbookCartItemDto
    {
        public long EbookId { get; set; }
     //   public string EbookName { get; set; } = null!;
        public int Quantity { get; set; }
 //       public decimal Price { get; set; }
    //    public string? PrimaryCoverPath { get; set; } // 這個欄位會被自動忽略

    }
}
