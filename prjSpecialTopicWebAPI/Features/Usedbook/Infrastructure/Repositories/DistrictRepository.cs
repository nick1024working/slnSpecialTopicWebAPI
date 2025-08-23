using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    public class DistrictRepository
    {
        private readonly TeamAProjectContext _db;

        public DistrictRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<DistrictQueryResult>> GetByCountyIdAsync(int countyId, CancellationToken ct = default)
        {
            return await _db.Districts
                .AsNoTracking()
                .OrderBy(d => d.Id)
                .Where(d => d.CountyId == countyId)
                .Select(d => new DistrictQueryResult
                {
                    Id = d.Id,
                    Name = d.Name,
                })
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<IdNameDto>> GetCountyIdDistrictNameAsync(CancellationToken ct = default)
        {
            return await _db.Districts
                .AsNoTracking()
                .OrderBy(d => d.Id)
                .Select(d => new IdNameDto
                {
                    Id = d.CountyId,
                    Name = d.Name,
                })
                .ToListAsync(ct);
        }
    }
}
