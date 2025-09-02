using prjSpecialTopicWebAPI.Features.Fund.Dtos;

namespace prjSpecialTopicWebAPI.Features.Fund.Services;

public interface IFundOrderService
{
    Task<OrderDto> CreateAsync(Guid uid, CreateOrderDto dto);
    Task<OrderDto?> GetByIdAsync(Guid uid, int id);
    Task<IEnumerable<OrderDto>> GetMineAsync(Guid uid);
    Task<bool> MarkPaidAsync(Guid uid, int id, string method);
    Task<bool> CancelAsync(Guid uid, int id);
}