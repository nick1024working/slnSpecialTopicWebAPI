using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Fund.Dtos;
using prjSpecialTopicWebAPI.Models; // ★ 用現有的 DonateOrder 實體

namespace prjSpecialTopicWebAPI.Features.Fund.Services;

public class FundOrderService : IFundOrderService
{
    private readonly TeamAProjectContext _db;

    public FundOrderService(TeamAProjectContext db) => _db = db;

    public async Task<OrderDto> CreateAsync(Guid uid, CreateOrderDto dto)
    {
        var entity = new DonateOrder
        {
            Uid = uid,                                         // ← 注意屬性名稱是 Uid
            TotalAmount = dto.TotalAmount,
            PaymentMethod = dto.PaymentMethod ?? string.Empty, // ← 實體是非 nullable，避免 null
            PaymentDate = null,
            OrderCreatedAt = DateTime.UtcNow,
            DonateProjectId = dto.ProjectId,
            DonatePlanId = dto.DonatePlanId
        };

        _db.DonateOrders.Add(entity); // ← DbSet<DonateOrder>
        await _db.SaveChangesAsync();

        return new OrderDto(
            entity.DonateOrderId,
            entity.TotalAmount,
            entity.PaymentMethod,
            entity.PaymentDate,
            entity.OrderCreatedAt
        );
    }

    public async Task<OrderDto?> GetByIdAsync(Guid uid, int id) =>
        await _db.DonateOrders
            .AsNoTracking()
            .Where(o => o.DonateOrderId == id && o.Uid == uid)
            .Select(o => new OrderDto(
                o.DonateOrderId,
                o.TotalAmount,
                o.PaymentMethod,
                o.PaymentDate,
                o.OrderCreatedAt
            ))
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<OrderDto>> GetMineAsync(Guid uid) =>
        await _db.DonateOrders
            .AsNoTracking()
            .Where(o => o.Uid == uid)
            .OrderByDescending(o => o.OrderCreatedAt)
            .Select(o => new OrderDto(
                o.DonateOrderId,
                o.TotalAmount,
                o.PaymentMethod,
                o.PaymentDate,
                o.OrderCreatedAt
            ))
            .ToListAsync();

    public async Task<bool> MarkPaidAsync(Guid uid, int id, string method)
    {
        var order = await _db.DonateOrders
            .FirstOrDefaultAsync(o => o.DonateOrderId == id && o.Uid == uid);
        if (order is null) return false;

        order.PaymentMethod = method ?? string.Empty;
        order.PaymentDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelAsync(Guid uid, int id)
    {
        var order = await _db.DonateOrders
            .FirstOrDefaultAsync(o => o.DonateOrderId == id && o.Uid == uid);
        if (order is null) return false;

        _db.DonateOrders.Remove(order);
        await _db.SaveChangesAsync();
        return true;
    }
}