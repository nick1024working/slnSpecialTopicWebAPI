using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    public class ExternalDomainRepository
    {
        private readonly TeamAProjectContext _db;

        public ExternalDomainRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<Guid>> GetSellerListAsync(CancellationToken ct = default)
        {
            var result = await _db.UsedBooks
                .AsNoTracking()
                .GroupBy(b => b.SellerId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .ToListAsync(ct);
            return result;
        }
    }
}
