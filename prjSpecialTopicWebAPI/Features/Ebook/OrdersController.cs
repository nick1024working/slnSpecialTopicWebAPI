using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Ebook.DTOs;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Ebook
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly TeamAProjectContext _context; // 請確認您的 DbContext 名稱是否為 TeamAProjectContext

        // 透過依賴注入(DI)取得資料庫上下文
        public OrdersController(TeamAProjectContext context)
        {
            _context = context;
        }

        // --- 這是您前端需要的 API 端點 ---
        // 它會處理像 GET /api/orders/20250814009202 這樣的請求
        [HttpGet("{orderIdString}")]
        public async Task<ActionResult<OrderHistoryDto>> GetOrderById(string orderIdString)
        {
            // 因為資料庫中的 OrderId 是 long 型別，而前端傳來的是 string，所以需要轉換
            if (!long.TryParse(orderIdString, out long orderId))
            {
                return BadRequest("無效的訂單編號格式。");
            }

            // 使用 LINQ 進行資料庫查詢
            var orderDto = await _context.EBookOrderMains
                .Where(o => o.OrderId == orderId) // 根據訂單 ID 篩選
                .Include(o => o.OrderStatus)  // 使用 Include 載入關聯的訂單狀態
                .Include(o => o.OrderItems)   // 載入這筆訂單所有的商品項目
                    .ThenInclude(oi => oi.EBook) // 再從商品項目，載入關聯的 EBook 資訊
                .Select(order => new OrderHistoryDto // 使用 Select 將查詢結果映射到 DTO
                {
                    OrderId = order.OrderId.ToString(),
                    OrderDate = order.OrderDateTime.ToString("yyyy/MM/dd HH:mm:ss"), // 格式化日期
                    Status = order.OrderStatus.StatusName, // 從關聯的 OrderStatus 取得狀態名稱
                    TotalAmount = order.TotalAmount,
                    Items = order.OrderItems.Select(item => new OrderHistoryItemDto
                    {
                        EbookId = item.EBookId ?? 0,
                        EbookName = item.ItemNameSnapshot, //
                        Price = item.UnitPriceAtPurchase,   //
                        Quantity = item.Quantity,           //
                        PrimaryCoverPath = (item.EBook != null && item.EBook.PrimaryCoverPath != null)
                                            ? $"{Request.Scheme}://{Request.Host}/{item.EBook.PrimaryCoverPath.TrimStart('/')}"
                                            : null
                    }).ToList()
                })
                .FirstOrDefaultAsync(); // 取得第一筆符合的資料，或 null

            if (orderDto == null)
            {
                // 如果在資料庫中找不到這筆訂單，回傳 404 Not Found
                return NotFound();
            }

            return Ok(orderDto); // 回傳 200 OK 以及 DTO 物件
        }
    }
}
