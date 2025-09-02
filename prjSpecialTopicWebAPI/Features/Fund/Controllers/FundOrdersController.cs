using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Fund.Dtos;
using prjSpecialTopicWebAPI.Features.Fund.Services;
using System.Security.Claims;

namespace prjSpecialTopicWebAPI.Features.Fund.Controllers;

[ApiController]
[Route("api/fund/[controller]")]
public class FundOrdersController : ControllerBase
{
    private readonly IFundOrderService _svc;

    public FundOrdersController(IFundOrderService svc)
    {
        _svc = svc;
    }

    private bool TryGetUid(out Guid uid)
    {
        uid = Guid.Empty;
        var uidStr = User.FindFirst("uid")?.Value
                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return !string.IsNullOrEmpty(uidStr) && Guid.TryParse(uidStr, out uid);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        if (!TryGetUid(out var uid)) return Unauthorized();
        var entity = await _svc.CreateAsync(uid, dto);
        return CreatedAtAction(nameof(GetById), new { id = entity.DonateOrderId }, new { orderId = entity.DonateOrderId });
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById([FromRoute] int id)
    {
        if (!TryGetUid(out var uid)) return Unauthorized();
        var data = await _svc.GetByIdAsync(uid, id);
        return data == null ? NotFound() : data;
    }

    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetMine()
    {
        if (!TryGetUid(out var uid)) return Unauthorized();
        var list = await _svc.GetMineAsync(uid);
        return Ok(list);
    }

    [Authorize]
    [HttpPatch("{id:int}/pay")]
    public async Task<IActionResult> MarkPaid([FromRoute] int id, [FromBody] PayDto dto)
    {
        if (!TryGetUid(out var uid)) return Unauthorized();
        var ok = await _svc.MarkPaidAsync(uid, id, dto.PaymentMethod);
        return ok ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel([FromRoute] int id)
    {
        if (!TryGetUid(out var uid)) return Unauthorized();
        var ok = await _svc.CancelAsync(uid, id);
        return ok ? NoContent() : NotFound();
    }
}
