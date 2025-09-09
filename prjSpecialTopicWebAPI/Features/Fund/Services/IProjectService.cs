using prjSpecialTopicWebAPI.Features.Fund.Dtos;

namespace prjSpecialTopicWebAPI.Features.Fund.Services
{
    public interface IProjectService
    {
        Task<PagedResult<ProjectListDto>> GetListAsync(string? status, int? categoryId, string? keyword, int page, int pageSize);
        Task<ProjectDetailDto?> GetDetailAsync(int id);
        Task<ProjectDetailDto> CreateAsync(ProjectCreateDto dto, Guid uid);
        Task<bool> UpdateAsync(int id, ProjectUpdateDto dto);
        Task<bool> ChangeStatusAsync(int id, string status);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<IReadOnlyList<ProjectListDto>> GetMineAsync(Guid uid, bool includeDeleted = true);
    }
}
