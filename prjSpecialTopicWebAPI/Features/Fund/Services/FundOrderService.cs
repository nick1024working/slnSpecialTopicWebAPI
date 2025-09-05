using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Fund.Dtos;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Fund.Services;

public class FundOrderService : IFundOrderService
{
    private readonly TeamAProjectContext _db;
    public FundOrderService(TeamAProjectContext db) => _db = db;
    private static DateTime AsUtc(DateTime dt) =>
    DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    private static DateTime? AsUtc(DateTime? dt) =>
        dt.HasValue ? DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc) : (DateTime?)null;

    public async Task<OrderDto> CreateAsync(Guid uid, CreateOrderDto dto)
    {
        // 由 plan 反推 project（前端抓不到 projectId 也能成立）
        int? projectId = dto.ProjectId;
        if (!(projectId.HasValue && projectId.Value > 0))
        {
            if (!(dto.DonatePlanId.HasValue && dto.DonatePlanId.Value > 0))
                throw new ArgumentException("Invalid donatePlanId.");
            int planId = dto.DonatePlanId.Value;

            projectId = await _db.DonatePlans
                .Where(p => p.DonatePlanId == planId)
                .Select(p => (int?)p.DonateProjectId)
                .FirstOrDefaultAsync();

            if (projectId is null)
                throw new ArgumentException("Donate plan not found.");
        }

        var entity = new DonateOrder
        {
            Uid = uid,
            TotalAmount = dto.TotalAmount,
            PaymentMethod = dto.PaymentMethod ?? string.Empty,
            PaymentDate = null,
            OrderCreatedAt = DateTime.UtcNow,
            DonateProjectId = projectId,
            DonatePlanId = dto.DonatePlanId
        };

        _db.DonateOrders.Add(entity);
        await _db.SaveChangesAsync();

        return new OrderDto(
            entity.DonateOrderId,
            entity.TotalAmount,
            entity.PaymentMethod,
            AsUtc(entity.PaymentDate),
            AsUtc(entity.OrderCreatedAt),
            entity.DonateProjectId,
            entity.DonatePlanId
        );
    }

    public async Task<OrderDto?> GetByIdAsync(Guid uid, int id) =>
    await (from o in _db.DonateOrders.AsNoTracking()
           where o.DonateOrderId == id && o.Uid == uid
           join pl in _db.DonatePlans.AsNoTracking()
                on o.DonatePlanId equals pl.DonatePlanId into _pl
           from pl in _pl.DefaultIfEmpty()
           join pj in _db.DonateProjects.AsNoTracking()
                on o.DonateProjectId equals pj.DonateProjectId into _pj
           from pj in _pj.DefaultIfEmpty()
           select new OrderDto(
               o.DonateOrderId, o.TotalAmount, o.PaymentMethod, AsUtc(o.PaymentDate), AsUtc(o.OrderCreatedAt),
               o.DonateProjectId, o.DonatePlanId,
               // ★ 新增欄位
               pj != null ? pj.ProjectTitle : null,
               pl != null ? pl.PlanTitle : null
           )).FirstOrDefaultAsync();

    public async Task<IEnumerable<OrderDto>> GetMineAsync(Guid uid) =>
    await (from o in _db.DonateOrders.AsNoTracking()
           where o.Uid == uid
           orderby o.OrderCreatedAt descending
           join pl in _db.DonatePlans.AsNoTracking()
                on o.DonatePlanId equals pl.DonatePlanId into _pl
           from pl in _pl.DefaultIfEmpty()
           join pj in _db.DonateProjects.AsNoTracking()
                on o.DonateProjectId equals pj.DonateProjectId into _pj
           from pj in _pj.DefaultIfEmpty()
           select new OrderDto(
               o.DonateOrderId, o.TotalAmount, o.PaymentMethod, AsUtc(o.PaymentDate), AsUtc(o.OrderCreatedAt),
               o.DonateProjectId, o.DonatePlanId,
               // ★ 新增欄位
               pj != null ? pj.ProjectTitle : null,
               pl != null ? pl.PlanTitle : null
           )).ToListAsync();

    public async Task<bool> MarkPaidAsync(Guid uid, int id, string method)
    {
        // 交易，避免部份寫入
        await using var tx = await _db.Database.BeginTransactionAsync();

        var order = await _db.DonateOrders
            .FirstOrDefaultAsync(o => o.DonateOrderId == id && o.Uid == uid);

        if (order is null) return false;

        // 已經記錄過付款 → 視為成功（避免重複累加金額/人數）
        if (order.PaymentDate != null)
        {
            await tx.CommitAsync();
            return true;
        }

        order.PaymentMethod = method ?? string.Empty;
        order.PaymentDate = DateTime.UtcNow;

        // 只有在專案存在、且尚未被刪除時才更新統計
        if (order.DonateProjectId.HasValue)
        {
            var projectId = order.DonateProjectId.Value;
            var project = await _db.DonateProjects
                .FirstOrDefaultAsync(p => p.DonateProjectId == projectId && !p.IsDeleted);

            if (project != null)
            {
                // 1) 金額加總
                project.CurrentAmount += order.TotalAmount;

                // 2) 人數：同一 UID 贊助同專案僅算 1 人（需為「已付款」的訂單）
                bool alreadyBacker = await _db.DonateOrders.AsNoTracking().AnyAsync(o =>
                    o.DonateProjectId == projectId &&
                    o.Uid == uid &&
                    o.PaymentDate != null &&         // 只算已付款
                    o.DonateOrderId != id);          // 排除當前這筆

                if (!alreadyBacker)
                {
                    project.BackerCount = (project.BackerCount ?? 0) + 1;
                }

                project.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync();
        await tx.CommitAsync();
        return true;
    }

    public async Task<bool> CancelAsync(Guid uid, int id)
    {
        var order = await _db.DonateOrders.FirstOrDefaultAsync(o => o.DonateOrderId == id && o.Uid == uid);
        if (order is null) return false;
        _db.DonateOrders.Remove(order);
        await _db.SaveChangesAsync();
        return true;
    }
}
