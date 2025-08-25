using prjSpecialTopicWebAPI.Features.Fund.Dtos;

namespace prjSpecialTopicWebAPI.Features.Fund.Services
{
    public interface IfundImageService
    {
        Task<List<ImageDto>> GetByProjectAsync(int projectId);
        Task<ImageDto?> GetMainAsync(int projectId);
        Task<ImageDto> CreateAsync(ImageCreateDto dto);
        Task<ImageDto> CreateUploadedAsync(int projectId, string path, bool isMain);
        Task<bool> UpdateAsync(int id, ImageUpdateDto dto);
        Task<bool> SetMainAsync(int projectId, int imageId);
        Task<bool> DeleteAsync(int id);
    }
}
