using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    public class LanguageRepository
    {
        private readonly TeamAProjectContext _db;

        public LanguageRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<LanguageQueryResult>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Languages
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new LanguageQueryResult
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync(ct);
        } 
    }
}
