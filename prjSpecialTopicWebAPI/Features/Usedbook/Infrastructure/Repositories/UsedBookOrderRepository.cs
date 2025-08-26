using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    public class UsedBookOrderRepository
    {
        private readonly TeamAProjectContext _db;

        public UsedBookOrderRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        // ========== 查詢實體 ==========

        public async Task<UsedBookOrder?> GetEntityByIdAsync(int id, CancellationToken ct = default) =>
            await _db.UsedBookOrders.FirstOrDefaultAsync(cg => cg.Id == id, ct);

        // ========== 新增、更新、刪除 ==========

        public void Add(UsedBookOrder entity) =>
            _db.UsedBookOrders.Add(entity);

    }
}
