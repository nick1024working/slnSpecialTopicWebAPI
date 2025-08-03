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

        public void Add(BookCategory entity)
        {
            _db.BookCategories.Add(entity);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct) =>
             await _db.BookCategories.AsNoTracking().AnyAsync(t => t.Id == id, ct);

        public async Task<bool> UpdateAsync(BookCategory entity, CancellationToken ct = default)
        {
            var queryResult = await _db.BookCategories
                .SingleOrDefaultAsync(c => c.Id == entity.Id, ct);
            if (queryResult == null)
                return false;

            queryResult.Name = entity.Name;

            return true;
        }

        public async Task<bool> UpdateActiveStatusAsync(int id, bool isActive, CancellationToken ct = default)
        {
            var queryResult = await _db.BookCategories
                .SingleOrDefaultAsync(c => c.Id == id, ct);
            if (queryResult == null)
                return false;

            queryResult.IsActive = isActive;

            return true;
        }

        public async Task UpdateAllAsync(IEnumerable<BookCategory> entityList, CancellationToken ct = default)
        {
            var oldList = await _db.BookCategories.ToListAsync(ct);
            _db.BookCategories.RemoveRange(oldList);
            _db.BookCategories.AddRange(entityList);
        }

        public async Task<bool> RemoveByIdAsync(int id, CancellationToken ct = default)
        {
            var queryResult = await _db.BookCategories
                .SingleOrDefaultAsync(c => c.Id == id, ct);
            if (queryResult == null)
                return false;
            _db.BookCategories.Remove(queryResult);
            return true;
        }

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
