using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Features.Usedbook.Enums;
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

        public async Task<UsedBookOrder?> GetEntityByNoAsync(string orderNo, CancellationToken ct = default) =>
            await _db.UsedBookOrders.FirstOrDefaultAsync(cg => cg.OrderNo == orderNo, ct);


        // ========== 新增、更新、刪除 ==========

        public void Add(UsedBookOrder entity) =>
            _db.UsedBookOrders.Add(entity);

        public async Task<IReadOnlyList<UserOrderListItemQueryResult>> GetSellerOrderListAsync(
            Guid userId, CancellationToken ct = default)
        {
            var result = await _db.UsedBookOrders
                .AsNoTracking()
                .Where(r => r.SellerId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new UserOrderListItemQueryResult
                {
                    OrderNo = r.OrderNo,
                    OrderStatus = (OrderStatus)r.OrderStatus,
                    PaymentStatus = r.PaymentStatus,
                    DeliveryStatus = r.DeliveryStatus,
                    PaymentMethod = r.PaymentMethod,
                    DeliveryMethod = r.DeliveryMethod,
                    BuyerId = r.BuyerId,
                    SellerId = r.SellerId,
                    BookId = r.BookId,
                    Title = r.Title,
                    SalePrice = r.SalePrice,
                    CreatedAt = r.CreatedAt,
                })
                .ToListAsync(ct);

            return result;
        }

        public async Task<IReadOnlyList<UserOrderListItemQueryResult>> GetBuyerOrderListAsync(
            Guid userId, CancellationToken ct = default)
        {
            var result = await _db.UsedBookOrders
                .AsNoTracking()
                .Where(r => r.BuyerId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new UserOrderListItemQueryResult
                {
                    OrderNo = r.OrderNo,
                    OrderStatus = (OrderStatus)r.OrderStatus,
                    PaymentStatus = r.PaymentStatus,
                    DeliveryStatus = r.DeliveryStatus,
                    PaymentMethod = r.PaymentMethod,
                    DeliveryMethod = r.DeliveryMethod,
                    BuyerId = r.BuyerId,
                    SellerId = r.SellerId,
                    BookId = r.BookId,
                    Title = r.Title,
                    SalePrice = r.SalePrice,
                    CreatedAt = r.CreatedAt,
                })
                .ToListAsync(ct);

            return result;
        }

    }
}
