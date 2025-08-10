using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class CreateEBookDto
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

        public List<int>? LabelIds { get; set; }
    }
}
