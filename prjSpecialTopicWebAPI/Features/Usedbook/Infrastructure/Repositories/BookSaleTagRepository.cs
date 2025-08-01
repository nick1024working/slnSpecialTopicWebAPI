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

        /// <summary>
        /// 直接返回促銷標籤實體。
        /// </summary>
        public async Task<BookSaleTag?> GetEntityAsync(int id, CancellationToken ct = default) =>
            await _db.BookSaleTags
            .SingleOrDefaultAsync(st => st.Id == id, ct);


        /// <summary>
        /// 建立新銷售標籤。
        /// </summary>
        public void Add(BookSaleTag entity)
        {
            _db.BookSaleTags.Add(entity);
        }

        /// <summary>
        /// 檢查是否存在。
        /// </summary>
        public async Task<bool> ExistsAsync(int tagId, CancellationToken ct = default) =>
             await _db.BookSaleTags.AsNoTracking().AnyAsync(t => t.Id == tagId, ct);

        /// <summary>
        /// 更新指定銷售標籤的名稱。
        /// </summary>
        /// <exception cref="InvalidOperationException">查詢結果超過一筆時拋出，通常代表資料違反唯一性約束。</exception>
        public async Task<bool> UpdateAsync(BookSaleTag entity, CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .SingleOrDefaultAsync(st => st.Id == entity.Id, ct);
            if (queryResult == null)
                return false;

            queryResult.Name = entity.Name;

            return true;
        }

        public async Task<bool> UpdateActiveStatusAsync(int id, bool isActive, CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .SingleOrDefaultAsync(st => st.Id == id, ct);
            if (queryResult == null)
                return false;

            queryResult.IsActive = isActive;

            return true;
        }

        public async Task UpdateAllAsync(IEnumerable<BookSaleTag> entityList, CancellationToken ct = default)
        {
            var oldList = await _db.BookSaleTags.ToListAsync(ct);
            _db.BookSaleTags.RemoveRange(oldList);

            _db.BookSaleTags.AddRange(entityList);
        }

        /// <summary>
        /// 根據 Id 查詢銷售標籤。
        /// </summary>
        /// <exception cref="InvalidOperationException">查詢結果超過一筆時拋出，通常代表資料違反唯一性約束。</exception>
        public async Task<BookSaleTagResult?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .AsNoTracking()
                .Where(st => st.Id == id && st.IsActive)
                .Select(st => new BookSaleTagResult
                {
                    Id = st.Id,
                    Name = st.Name,
                })
                .SingleOrDefaultAsync(ct);
            return queryResult;
        }

        /// <summary>
        /// 根據 BookId 查詢所有銷售標籤。
        /// </summary>
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
                })
                .ToListAsync(ct);
            return queryResult;
        }

        /// <summary>
        /// 查詢所有銷售標籤清單。
        /// </summary>
        public async Task<IReadOnlyList<BookSaleTagResult>> GetAllAsync(CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .AsNoTracking()
                .OrderBy(st => st.DisplayOrder)
                .Select(st => new BookSaleTagResult
                {
                    Id = st.Id,
                    Name = st.Name,
                })
                .ToListAsync(ct);
            return queryResult;
        }

    }
}
