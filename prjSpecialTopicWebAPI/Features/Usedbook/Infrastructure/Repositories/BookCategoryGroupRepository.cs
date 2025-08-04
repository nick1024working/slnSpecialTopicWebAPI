using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    public class BookCategoryGroupRepository
    {
        private readonly TeamAProjectContext _db;

        public BookCategoryGroupRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        // ========== 通用 ==========

        public async Task<bool> HasRecords(CancellationToken ct = default) => await _db.BookCategoryGroups.AnyAsync(ct);

        public async Task<int> GetMaxDisplayOrderAsync(CancellationToken ct = default) =>
            await _db.BookCategoryGroups.AsNoTracking().MaxAsync(st => st.DisplayOrder, ct);

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default) =>
            await _db.BookCategoryGroups.AsNoTracking().AnyAsync(cg => cg.Name == name);

        // ========== 查詢實體 ==========

        public async Task<BookCategoryGroup?> GetEntityByIdAsync(int id, CancellationToken ct = default) =>
            await _db.BookCategoryGroups.FirstOrDefaultAsync(cg => cg.Id == id, ct);

        public async Task<IReadOnlyList<BookCategoryGroup>> GetEntityListAsync(CancellationToken ct = default) =>
            await _db.BookCategoryGroups.ToListAsync(ct);

        // ========== 新增、更新、刪除 ==========

        public void Add(BookCategoryGroup entity) =>
            _db.BookCategoryGroups.Add(entity);

        public async Task<bool> RemoveByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.BookCategoryGroups
                .SingleOrDefaultAsync(cg => cg.Id == id, ct);
            if (entity == null)
                return false;
            _db.BookCategoryGroups.Remove(entity);
            return true;
        }

        // ========== 查詢 ==========

        public async Task<BookCategoryGroupResult?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var queryResult = await _db.BookCategoryGroups
                .AsNoTracking()
                .Where(cg => cg.Id == id && cg.IsActive)
                .Select(cg => new BookCategoryGroupResult
                {
                    Id = cg.Id,
                    Name = cg.Name,
                    IsActive = cg.IsActive,
                    Slug = cg.Slug
                })
                .SingleOrDefaultAsync(ct);
            return queryResult;
        }

        public async Task<IReadOnlyList<BookCategoryGroupResult>> GetAllAsync(CancellationToken ct = default)
        {
            var queryResult = await _db.BookCategoryGroups
                .AsNoTracking()
                .OrderBy(cg => cg.DisplayOrder)
                .Select(cg => new BookCategoryGroupResult
                {
                    Id = cg.Id,
                    Name = cg.Name,
                    IsActive = cg.IsActive,
                    Slug = cg.Slug
                })
                .ToListAsync(ct);
            return queryResult;
        }

    }
}
