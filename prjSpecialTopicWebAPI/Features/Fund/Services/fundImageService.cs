using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Fund.Dtos;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Fund.Services
{
    public class fundImageService : IfundImageService
    {
        private readonly TeamAProjectContext _db;

        public fundImageService(TeamAProjectContext db) => _db = db;

        public async Task<List<ImageDto>> GetByProjectAsync(int projectId)
        {
            return await _db.DonateImages
                .Where(i => i.DonateProjectId == projectId)
                .OrderByDescending(i => i.IsMain == true)
                .ThenBy(i => i.DonateImageId)
                .Select(i => new ImageDto(
                    i.DonateImageId,
                    i.DonateProjectId,
                    i.DonateImagePath,
                    i.IsMain,
                    i.ProjectGalleryPath))
                .ToListAsync();
        }

        public async Task<ImageDto?> GetMainAsync(int projectId)
        {
            var main = await _db.DonateImages
                .Where(i => i.DonateProjectId == projectId && i.IsMain == true)
                .OrderBy(i => i.DonateImageId)
                .FirstOrDefaultAsync();

            return main == null ? null :
                new ImageDto(main.DonateImageId, main.DonateProjectId, main.DonateImagePath, main.IsMain, main.ProjectGalleryPath);
        }

        // 直接用路徑新增
        public async Task<ImageDto> CreateAsync(ImageCreateDto dto)
        {
            var projectExists = await _db.DonateProjects
                .AnyAsync(p => p.DonateProjectId == dto.DonateProjectId && !p.IsDeleted);
            if (!projectExists) throw new InvalidOperationException("Project not found.");

            if (string.IsNullOrWhiteSpace(dto.DonateImagePath) && string.IsNullOrWhiteSpace(dto.ProjectGalleryPath))
                throw new InvalidOperationException("請提供路徑。");

            // DB NOT NULL：即便是相簿也要給 DonateImagePath
            var entity = new DonateImage
            {
                DonateProjectId = dto.DonateProjectId,
                DonateImagePath = dto.DonateImagePath ?? dto.ProjectGalleryPath!,
                ProjectGalleryPath = dto.ProjectGalleryPath,
                IsMain = dto.IsMain ?? (dto.DonateImagePath != null)
            };

            if (entity.IsMain == true)
            {
                await _db.DonateImages
                    .Where(i => i.DonateProjectId == dto.DonateProjectId && i.IsMain == true)
                    .ExecuteUpdateAsync(up => up.SetProperty(x => x.IsMain, false));
            }

            _db.DonateImages.Add(entity);
            await _db.SaveChangesAsync();

            return new ImageDto(entity.DonateImageId, entity.DonateProjectId, entity.DonateImagePath, entity.IsMain, entity.ProjectGalleryPath);
        }

        // 上傳檔案後的新增
        public async Task<ImageDto> CreateUploadedAsync(int projectId, string path, bool isMain)
        {
            var projectExists = await _db.DonateProjects
                .AnyAsync(p => p.DonateProjectId == projectId && !p.IsDeleted);
            if (!projectExists) throw new InvalidOperationException("Project not found.");

            var entity = new DonateImage
            {
                DonateProjectId = projectId,
                DonateImagePath = path,                 // ★ 永遠給值（DB NOT NULL）
                ProjectGalleryPath = isMain ? null : path, // 相簿仍保留在 Gallery 欄位
                IsMain = isMain
            };

            if (isMain)
            {
                await _db.DonateImages
                    .Where(i => i.DonateProjectId == projectId && i.IsMain == true)
                    .ExecuteUpdateAsync(up => up.SetProperty(x => x.IsMain, false));
            }

            _db.DonateImages.Add(entity);
            await _db.SaveChangesAsync();

            return new ImageDto(entity.DonateImageId, entity.DonateProjectId, entity.DonateImagePath, entity.IsMain, entity.ProjectGalleryPath);
        }

        public async Task<bool> UpdateAsync(int id, ImageUpdateDto dto)
        {
            var entity = await _db.DonateImages.FindAsync(id);
            if (entity == null) return false;

            entity.DonateImagePath = dto.DonateImagePath ?? entity.DonateImagePath;
            entity.ProjectGalleryPath = dto.ProjectGalleryPath ?? entity.ProjectGalleryPath;
            entity.IsMain = dto.IsMain ?? entity.IsMain;

            if (entity.IsMain == true)
            {
                await _db.DonateImages
                    .Where(i => i.DonateProjectId == entity.DonateProjectId && i.DonateImageId != id && i.IsMain == true)
                    .ExecuteUpdateAsync(up => up.SetProperty(x => x.IsMain, false));
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetMainAsync(int projectId, int imageId)
        {
            var exist = await _db.DonateImages
                .AnyAsync(i => i.DonateImageId == imageId && i.DonateProjectId == projectId);
            if (!exist) return false;

            await _db.DonateImages
                .Where(i => i.DonateProjectId == projectId)
                .ExecuteUpdateAsync(up => up.SetProperty(x => x.IsMain, false));

            await _db.DonateImages
                .Where(i => i.DonateImageId == imageId)
                .ExecuteUpdateAsync(up => up.SetProperty(x => x.IsMain, true));

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.DonateImages.FindAsync(id);
            if (entity == null) return false;
            _db.DonateImages.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}