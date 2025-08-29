using prjSpecialTopicWebAPI.Features.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public sealed class UpdateDeliveryRequest
    {
        [Required]
        public ProductProvider ProductProvider { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DeliveryFee { get; set; }
    }
}
