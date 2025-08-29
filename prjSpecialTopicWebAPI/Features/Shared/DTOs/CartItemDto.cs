using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public class CartItemDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, Url]
        public string ImageUrl { get; set; } = string.Empty;

        [Required, Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
