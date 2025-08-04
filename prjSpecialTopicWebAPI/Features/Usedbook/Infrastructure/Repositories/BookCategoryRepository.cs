using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    public class BookCategoryRepository
    {
        private readonly TeamAProjectContext _db;

        public BookCategoryRepository(TeamAProjectContext context)
        {
            _db = context;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct) =>
             await _db.BookCategories.AsNoTracking().AnyAsync(t => t.Id == id, ct);

        // ========== 查詢實體 ==========

        public async Task<BookCategory?> GetEntityByIdAsync(int id, CancellationToken ct = default) =>
            await _db.BookCategories
            .SingleOrDefaultAsync(c => c.Id == id, ct);

        public async Task<IReadOnlyList<BookCategory>> GetEntityListAsync(CancellationToken ct = default) =>
            await _db.BookCategories.ToListAsync(ct);

        // ========== 新增、更新、刪除 ==========

        public void Add(BookCategory entity) =>
            _db.BookCategories.Add(entity);

        public void AddRange(List<BookCategory> entityList) =>
            _db.BookCategories.AddRange(entityList);

        public void RemoveRange(List<BookCategory> entityList) =>
            _db.BookCategories.RemoveRange(entityList);

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
                    Name = c.Name,
                })
                .SingleOrDefaultAsync(ct);
            return queryResult;
        }

        public async Task<IReadOnlyList<BookCategoryResult>> GetAllAsync(CancellationToken ct = default)
        {
            var queryResult = await _db.BookCategories
                .AsNoTracking()
                .Select(c => new BookCategoryResult
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .OrderBy(c => c.Id)
                .ToListAsync(ct);
            return queryResult;
        }

    }
}
