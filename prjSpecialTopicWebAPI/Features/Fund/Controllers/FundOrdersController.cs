using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Fund.Dtos;
using prjSpecialTopicWebAPI.Features.Fund.Services;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Shared.DTOs;
using prjSpecialTopicWebAPI.Features.Shared.Extensions;
using prjSpecialTopicWebAPI.Features.Shared.Service;     // ← LinePayService
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace prjSpecialTopicWebAPI.Features.Fund.Controllers;

public record FundLinePaySessionCtx(Guid Uid, int OrderId, int Amount, string Currency = "TWD");

[ApiController]
[Route("api/fund/[controller]")]
public class FundOrdersController : ControllerBase
{
    private readonly IFundOrderService _svc;

    public FundOrdersController(IFundOrderService svc) => _svc = svc;

    private bool TryGetUid(out Guid uid)
    {
        uid = Guid.Empty;
        var uidStr =
            User.FindFirst("uid")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value   // ← 支援 JWT 的 sub
            ?? User.FindFirst("sub")?.Value;                        // ← 保險再補一個字串 "sub"

        return !string.IsNullOrEmpty(uidStr) && Guid.TryParse(uidStr, out uid);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        if (!TryGetUid(out var uid)) return Unauthorized();
        if (dto is null || dto.DonatePlanId is null || dto.DonatePlanId <= 0
            || dto.TotalAmount <= 0 || dto.Quantity <= 0)
            return BadRequest("Invalid order data.");

        try
        {
            // 1) 建單（Service 會由 plan 補 project）
            var order = await _svc.CreateAsync(uid, dto);

            // 2) 存 Session（回跳 confirm/cancel 要用）
            HttpContext.Session.SetObject("LINEPAY:FUND",
                new FundLinePaySessionCtx(uid, order.DonateOrderId, (int)order.TotalAmount));

            // 3) 初始化 LINE Pay，取付款網址
            var req = new LinePayPaymentRequestDto
            {
                OrderId = $"FUND-{order.DonateOrderId}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
                Amount = (int)order.TotalAmount,
                Currency = "TWD",
                Packages = new List<PackageDto> {
                new PackageDto {
                    Id = "FUND",
                    Name = "ProBookLand 募資",
                    Amount = (int)order.TotalAmount,
                    Products = new List<ProductDto> {
                        new ProductDto { Name = "Donate Plan", Quantity = 1, Price = (int)order.TotalAmount }
                    }
                }
            },
                RedirectUrls = new RedirectUrlsDto
                {
                    ConfirmUrl = $"{Request.Scheme}://{Request.Host}/api/fund/linepay/confirm",
                    CancelUrl = $"{Request.Scheme}://{Request.Host}/api/fund/linepay/cancel"
                }
            };

            var linePay = HttpContext.RequestServices.GetRequiredService<LinePayService>();
            var init = await linePay.RequestLinePayPaymentAsync(req, ct);
            var url = init?.Info?.PaymentUrl?.Web;
            if (string.IsNullOrEmpty(url))
                return Problem("LINE Pay 初始化失敗（無付款網址）");

            // 4) 與二手書一致：直接回 URL
            return Ok(new { url });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            return UnprocessableEntity($"寫入訂單失敗（外鍵或必要欄位缺失）：{ex.InnerException?.Message ?? ex.Message}");
        }
        catch (Exception ex)
        {
            // 避免黑盒子的 500，看得到實際錯誤訊息（開發期）
            return Problem($"Unhandled error: {ex.Message}");
        }
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
    public async Task<IActionResult> Pay([FromRoute] int id, CancellationToken ct)
    {
        if (!TryGetUid(out var uid)) return Unauthorized();

        var order = await _svc.GetByIdAsync(uid, id);
        if (order is null) return NotFound();

        HttpContext.Session.SetObject("LINEPAY:FUND",
            new FundLinePaySessionCtx(uid, id, (int)order.TotalAmount));

        var req = new LinePayPaymentRequestDto
        {
            OrderId = $"FUND-{id}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
            Amount = (int)order.TotalAmount,
            Currency = "TWD",
            Packages = new List<PackageDto> {
                new PackageDto {
                    Id = "FUND",
                    Name = "ProBookLand 募資",
                    Amount = (int)order.TotalAmount,
                    Products = new List<ProductDto> {
                        new ProductDto { Name = "Donate Plan", Quantity = 1, Price = (int)order.TotalAmount }
                    }
                }
            },
            RedirectUrls = new RedirectUrlsDto
            {
                ConfirmUrl = $"{Request.Scheme}://{Request.Host}/api/fund/linepay/confirm",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/api/fund/linepay/cancel"
            }
        };

        var linePay = HttpContext.RequestServices.GetRequiredService<LinePayService>();
        var init = await linePay.RequestLinePayPaymentAsync(req, ct);
        var url = init?.Info?.PaymentUrl?.Web;
        if (string.IsNullOrEmpty(url)) return Problem("LINE Pay 初始化失敗（無付款網址）");

        return Ok(new { url });
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
