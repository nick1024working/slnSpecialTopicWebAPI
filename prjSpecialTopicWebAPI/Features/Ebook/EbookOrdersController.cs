using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Ebook.DTOs;

using prjSpecialTopicWebAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
//using ECPay.Payment.Integration; // 假設使用官方 ECPay SDK，根據實際情況調整
using System.Net;
using System.Collections;
using System.Text;

namespace prjSpecialTopicWebAPI.Features.Ebook
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EbookOrdersController : ControllerBase
    {
        private readonly TeamAProjectContext _context;

        private readonly IConfiguration _configuration;


        // 為了暫存非同步回傳的虛擬帳號，我們使用靜態字典。真實專案應使用 Redis 或資料庫。
        private static readonly Dictionary<long, BankTransferDetails> _atmAccountCache = new Dictionary<long, BankTransferDetails>();

        public EbookOrdersController(TeamAProjectContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }



        //    // --- 信用卡付款 API ---
        //    [HttpPost("create-ecpay-payment")]
        //    public async Task<ActionResult<string>> CreateEcpayPayment([FromBody] CreatePaymentRequestDto requestDto)
        //    {
        //        var order = await _context.EBookOrderMains.FindAsync(requestDto.OrderId);
        //        if (order == null) return NotFound("找不到訂單");

        //        var ecpaySettings = _configuration.GetSection("Payments:Ecpay");
        //        string tradeNo = $"PBC{requestDto.OrderId}{DateTime.Now:mmssfff}";
        //        string htmlContent = string.Empty; // 用於接收輸出的 HTML

        //        // 建立參數字典
        //        var parameters = new Dictionary<string, string>
        //{
        //    { "MerchantID", ecpaySettings["MerchantID"] },
        //    { "MerchantTradeNo", tradeNo },
        //    { "MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") },
        //    { "TotalAmount", order.TotalAmount.ToString("F0") },
        //    { "TradeDesc", "ProBookLand 電子書城" },
        //    { "ItemName", "電子書一批" },
        //    { "ChoosePayment", "Credit" },
        //    { "EncryptType", "1" },
        //    { "ClientBackURL", $"http://localhost:4200/checkout/result?orderId={requestDto.OrderId}" },
        //    { "ReturnURL", $"https://{Request.Host}/api/EbookOrders/ecpay-callback" }
        //};

        //        // [最終修正] 將參數字典手動序列化成 URL Query String 格式
        //        string parameterString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));

        //        using (var oPayment = new ECPay.Payment.Integration.AllInOne())
        //        {
        //            oPayment.HashKey = ecpaySettings["HashKey"];
        //            oPayment.HashIV = ecpaySettings["HashIV"];
        //            try
        //            {
        //                // [最終修正] 遵從 (string, ref string) 簽章進行呼叫
        //                oPayment.CheckOutString(parameterString, ref htmlContent);
        //                return Ok(htmlContent);
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(500, $"支付表單生成失敗: {ex.Message}");
        //            }
        //        }
        //    }

        //    // --- ATM 付款 API ---
        //    [HttpPost("create-atm-payment")]
        //    public async Task<ActionResult<string>> CreateAtmPayment([FromBody] CreatePaymentRequestDto requestDto)
        //    {
        //        var order = await _context.EBookOrderMains.FindAsync(requestDto.OrderId);
        //        if (order == null) return NotFound("找不到訂單");

        //        var ecpaySettings = _configuration.GetSection("Payments:Ecpay");
        //        string tradeNo = $"PBN{requestDto.OrderId}{DateTime.Now:mmssfff}";
        //        string htmlContent = string.Empty; // 用於接收輸出的 HTML

        //        // 建立參數字典
        //        var parameters = new Dictionary<string, string>
        //{
        //    { "MerchantID", ecpaySettings["MerchantID"] },
        //    { "MerchantTradeNo", tradeNo },
        //    { "MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") },
        //    { "TotalAmount", order.TotalAmount.ToString("F0") },
        //    { "TradeDesc", "ProBookLand 電子書城" },
        //    { "ItemName", "電子書一批" },
        //    { "ChoosePayment", "ATM" },
        //    { "EncryptType", "1" },
        //    { "ExpireDate", "3" },
        //    { "PaymentInfoURL", $"https://{Request.Host}/api/EbookOrders/ecpay-callback" },
        //    { "ClientBackURL", $"http://localhost:4200/checkout/result?orderId={requestDto.OrderId}" }
        //};

        //        // [最終修正] 將參數字典手動序列化成 URL Query String 格式
        //        string parameterString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));

        //        using (var oPayment = new ECPay.Payment.Integration.AllInOne())
        //        {
        //            oPayment.HashKey = ecpaySettings["HashKey"];
        //            oPayment.HashIV = ecpaySettings["HashIV"];
        //            try
        //            {
        //                // [最終修正] 遵從 (string, ref string) 簽章進行呼叫
        //                oPayment.CheckOutString(parameterString, ref htmlContent);
        //                return Ok(htmlContent);
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(500, $"支付表單生成失敗: {ex.Message}");
        //            }
        //        }
        //    }
        //    // --- [核心修改] 接收 ECPay 所有回呼的 API ---
        //    [AllowAnonymous]
        //    [HttpPost("ecpay-callback")]
        //    public async Task<IActionResult> EcpayCallback([FromForm] IFormCollection form)
        //    {
        //        var ecpaySettings = _configuration.GetSection("Payments:Ecpay");
        //        var hashKey = ecpaySettings["HashKey"];
        //        var hashIV = ecpaySettings["HashIV"];
        //        var htFeedback = new Hashtable();
        //        foreach (string key in form.Keys) { htFeedback[key] = form[key].ToString(); }
        //        var sortedFeedback = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        //        foreach (DictionaryEntry entry in htFeedback) { sortedFeedback.Add((string)entry.Key, (string)entry.Value); }

        //        using (var oPayment = new AllInOne())
        //        {
        //            oPayment.HashKey = hashKey; oPayment.HashIV = hashIV;
        //            var errors = oPayment.CheckOutFeedback(sortedFeedback, ref htFeedback);
        //            if (errors.Any()) { return BadRequest("CheckMacValue 驗證失敗"); }
        //        }

        //        string merchantTradeNo = form["MerchantTradeNo"].ToString();
        //        string rtnCode = form["RtnCode"].ToString(); // 交易狀態碼 (1=成功)
        //        string paymentType = form["PaymentType"].ToString();

        //        if (!long.TryParse(new string(merchantTradeNo.Substring(3).Where(char.IsDigit).ToArray()), out long orderId))
        //        {
        //            return BadRequest("無法從 MerchantTradeNo 解析訂單 ID");
        //        }

        //        var order = await _context.EBookOrderMains.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);
        //        if (order == null) return NotFound("找不到對應訂單");

        //        // 如果訂單不是待付款狀態，直接回傳 OK，避免重複處理
        //        if (order.OrderStatusId != 1) return Content("1|OK");

        //        // [修改] 將訂單完成的邏輯抽成一個獨立方法
        //        if (rtnCode == "1") // 交易成功 (適用於信用卡一次付清 & ATM 付款完成)
        //        {
        //            await CompleteOrder(order);
        //        }
        //        else if (rtnCode == "2" && paymentType.StartsWith("ATM_")) // ATM 取號成功
        //        {
        //            var details = new BankTransferDetails
        //            {
        //                OrderId = orderId.ToString(),
        //                BankName = "中國信託商業銀行", // ECPay 合作銀行
        //                BankCode = form["BankCode"].ToString(),
        //                AccountNumber = form["vAccount"].ToString(),
        //                Amount = (int)Math.Round(order.TotalAmount, 0),
        //                PaymentDeadline = form["ExpireDate"].ToString()
        //            };
        //            _atmAccountCache[orderId] = details; // 暫存起來供前端查詢
        //        }

        //        return Content("1|OK");
        //    }

        /// <summary>
        /// [新增] 將訂單設為完成、書籍加入書櫃的共用方法
        /// </summary>
        private async Task CompleteOrder(EBookOrderMain order)
        {
            order.OrderStatusId = 2; // 狀態改為「已付款」
            order.LastModifiedDate = DateTime.UtcNow;

            foreach (var item in order.OrderItems)
            {
                if (item.EBookId.HasValue)
                {
                    var exists = await _context.EbookPurchaseds.AnyAsync(p => p.Uid == order.Uid && p.EBookId == item.EBookId.Value);
                    if (!exists)
                    {
                        _context.EbookPurchaseds.Add(new EbookPurchased
                        {
                            Uid = order.Uid,
                            EBookId = item.EBookId.Value,
                            PurchaseDateTime = DateTime.UtcNow,
                            LastReadTime = DateTime.UtcNow,
                        });
                    }
                }
            }
            await _context.SaveChangesAsync();
        }

        //// [新增] 提供給前端查詢 ATM 轉帳資訊的 API
        //[HttpGet("{orderId}/bank-details")]

        //// --- [核心修正] 修改路由，使其更獨特，避免與 GetOrderById 衝突 ---
        //[HttpGet("atm-details/{orderId}")]
        //public IActionResult GetBankTransferDetails(long orderId)
        //{
        //    if (_atmAccountCache.TryGetValue(orderId, out var details))
        //    {
        //        return Ok(details);
        //    }
        //    return NotFound("找不到該訂單的轉帳資訊，可能已過期或不存在。");
        //}



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


        // --- [新增] 取消訂單的 API 端點 ---
        [HttpPatch("{orderId}/cancel")] // 使用 PATCH 來更新訂單狀態
        public async Task<IActionResult> CancelOrder(long orderId)
        {
            var userIdString = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("無效的使用者識別碼。");
            }

            var order = await _context.EBookOrderMains
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.Uid == userId);

            if (order == null)
            {
                return NotFound("找不到您的訂單，或您無權操作此訂單。");
            }

            // 只有在「待付款」(ID=1) 狀態下才允許取消
            if (order.OrderStatusId != 1)
            {
                return BadRequest("只有待付款的訂單才能被取消。");
            }

            // 將訂單狀態更新為「已取消」(ID=3)
            order.OrderStatusId = 3;
            order.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "訂單已成功取消。" });
        }
    }
}
