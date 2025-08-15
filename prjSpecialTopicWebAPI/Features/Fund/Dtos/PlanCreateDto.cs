using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Fund.Dtos
{
    public class PlanCreateDto
    {
        [Required] public int DonateProjectId { get; set; }
        [Required, MaxLength(50)] public string PlanTitle { get; set; } = null!;
        [Range(0, double.MaxValue)] public decimal Price { get; set; }
        public string? PlanDescription { get; set; }
    }

}
