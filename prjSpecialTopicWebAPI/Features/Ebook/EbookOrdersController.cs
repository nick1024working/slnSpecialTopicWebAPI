using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Ebook.DTOs;
using prjSpecialTopicWebAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace prjSpecialTopicWebAPI.Features.Ebook
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EbookOrdersController : ControllerBase
    {
        private readonly TeamAProjectContext _context;

        public EbookOrdersController(TeamAProjectContext context)
        {
            _context = context;
        }

        // --- 取得登入會員的所有歷史訂單 ---
        // 這個方法已經包含了處理圖片路徑的邏輯
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderHistoryDto>>> GetMyOrders()
        {
            //var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // [修改] 改用 JwtRegisteredClaimNames.Sub
            var userIdString = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("無效的使用者 ID 格式。");
            }

            var orders = await _context.EBookOrderMains
                .AsNoTracking()
                .Where(o => o.Uid == userId)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderItems)
                    // 關鍵：載入訂單項目關聯的 EBook 實體，才能取得圖片路徑
                    .ThenInclude(oi => oi.EBook)
                .OrderByDescending(o => o.OrderDateTime)
                .Select(order => new OrderHistoryDto
                {
                    OrderId = order.OrderId.ToString(),
                    OrderDate = order.OrderDateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                    Status = order.OrderStatus.StatusName,
                    TotalAmount = order.TotalAmount,
                    Items = order.OrderItems.Select(item => new OrderHistoryItemDto
                    {
                        EbookId = item.EBookId ?? 0,
                        EbookName = item.ItemNameSnapshot,
                        Price = item.UnitPriceAtPurchase,
                        Quantity = item.Quantity,
                        // --- 核心：組裝完整的圖片 URL ---
                        // 檢查 EBook 和 PrimaryCoverPath 是否存在，然後組合 URL
                        PrimaryCoverPath = (item.EBook != null && !string.IsNullOrEmpty(item.EBook.PrimaryCoverPath))
                                            ? $"{Request.Scheme}://{Request.Host}/{item.EBook.PrimaryCoverPath.TrimStart('/')}"
                                            : null // 如果沒有圖片，則回傳 null
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        // --- 根據訂單 ID 取得單筆訂單的詳細資訊 ---
        // 這個方法也已經包含了處理圖片路徑的邏輯
        [HttpGet("{orderId:long}")]
        public async Task<ActionResult<OrderHistoryDto>> GetOrderById(long orderId)
        {
            //var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // [修改] 改用 JwtRegisteredClaimNames.Sub
            var userIdString = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("無效的使用者識別碼。");
            }

            var orderDto = await _context.EBookOrderMains
                .Where(o => o.OrderId == orderId && o.Uid == userId)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.EBook) // 同樣需要載入 EBook 實體
                .Select(order => new OrderHistoryDto
                {
                    OrderId = order.OrderId.ToString(),
                    OrderDate = order.OrderDateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                    Status = order.OrderStatus.StatusName,
                    TotalAmount = order.TotalAmount,
                    Items = order.OrderItems.Select(item => new OrderHistoryItemDto
                    {
                        EbookId = item.EBookId ?? 0,
                        EbookName = item.ItemNameSnapshot,
                        Price = item.UnitPriceAtPurchase,
                        Quantity = item.Quantity,
                        // --- 核心：組裝完整的圖片 URL ---
                        PrimaryCoverPath = (item.EBook != null && !string.IsNullOrEmpty(item.EBook.PrimaryCoverPath))
                                            ? $"{Request.Scheme}://{Request.Host}/{item.EBook.PrimaryCoverPath.TrimStart('/')}"
                                            : null
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (orderDto == null)
            {
                return NotFound("找不到該訂單，或您沒有權限檢視此訂單。");
            }

            return Ok(orderDto);
        }

        // --- 建立新訂單 ---
        // --- 建立新訂單 ---
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] List<EbookCartItemDto> cartItems)
        {
            if (cartItems == null || !cartItems.Any())
            {
                return BadRequest("購物車項目不得為空。");
            }

            // var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            // [修改] 改用 JwtRegisteredClaimNames.Sub
            var userIdString = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("無效的使用者識別碼。");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ebookIds = cartItems.Select(item => item.EbookId).Distinct().ToList();
                var ebooksInDb = await _context.EBookMains
                    .Where(e => ebookIds.Contains(e.EbookId) && e.IsAvailable)
                    .ToDictionaryAsync(e => e.EbookId);

                if (ebookIds.Count != ebooksInDb.Count)
                {
                    await transaction.RollbackAsync();
                    return BadRequest("購物車中有一或多本書籍無效或已下架。");
                }

                var newOrder = new EBookOrderMain
                {
                    Uid = userId,
                    OrderDateTime = DateTime.UtcNow,
                    OrderStatusId = 1, // 假設 1 = 處理中/已完成
                    CurrencyCode = "TWD",
                    TotalAmount = 0,
                    LastModifiedDate = DateTime.UtcNow,
                };

                foreach (var item in cartItems)
                {
                    var ebook = ebooksInDb[item.EbookId];
                    var priceAtPurchase = ebook.ActualPrice ?? ebook.FixedPrice;

                    // [修正 1] 計算單一品項的總金額 (LineItemTotal)
                    var lineTotal = priceAtPurchase * item.Quantity;

                    // 將品項總額累加到訂單總金額
                    newOrder.TotalAmount += lineTotal;

                    newOrder.OrderItems.Add(new OrderItem
                    {
                        ItemTypeId = 1, // 假設 1 = E-Book
                        EBookId = item.EbookId,
                        Quantity = item.Quantity,
                        UnitPriceAtPurchase = priceAtPurchase,
                        ItemNameSnapshot = ebook.EbookName,
                        // [修正 1] 賦予 LineItemTotal 的值
                        LineItemTotal = lineTotal
                    });
                }

                _context.EBookOrderMains.Add(newOrder);
                // 第一次儲存，目的是為了讓 EF Core 產生 OrderId
                await _context.SaveChangesAsync();

                // --- [核心修改] ---
                // 移除在這裡新增 EbookPurchaseds 的所有程式碼，
                // 因為付款尚未完成，不能給予書籍。

                //var existingPurchases = await _context.EbookPurchaseds
                //    .Where(p => p.Uid == userId && ebookIds.Contains(p.EBookId))
                //    .Select(p => p.EBookId)
                //    .ToListAsync();

                //foreach (var item in newOrder.OrderItems)
                //{
                //    if (item.EBookId.HasValue && !existingPurchases.Contains(item.EBookId.Value))
                //    {
                //        var purchaseRecord = new EbookPurchased
                //        {
                //            Uid = userId,
                //            EBookId = item.EBookId.Value,

                //            // [修正 2] 根據 EbookPurchased.cs 模型，移除不存在的 OrderId 屬性
                //            // OrderId = newOrder.OrderId, 

                //            PurchaseDateTime = DateTime.UtcNow,
                //            LastReadTime = DateTime.UtcNow,
                //        };
                //        _context.EbookPurchaseds.Add(purchaseRecord);
                //    }
                //}

                //// 第二次儲存，將 EbookPurchased 的記錄寫入資料庫
                //await _context.SaveChangesAsync();

                // --- [修改結束] ---

                await transaction.CommitAsync();

                //return Ok(new { orderId = newOrder.OrderId });

                // [核心修改] 只回傳新建立的訂單 ID
                return Ok(new { orderId = newOrder.OrderId });
            }
            catch (Exception) // 建議可以加上 Log
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "建立訂單時發生內部錯誤。");
            }
        }
    }
}
