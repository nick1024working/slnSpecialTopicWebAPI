using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    public class BookBindingRepository
    {
        private readonly TeamAProjectContext _db;

        public BookBindingRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<BookBindingQueryResult>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.BookBindings
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new BookBindingQueryResult
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync(ct);
        } 
    }
}
