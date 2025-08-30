using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Authentication;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Usedbook.Application.Services;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Controllers
{
    [ApiController]
    [Route("api/usedbooks")]
    public class UsedBookOrderController : ControllerBase
    {
        private readonly AuthHelper _authHelper;
        private readonly UsedBookOrderService _orderService;

        public UsedBookOrderController(
            AuthHelper authHelper,
            UsedBookOrderService orderService)
        {
            _authHelper = authHelper;
            _orderService = orderService;
        }

        // ========== 新增、更新、刪除 ==========

        [HttpPost("orders")]
        public async Task<ActionResult<UrlDto>> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken ct)
        {
            Guid userId = _authHelper.GetUser(HttpContext) ?? Guid.Parse("EBB03874-054F-4FEA-9AE8-02B8D05C4BB3");

            var result = await _orderService.CreateAsync(userId, request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

        // ========== 更改狀態 ==========

        [HttpPatch("orders/{orderNo}")]
        public async Task<IActionResult> UpdateOrderStatus(
            [FromRoute] string orderNo, [FromBody] UpdateOrderStatusRequest req, CancellationToken ct)
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderNo, req, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        // ========== 查詢 ==========

        [HttpGet("orders/{orderNo}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderDetail([FromRoute] string orderNo, CancellationToken ct)
        {
            Guid userId = _authHelper.GetUser(HttpContext) ?? Guid.Parse("EBB03874-054F-4FEA-9AE8-02B8D05C4BB3");

            var result = await _orderService.GetOrderDetailAsync(orderNo, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

        [HttpGet("admin/orders")]
        public async Task<ActionResult<IReadOnlyList<AdminOrderListItemDto>>> GetAdminOrderList(CancellationToken ct)
        {
            var result = await _orderService.GetAdminOrderListAsync(ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

        [HttpGet("sellers/orders")]
        public async Task<ActionResult<IReadOnlyList<UserOrderListItemDto>>> GetSellerOrderList(CancellationToken ct)
        {
            Guid userId = _authHelper.GetUser(HttpContext) ?? Guid.Parse("EBB03874-054F-4FEA-9AE8-02B8D05C4BB3");

            var result = await _orderService.GetSellerOrderListAsync(userId, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

        [HttpGet("buyers/orders")]
        public async Task<ActionResult<IReadOnlyList<UserOrderListItemDto>>> GetBuyerOrderList(CancellationToken ct)
        {
            Guid userId = _authHelper.GetUser(HttpContext) ?? Guid.Parse("EBB03874-054F-4FEA-9AE8-02B8D05C4BB3");

            var result = await _orderService.GetBuyerOrderListAsync(userId, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

    }
}
