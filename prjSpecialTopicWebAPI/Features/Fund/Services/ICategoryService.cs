using prjSpecialTopicWebAPI.Features.Fund.Dtos;

namespace prjSpecialTopicWebAPI.Features.Fund.Services
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryDto>> GetPagedAsync(string? keyword, int page, int pageSize);
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CategoryCreateDto dto);
        Task<bool> UpdateAsync(int id, CategoryUpdateDto dto);
        Task<(bool ok, string? reason)> DeleteAsync(int id);
    }
}
