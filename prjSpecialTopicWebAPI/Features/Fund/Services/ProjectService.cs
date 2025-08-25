using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Fund.Dtos;
using prjSpecialTopicWebAPI.Models;
using System.Linq;

namespace prjSpecialTopicWebAPI.Features.Fund.Services
{
    public class ProjectService : IProjectService
    {
        private readonly TeamAProjectContext _db;
        public ProjectService(TeamAProjectContext db) => _db = db;
        public async Task<PagedResult<ProjectListDto>> GetListAsync(
            string? status, int? categoryId, string? keyword, int page, int pageSize)
        {
            var q = _db.DonateProjects.Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(status)) q = q.Where(p => p.Status == status);
            if (categoryId.HasValue) q = q.Where(p => p.DonateCategoriesId == categoryId.Value);
            if (!string.IsNullOrWhiteSpace(keyword)) q = q.Where(p => p.ProjectTitle.Contains(keyword));

            var total = await q.CountAsync();

            var items = await q
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(p => new ProjectListDto(
                    p.DonateProjectId,
                    p.ProjectTitle,
                    p.ProjectDescription,
                    p.TargetAmount,
                    p.CurrentAmount,
                    p.BackerCount ??0,
                    new DateTime(p.StartDate.Year, p.StartDate.Month, p.StartDate.Day),
                    new DateTime(p.EndDate.Year, p.EndDate.Month, p.EndDate.Day),
                    p.Status,
                    // MainImagePath：is_main=1 的 donateImagePath；沒有就第一張
                    p.DonateImages
                        .Where(i => i.IsMain == true)
                        .Select(i => i.DonateImagePath)
                        .FirstOrDefault()
                        ?? p.DonateImages
                             .OrderBy(i => i.DonateImageId)
                             .Select(i => i.DonateImagePath)
                             .FirstOrDefault(),
                    // IsFavorite
                    p.ProjectIsFavorite == true
                ))
                .ToListAsync();

            return new PagedResult<ProjectListDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                Items = items
            };
        }

        public async Task<ProjectDetailDto?> GetDetailAsync(int id)
        {
            return await _db.DonateProjects
                .Where(p => p.DonateProjectId == id && !p.IsDeleted)
                .Select(p => new ProjectDetailDto(
                    p.DonateProjectId,
                    p.ProjectTitle,
                    p.ProjectDescription,
                    p.TargetAmount,
                    p.CurrentAmount,
                    p.BackerCount ??0,
                    new DateTime(p.StartDate.Year, p.StartDate.Month, p.StartDate.Day),
                    new DateTime(p.EndDate.Year, p.EndDate.Month, p.EndDate.Day),
                    p.Status,
                    p.DonateImages
                        .Where(i => i.IsMain == true)
                        .Select(i => i.DonateImagePath)
                        .FirstOrDefault()
                        ?? p.DonateImages
                             .OrderBy(i => i.DonateImageId)
                             .Select(i => i.DonateImagePath)
                             .FirstOrDefault(),
                    p.ProjectIsFavorite == true
                )
                {
                    LongDescription = p.ProjectLongDescription,
                    // Gallery：優先取 projectGalleryPath；再補上非主圖的 donateImagePath
                    Gallery = p.DonateImages
            .Where(i => i.IsMain != true)
            .Select(i => i.ProjectGalleryPath ?? i.DonateImagePath)
            .ToList(),
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ProjectDetailDto> CreateAsync(ProjectCreateDto dto)
        {
            var now = DateTime.UtcNow;

            var entity = new DonateProject
            {
                DonateCategoriesId = dto.DonateCategoriesId,
                Uid = dto.UID, // 若你的屬性叫 UID，請改為 UID = dto.UID
                ProjectTitle = dto.ProjectTitle,
                ProjectDescription = dto.ProjectDescription,
                TargetAmount = dto.TargetAmount,
                CurrentAmount = 0,
                BackerCount = 0,
                StartDate = DateOnly.FromDateTime(dto.StartDate),
                EndDate = DateOnly.FromDateTime(dto.EndDate),
                Status = "募資中",
                IsDeleted = false,
                CreatedAt = now,
                UpdatedAt = now,
                ProjectLongDescription = dto.LongDescription,
                ProjectIsFavorite = dto.IsFavorite ?? false
            };

            _db.DonateProjects.Add(entity);
            await _db.SaveChangesAsync();

            // 建主圖
            if (!string.IsNullOrWhiteSpace(dto.MainImagePath))
            {
                _db.DonateImages.Add(new DonateImage
                {
                    DonateProjectId = entity.DonateProjectId,
                    DonateImagePath = dto.MainImagePath!,
                    IsMain = true
                });
            }

            // 建相簿
            if (dto.Gallery != null)
            {
                foreach (var path in dto.Gallery.Where(p => !string.IsNullOrWhiteSpace(p)))
                {
                    _db.DonateImages.Add(new DonateImage
                    {
                        DonateProjectId = entity.DonateProjectId,
                        ProjectGalleryPath = path!,
                        IsMain = false
                    });
                }
            }

            await _db.SaveChangesAsync();
            return (await GetDetailAsync(entity.DonateProjectId))!;
        }

        public async Task<bool> UpdateAsync(int id, ProjectUpdateDto dto)
        {
            var p = await _db.DonateProjects
                .FirstOrDefaultAsync(x => x.DonateProjectId == id && !x.IsDeleted);

            if (p is null) return false;

            p.DonateCategoriesId = dto.DonateCategoriesId;
            p.ProjectTitle = dto.ProjectTitle;
            p.ProjectDescription = dto.ProjectDescription;
            p.TargetAmount = dto.TargetAmount;
            p.StartDate = DateOnly.FromDateTime(dto.StartDate);
            p.EndDate = DateOnly.FromDateTime(dto.EndDate);
            p.ProjectLongDescription = dto.LongDescription;
            if (dto.IsFavorite.HasValue) p.ProjectIsFavorite = dto.IsFavorite.Value;
            p.UpdatedAt = DateTime.UtcNow;

            //（如需更新主圖/相簿，這裡可另外寫替換邏輯）
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeStatusAsync(int id, string status)
        {
            var p = await _db.DonateProjects
                .FirstOrDefaultAsync(x => x.DonateProjectId == id && !x.IsDeleted);

            if (p is null) return false;

            p.Status = status;
            p.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var p = await _db.DonateProjects
                .FirstOrDefaultAsync(x => x.DonateProjectId == id && !x.IsDeleted);

            if (p is null) return false;

            p.IsDeleted = true;
            p.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
