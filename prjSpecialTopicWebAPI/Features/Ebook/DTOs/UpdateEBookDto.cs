using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class UpdateEBookDto
    {
        [Required]
        [MaxLength(200)]
        public string EbookName { get; set; } = null!;

        [Required]
        public string Author { get; set; } = null!;

        public string? Publisher { get; set; }
        public string? BookDescription { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal FixedPrice { get; set; }

        [Required]
        public int CategoryId { get; set; }

        // 在更新時，也允許重新設定整本書的標籤
        public List<int>? LabelIds { get; set; }

        [Required]
        public bool IsAvailable { get; set; } // 更新時可以決定是否上架
    }
}
