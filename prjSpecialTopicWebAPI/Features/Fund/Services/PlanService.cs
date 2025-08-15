using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Fund.Dtos;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Fund.Services
{
    public class PlanService : IPlanService
    {
        private readonly TeamAProjectContext _db;
        public PlanService(TeamAProjectContext db) => _db = db;

        public async Task<List<PlanDto>> GetByProjectAsync(int projectId)
        {
            return await _db.Set<DonatePlan>().AsNoTracking()
                .Where(p => p.DonateProjectId == projectId)
                .OrderBy(p => p.Price)
                .Select(p => new PlanDto(
                    p.DonatePlanId,
                    p.DonateProjectId,
                    p.PlanTitle,
                    p.Price,
                    p.PlanDescription,
                    // 先找方案圖（donatePlan_id = 該方案）
                    _db.Set<DonateImage>()
                       .Where(i => i.DonatePlanId == p.DonatePlanId)
                       .OrderByDescending(i => i.IsMain == true).ThenBy(i => i.DonateImageId)
                       .Select(i => i.DonateImagePath)
                       .FirstOrDefault()
                    // 沒有方案圖 → 回退專案主圖/第一張
                    ?? _db.Set<DonateImage>()
                       .Where(i => i.DonateProjectId == p.DonateProjectId)
                       .OrderByDescending(i => i.IsMain == true).ThenBy(i => i.DonateImageId)
                       .Select(i => i.DonateImagePath)
                       .FirstOrDefault()
                ))
                .ToListAsync();
        }

        public async Task<PlanDto?> GetAsync(int id)
        {
            return await _db.Set<DonatePlan>().AsNoTracking()
                .Where(p => p.DonatePlanId == id)
                .Select(p => new PlanDto(
                    p.DonatePlanId,
                    p.DonateProjectId,
                    p.PlanTitle,
                    p.Price,
                    p.PlanDescription,
                    _db.Set<DonateImage>()
                       .Where(i => i.DonatePlanId == p.DonatePlanId)
                       .OrderByDescending(i => i.IsMain == true).ThenBy(i => i.DonateImageId)
                       .Select(i => i.DonateImagePath)
                       .FirstOrDefault()
                    ?? _db.Set<DonateImage>()
                       .Where(i => i.DonateProjectId == p.DonateProjectId)
                       .OrderByDescending(i => i.IsMain == true).ThenBy(i => i.DonateImageId)
                       .Select(i => i.DonateImagePath)
                       .FirstOrDefault()
                ))
                .FirstOrDefaultAsync();
        }

        public async Task<PlanDto> CreateAsync(PlanCreateDto dto)
        {
            // 確認專案存在，避免外鍵錯誤
            bool projectExists = await _db.Set<DonateProject>()
                .AnyAsync(p => p.DonateProjectId == dto.DonateProjectId);
            if (!projectExists) throw new KeyNotFoundException("DonateProject not found.");

            var entity = new DonatePlan
            {
                DonateProjectId = dto.DonateProjectId,
                PlanTitle = dto.PlanTitle,
                Price = dto.Price,
                PlanDescription = dto.PlanDescription
            };

            _db.Add(entity);
            await _db.SaveChangesAsync();
            var imagePath =
        await _db.Set<DonateImage>()
                 .Where(i => i.DonateProjectId == entity.DonateProjectId && i.IsMain == true)
                 .Select(i => i.DonateImagePath)
                 .FirstOrDefaultAsync()
        ?? await _db.Set<DonateImage>()
                    .Where(i => i.DonateProjectId == entity.DonateProjectId)
                    .Select(i => i.DonateImagePath)
                    .FirstOrDefaultAsync();

            // 回傳剛新增的資料（給 Swagger / 前端用）
            return new PlanDto(
                entity.DonatePlanId,
                entity.DonateProjectId,
                entity.PlanTitle,
                entity.Price,
                entity.PlanDescription,
                imagePath
            );
        }

        public async Task<bool> UpdateAsync(int id, PlanUpdateDto dto)
        {
            var entity = await _db.Set<DonatePlan>().FindAsync(id);
            if (entity == null) return false;

            if (dto.PlanTitle != null) entity.PlanTitle = dto.PlanTitle;
            if (dto.Price.HasValue) entity.Price = dto.Price.Value;
            if (dto.PlanDescription != null) entity.PlanDescription = dto.PlanDescription;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Set<DonatePlan>().FindAsync(id);
            if (entity == null) return false;
            _db.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}