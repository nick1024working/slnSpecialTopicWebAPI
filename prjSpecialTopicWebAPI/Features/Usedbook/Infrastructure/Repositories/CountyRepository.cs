using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    public class CountyRepository
    {
        private readonly TeamAProjectContext _db;

        public CountyRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<CountyQueryResult>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Counties
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new CountyQueryResult
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync(ct);
        } 
    }
}
