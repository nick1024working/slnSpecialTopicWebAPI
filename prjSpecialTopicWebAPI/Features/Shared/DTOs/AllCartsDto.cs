using prjSpecialTopicWebAPI.Features.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public class AllCartsDto
    {
        [Required]
        public Dictionary<ProductProvider, CartDto> Carts { get; set; } = new();

        [Required, Range(0, double.MaxValue)]
        public decimal GrandTotal { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
