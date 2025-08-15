using prjSpecialTopicWebAPI.Features.Fund.Dtos;

namespace prjSpecialTopicWebAPI.Features.Fund.Services
{
    public interface IPlanService
    {
        Task<List<PlanDto>> GetByProjectAsync(int projectId);
        Task<PlanDto?> GetAsync(int id);
        Task<PlanDto> CreateAsync(PlanCreateDto dto);
        Task<bool> UpdateAsync(int id, PlanUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
