using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Shared.DTOs; // 共用的 DTO
using prjSpecialTopicWebAPI.Features.Shared.Service; // 共用的 LinePayService
using prjSpecialTopicWebAPI.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace prjSpecialTopicWebAPI.Features.Ebook
{
    [ApiController]
    [Route("api/ebooks/line-pay")] // [修改] 路由，使其歸屬於 Ebook 功能下
    public class EbookLinePayController : ControllerBase // [修改] 類別名稱
    {
        private readonly LinePayService _linePayService;
        private readonly TeamAProjectContext _context;

        public EbookLinePayController(LinePayService linePayService, TeamAProjectContext context)
        {
            _linePayService = linePayService;
            _context = context;
        }

        /// <summary>
        /// 步驟 1: 根據訂單 ID，請求 LINE Pay 付款連結
        /// </summary>
        [Authorize]
        [HttpPost("request/{orderId}")]
        public async Task<IActionResult> RequestPayment(long orderId)
        {
            var userIdString = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("無效的使用者識別碼。");
            }

            // 從資料庫找出屬於該使用者的訂單
            var order = await _context.EBookOrderMains
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.Uid == userId);

            if (order == null) return NotFound("找不到您的訂單，或您無權支付此訂單。");

            string uniqueOrderIdForLinePay = $"PBLE{order.OrderId}{DateTime.UtcNow:yyyyMMddHHmmssfff}";

            // 建立要傳送給 LINE Pay 的請求物件
            var linePayRequest = new LinePayPaymentRequestDto
            {
                Amount = (int)order.TotalAmount,
                Currency = "TWD",
                //OrderId = order.OrderId.ToString(),
                OrderId = uniqueOrderIdForLinePay, // <-- 修正點：使用新產生的唯一 ID
                Packages = new List<PackageDto>
                {
                    new PackageDto
                    {
                        Id = $"pkg_{order.OrderId}",
                        Amount = (int)order.TotalAmount,
                        Name = "ProBookLand 電子書",
                        Products = order.OrderItems.Select(item => new ProductDto
                        {
                            Name = item.ItemNameSnapshot,
                            Quantity = item.Quantity,
                            Price = (int)item.UnitPriceAtPurchase
                        }).ToList()
                    }
                },
                RedirectUrls = new RedirectUrlsDto
                {
                    // [重要] ConfirmUrl 需包含 orderId，以便返回時能識別是哪筆訂單
                    // ConfirmUrl = $"http://localhost:4200/checkout/confirm?orderId={order.OrderId}",
                    ConfirmUrl = $"http://localhost:4200/ebook/checkout/confirm?orderId={order.OrderId}",
                    CancelUrl = "http://localhost:4200/cart"
                }
            };

            var response = await _linePayService.RequestLinePayPaymentAsync(linePayRequest);
            return Ok(response);
        }

        /// <summary>
        /// 步驟 2: 使用者付款後，從 LINE Pay 返回時，確認付款結果
        /// </summary>
        [Authorize]
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment([FromQuery] string transactionId, [FromQuery] string orderId)
        {
            var userIdString = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("無效的使用者識別碼。");
            }

            if (!long.TryParse(orderId, out var orderIdLong))
            {
                return BadRequest("無效的訂單 ID 格式。");
            }

            // 再次驗證訂單歸屬
            var order = await _context.EBookOrderMains
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderIdLong && o.Uid == userId);

            if (order == null) return NotFound("找不到您的訂單，或您無權確認此訂單。");

            var confirmDto = new LinePayPaymentConfirmDto
            {
                Amount = (int)order.TotalAmount,
                Currency = "TWD"
            };

            var response = await _linePayService.ConfirmLinePayPaymentAsync(transactionId, confirmDto);

            if (response.ReturnCode == "0000")
            {
                // 付款成功
                order.OrderStatusId = 2; // 更新訂單狀態為「已完成」
                order.PaymentGatewayTransactionId = transactionId;

                // 將書籍加入書櫃
                foreach (var item in order.OrderItems)
                {
                    if (item.EBookId.HasValue)
                    {
                        var exists = await _context.EbookPurchaseds.AnyAsync(p => p.Uid == userId && p.EBookId == item.EBookId.Value);
                        if (!exists)
                        {
                            _context.EbookPurchaseds.Add(new EbookPurchased
                            {
                                Uid = userId,
                                EBookId = item.EBookId.Value,
                                PurchaseDateTime = DateTime.UtcNow,
                                LastReadTime = DateTime.UtcNow,
                            });
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return Ok(response);
            }

            // 付款失敗
            return BadRequest(response);
        }
    }
}
