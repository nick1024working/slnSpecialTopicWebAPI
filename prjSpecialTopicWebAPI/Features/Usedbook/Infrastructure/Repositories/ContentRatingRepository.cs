using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Interfaces;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    public class ContentRatingRepository
    {
        private readonly TeamAProjectContext _db;

        public ContentRatingRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<IHasIdName>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.ContentRatings
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new ContentRatingResult
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync(ct);
        } 
    }
}
