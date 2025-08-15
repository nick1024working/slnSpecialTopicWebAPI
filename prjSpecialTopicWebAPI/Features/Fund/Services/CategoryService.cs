using prjSpecialTopicWebAPI.Features.Fund.Dtos;
using prjSpecialTopicWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace prjSpecialTopicWebAPI.Features.Fund.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly TeamAProjectContext _db;
        public CategoryService(TeamAProjectContext db) => _db = db;

        public async Task<PagedResult<CategoryDto>> GetPagedAsync(string? keyword, int page, int pageSize)
        {
            var q = _db.DonateCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                q = q.Where(c => c.CategoriesName.Contains(keyword));

            var total = await q.CountAsync();

            var items = await q.OrderBy(c => c.CategoriesName)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .Select(c => new CategoryDto(c.DonateCategoriesId, c.CategoriesName))
                               .ToListAsync();

            return new PagedResult<CategoryDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                Items = items
            };
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _db.DonateCategories
                .OrderBy(c => c.CategoriesName)
                .Select(c => new CategoryDto(c.DonateCategoriesId, c.CategoriesName))
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            return await _db.DonateCategories
                .Where(c => c.DonateCategoriesId == id)
                .Select(c => new CategoryDto(c.DonateCategoriesId, c.CategoriesName))
                .FirstOrDefaultAsync();
        }

        public async Task<CategoryDto> CreateAsync(CategoryCreateDto dto)
        {
            // 名稱重複檢查（多半 DB 預設為不區分大小寫 Collation）
            var exists = await _db.DonateCategories
                .AnyAsync(c => c.CategoriesName == dto.CategoriesName);
            if (exists) throw new InvalidOperationException("分類名稱已存在。");

            var entity = new DonateCategory
            {
                CategoriesName = dto.CategoriesName
            };

            _db.DonateCategories.Add(entity);
            await _db.SaveChangesAsync();

            return new CategoryDto(entity.DonateCategoriesId, entity.CategoriesName);
        }

        public async Task<bool> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var cat = await _db.DonateCategories.FirstOrDefaultAsync(c => c.DonateCategoriesId == id);
            if (cat is null) return false;

            var dup = await _db.DonateCategories
                .AnyAsync(c => c.DonateCategoriesId != id && c.CategoriesName == dto.CategoriesName);
            if (dup) throw new InvalidOperationException("分類名稱已存在。");

            cat.CategoriesName = dto.CategoriesName;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(bool ok, string? reason)> DeleteAsync(int id)
        {
            var cat = await _db.DonateCategories.FirstOrDefaultAsync(c => c.DonateCategoriesId == id);
            if (cat is null) return (false, "NotFound");

            // 若仍有專案使用這個分類，阻擋刪除（避免 FK 例外）
            var inUse = await _db.DonateProjects.AnyAsync(p => p.DonateCategoriesId == id && !p.IsDeleted);
            if (inUse) return (false, "InUse");

            _db.DonateCategories.Remove(cat);
            await _db.SaveChangesAsync();
            return (true, null);
        }
    }
}