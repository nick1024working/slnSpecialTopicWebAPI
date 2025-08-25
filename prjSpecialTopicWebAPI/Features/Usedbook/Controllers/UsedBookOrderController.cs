using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Usedbook.Application.Services;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Controllers
{
    [ApiController]
    [Route("api/usedbooks/orders")]
    public class UsedBookOrderController : ControllerBase
    {
        private readonly UsedBookOrderService _orderService;

        public UsedBookOrderController(
            UsedBookOrderService orderService)
        {
            _orderService = orderService;
        }

        // ========== 新增、更新、刪除 ==========

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateOrder([FromForm] CreateOrderRequest request, CancellationToken ct)
        {
            var result = await _orderService.CreateAsync(request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        // ========== 更改狀態 ==========

        //[HttpPut("{orderNo}")]
        //public async Task<IActionResult> UpdateOrderStatus(
        //    [FromRoute] string orderNo, [FromBody] UpdateStatusRequest status, CancellationToken ct)
        //{
        //    var result = await _bookService.UpdateOrderStatusAsync(orderNo, status, ct);
        //    if (!result.IsSuccess)
        //        return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
        //    return NoContent();
        //}


        // ========== 查詢 ==========
    }
}
