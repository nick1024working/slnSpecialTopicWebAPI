using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    public class BookConditionRatingRepository
    {
        private readonly TeamAProjectContext _db;

        public BookConditionRatingRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<BookConditionRatingResult>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.BookConditionRatings
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new BookConditionRatingResult
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                })
                .ToListAsync(ct);
        }

        public async Task<string?> GetDescriptionByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.BookConditionRatings
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.Description)
                .SingleOrDefaultAsync(ct);
        }
    }
}
