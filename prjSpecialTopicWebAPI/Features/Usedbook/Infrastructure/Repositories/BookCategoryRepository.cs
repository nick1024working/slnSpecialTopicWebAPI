using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    public class BookCategoryRepository
    {
        private readonly TeamAProjectContext _db;

        public BookCategoryRepository(TeamAProjectContext context)
        {
            _db = context;
        }

        // ========== 通用 ==========

        public async Task<bool> HasRecords(CancellationToken ct = default) => await _db.BookCategories.AnyAsync(ct);

        public async Task<int> GetMaxDisplayOrderByGroupIdAsync(int groupId, CancellationToken ct = default) =>
            await _db.BookCategories.AsNoTracking().Where(cg => cg.GroupId == groupId).MaxAsync(cg => cg.DisplayOrder, ct);

        public async Task<bool> ExistsByNameAndGroupIdAsync(string name, int groupId, CancellationToken ct = default) =>
             await _db.BookCategories.AsNoTracking().AnyAsync(c => c.Name == name && c.GroupId == groupId, ct);

        // ========== 查詢實體 ==========

        public async Task<BookCategory?> GetEntityByIdAsync(int id, CancellationToken ct = default) =>
            await _db.BookCategories.FirstOrDefaultAsync(cg => cg.Id == id, ct);

        public async Task<IReadOnlyList<BookCategory>> GetEntityListByGroupIdAsync(int groupId, CancellationToken ct = default) =>
            await _db.BookCategories.Where(c => c.GroupId == groupId).ToListAsync(ct);

        // ========== 新增、更新、刪除 ==========

        public void Add(BookCategory entity) =>
            _db.BookCategories.Add(entity);

        public async Task<bool> RemoveByIdAsync(int id, CancellationToken ct = default)
        {
            var queryResult = await _db.BookCategories
                .SingleOrDefaultAsync(c => c.Id == id, ct);
            if (queryResult == null)
                return false;
            _db.BookCategories.Remove(queryResult);
            return true;
        }

        // ========== 查詢 ==========

        public async Task<BookCategoryResult?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var queryResult = await _db.BookCategories
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new BookCategoryResult
                {
                    Id = c.Id,
                    GroupId = c.GroupId,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    Slug = c.Slug,
                })
                .SingleOrDefaultAsync(ct);
            return queryResult;
        }

        public async Task<IReadOnlyList<BookCategoryResult>> GetAllByGroupIdAsync(int groupId, CancellationToken ct = default)
        {
            var queryResult = await _db.BookCategories
                .AsNoTracking()
                .Where(c => c.GroupId == groupId)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new BookCategoryResult
                {
                    Id = c.Id,
                    GroupId = c.GroupId,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    Slug = c.Slug,
                })
                .ToListAsync(ct);
            return queryResult;
        }

    }
}
