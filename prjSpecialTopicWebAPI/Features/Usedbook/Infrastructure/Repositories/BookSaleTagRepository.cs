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

        // ========== 通用 ==========

        public async Task<bool> HasRecords(CancellationToken ct = default) => await _db.BookSaleTags.AnyAsync(ct);

        public async Task<int> GetMaxDisplayOrderAsync(CancellationToken ct = default) =>
            await _db.BookSaleTags.AsNoTracking().MaxAsync(st => st.DisplayOrder, ct);

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default) =>
            await _db.BookSaleTags.AsNoTracking().AnyAsync(st => st.Name == name);

        // ========== 查詢實體 ==========

        public async Task<BookSaleTag?> GetEntityByIdAsync(int id, CancellationToken ct = default) =>
            await _db.BookSaleTags.SingleOrDefaultAsync(st => st.Id == id, ct);

        public async Task<IReadOnlyList<BookSaleTag>> GetEntityListAsync(CancellationToken ct = default) =>
            await _db.BookSaleTags.ToListAsync(ct);

        // ========== 新增、更新、刪除 ==========

        public void Add(BookSaleTag entity) =>
            _db.BookSaleTags.Add(entity);

        public async Task<bool> RemoveByIdAsync(int id, CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .SingleOrDefaultAsync(st => st.Id == id, ct);
            if (queryResult == null)
                return false;
            _db.BookSaleTags.Remove(queryResult);
            return true;
        }

        // ========== 查詢 ==========

        public async Task<BookSaleTagQueryResult?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .AsNoTracking()
                .Where(st => st.Id == id)
                .Select(st => new BookSaleTagQueryResult
                {
                    Id = st.Id,
                    Name = st.Name,
                    IsActive = st.IsActive,
                })
                .SingleOrDefaultAsync(ct);
            return queryResult;
        }

        // TODO: 需確認需求的 IsActive
        public async Task<IReadOnlyList<BookSaleTagQueryResult>> GetByBookIdAsync(Guid bookId, CancellationToken ct = default)
        {
            var queryResult = await _db.UsedBooks
                .AsNoTracking()
                .Where(b => b.Id == bookId)
                .SelectMany(b => b.Tags)
                .Where(st => st.IsActive)
                .OrderBy(st => st.DisplayOrder)
                .Select(st => new BookSaleTagQueryResult
                {
                    Id = st.Id,
                    Name = st.Name,
                    IsActive = st.IsActive,
                })
                .ToListAsync(ct);
            return queryResult;
        }

        // TODO: 需確認需求的 IsActive
        public async Task<IReadOnlyList<BookSaleTagQueryResult>> GetAllAsync(CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .AsNoTracking()
                .OrderBy(st => st.DisplayOrder)
                .Select(st => new BookSaleTagQueryResult
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
