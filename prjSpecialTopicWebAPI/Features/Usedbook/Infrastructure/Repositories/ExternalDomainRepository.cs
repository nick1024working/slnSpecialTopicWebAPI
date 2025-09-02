using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
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

        public async Task<IReadOnlyList<CurrentSellerDto>> GetSellerListAsync(CancellationToken ct = default)
        {
            var result = await _db.Users
                .AsNoTracking()
                .Select(u => new
                {
                    u.Uid,
                    u.Name,
                    u.Email,
                    BookCount = u.UsedBooks.Count()
                })
                .OrderByDescending(x => x.BookCount)
                .Where(x => x.BookCount > 0)
                .Select(x => new CurrentSellerDto
                {
                    Id = x.Uid,
                    Name = x.Name,
                    Email = x.Email,
                })
                .ToListAsync(ct);
            return result;
        }
    }
}
