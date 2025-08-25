namespace prjSpecialTopicWebAPI.Features.Fund.Dtos
{
    public record PlanDto(
         int DonatePlanId,
         int DonateProjectId,
         string PlanTitle,
         decimal Price,
         string? PlanDescription,
         string? PlanImagePath
     );
}
