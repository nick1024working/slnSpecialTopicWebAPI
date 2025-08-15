using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Fund.Dtos
{
    public class PlanUpdateDto
    {
        [MaxLength(50)] public string? PlanTitle { get; set; }
        [Range(0, double.MaxValue)] public decimal? Price { get; set; }
        public string? PlanDescription { get; set; }
    }
}
