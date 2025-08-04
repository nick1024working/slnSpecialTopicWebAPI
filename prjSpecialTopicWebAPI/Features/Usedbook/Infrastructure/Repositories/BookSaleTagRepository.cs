using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    public class BookSaleTagRepository
    {
        private readonly TeamAProjectContext _db;

        public BookSaleTagRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
             await _db.BookSaleTags.AsNoTracking().AnyAsync(st => st.Id == id, ct);

        public async Task<HashSet<int>> GetIdHashSetAsync(CancellationToken ct = default) =>
             await _db.BookSaleTags.AsNoTracking().Select(st => st.Id).ToHashSetAsync(ct);

        public async Task<int> CountAsync(CancellationToken ct = default) =>
            await _db.BookSaleTags.CountAsync(ct);

        // ========== 查詢實體 ==========

        public async Task<BookSaleTag?> GetEntityByIdAsync(int id, CancellationToken ct = default) =>
            await _db.BookSaleTags
            .SingleOrDefaultAsync(st => st.Id == id, ct);

        public async Task<IReadOnlyList<BookSaleTag>> GetEntityListAsync(CancellationToken ct = default) =>
            await _db.BookSaleTags.ToListAsync(ct);

        // ========== 新增、更新、刪除 ==========

        public void Add(BookSaleTag entity) =>
            _db.BookSaleTags.Add(entity);

        public void AddRange(IEnumerable<BookSaleTag> entityList) =>
            _db.BookSaleTags.AddRange(entityList);

        public void RemoveRange(IEnumerable<BookSaleTag> entityList) =>
            _db.BookSaleTags.RemoveRange(entityList);

        public async Task<bool> RemoveByIdAsync(int id, CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .SingleOrDefaultAsync(st => st.Id == id, ct);
            if (queryResult == null)
                return false;
            _db.BookSaleTags.Remove(queryResult);
            return true;
        }

        [Obsolete("目前 BLL 使用 GetEntityByIdAsync() 方法，直接更新實體", true)]
        public async Task<bool> UpdateAsync(BookSaleTag entity, CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .SingleOrDefaultAsync(st => st.Id == entity.Id, ct);
            if (queryResult == null)
                return false;

            queryResult.Name = entity.Name;
            queryResult.IsActive = entity.IsActive;

            return true;
        }

        // ========== 查詢 ==========

        public async Task<BookSaleTagResult?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .AsNoTracking()
                .Where(st => st.Id == id && st.IsActive)
                .Select(st => new BookSaleTagResult
                {
                    Id = st.Id,
                    Name = st.Name,
                    IsActive = st.IsActive,
                })
                .SingleOrDefaultAsync(ct);
            return queryResult;
        }

        public async Task<IReadOnlyList<BookSaleTagResult>> GetByBookIdAsync(Guid bookId, CancellationToken ct = default)
        {
            var queryResult = await _db.UsedBooks
                .AsNoTracking()
                .Where(b => b.Id == bookId)
                .SelectMany(b => b.Tags)
                .Where(st => st.IsActive)
                .OrderBy(st => st.DisplayOrder)
                .Select(st => new BookSaleTagResult
                {
                    Id = st.Id,
                    Name = st.Name,
                    IsActive = st.IsActive,
                })
                .ToListAsync(ct);
            return queryResult;
        }

        public async Task<IReadOnlyList<BookSaleTagResult>> GetAllAsync(CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .AsNoTracking()
                .OrderBy(st => st.DisplayOrder)
                .Select(st => new BookSaleTagResult
                {
                    Id = st.Id,
                    Name = st.Name,
                    IsActive = st.IsActive,
                })
                .ToListAsync(ct);
            return queryResult;
        }

    }
}
