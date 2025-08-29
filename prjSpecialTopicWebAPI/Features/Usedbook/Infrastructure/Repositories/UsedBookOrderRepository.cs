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
            await _db.UsedBookOrders.FirstOrDefaultAsync(od => od.OrderNo == orderNo, ct);

        // ========== 新增、更新、刪除 ==========

        public void AddOrder(UsedBookOrder entity) =>
            _db.UsedBookOrders.Add(entity);

        public void AddRangeOrderItems(IReadOnlyList<UsedBookOrderItem> entityList) =>
            _db.UsedBookOrderItems.AddRange(entityList);

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
                    BuyerId = r.BuyerId,
                    SellerId = r.SellerId,

                    OrderStatus = (OrderStatus)r.OrderStatus,
                    PaymentStatus = (PaymentStatus)r.PaymentStatus,
                    DeliveryStatus = (DeliveryStatus)r.DeliveryStatus,
                    PaymentMethod = (PaymentMethod)r.PaymentMethod,
                    DeliveryMethod = (DeliveryMethod)r.DeliveryMethod,

                    Subtotal = r.Subtotal,
                    DiscountTotal = r.DiscountTotal,
                    DeliveryFee = r.DeliveryFee,
                    GrandTotal = r.GrandTotal,

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
                    BuyerId = r.BuyerId,
                    SellerId = r.SellerId,

                    OrderStatus = (OrderStatus)r.OrderStatus,
                    PaymentStatus = (PaymentStatus)r.PaymentStatus,
                    DeliveryStatus = (DeliveryStatus)r.DeliveryStatus,
                    PaymentMethod = (PaymentMethod)r.PaymentMethod,
                    DeliveryMethod = (DeliveryMethod)r.DeliveryMethod,

                    Subtotal = r.Subtotal,
                    DiscountTotal = r.DiscountTotal,
                    DeliveryFee = r.DeliveryFee,
                    GrandTotal = r.GrandTotal,

                    CreatedAt = r.CreatedAt,
                })
                .ToListAsync(ct);

            return result;
        }

    }
}
