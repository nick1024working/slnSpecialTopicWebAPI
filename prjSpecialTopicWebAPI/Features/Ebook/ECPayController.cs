using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Ebook.Services;
using prjSpecialTopicWebAPI.Models; // 你的 DbContext 和 Models
using System.Text;

namespace prjSpecialTopicWebAPI.Features.Ebook
{
    [Route("api/[controller]")]
    [ApiController]
    public class ECPayController : ControllerBase
    {
        private readonly ECPayService _ecpayService;
        private readonly TeamAProjectContext _context;
        private readonly IConfiguration _configuration;

        // 【步驟1】建立一個靜態、唯讀的 Random 實例，確保整個應用程式共用同一個。
        private static readonly Random _random = new Random();

        public ECPayController(ECPayService ecpayService, TeamAProjectContext context, IConfiguration configuration)
        {
            _ecpayService = ecpayService;
            _context = context;
            _configuration = configuration;
        }

        public class CreatePaymentRequestDto
        {
            public long OrderId { get; set; }
        }

        [HttpPost("CreatePayment")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequestDto request)
        {
            EBookOrderMain order; // 將 order 的宣告移到 try-catch 的外面

            try
            {
                // 程式在這裡嘗試連線資料庫並查詢訂單
                order = await _context.EBookOrderMains
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == request.OrderId);
            }
            catch (Exception ex)
            {
                // 【關鍵】請在這下面一行設定一個新的斷點！
                Console.WriteLine($"資料庫查詢時發生嚴重錯誤: {ex.Message}");

                // 當程式停在這裡時，我們就能看到詳細的錯誤訊息了
                return StatusCode(500, $"資料庫查詢失敗: {ex.Message}");
            }


            if (order == null) return NotFound("找不到訂單");
            if (order.OrderStatusId != 1) return BadRequest("此訂單狀態無法進行付款");

            string itemName = string.Join("#", order.OrderItems.Select(i => i.ItemNameSnapshot));
            var ecpaySettings = _configuration.GetSection("Payments:Ecpay");

            // 【步驟2】採用更穩健的 MerchantTradeNo 產生策略，確保唯一性且長度固定為20碼。
            string timestamp = DateTime.Now.ToString("MMddHHmmss"); // 10碼
            string orderIdPart = order.OrderId.ToString().PadLeft(4, '0'); // 至少4碼
            if (orderIdPart.Length > 4)
            {
                orderIdPart = orderIdPart.Substring(orderIdPart.Length - 4); // 取末4碼
            }
            string randomPart;
            lock (_random) // 鎖定 _random 物件，確保多執行緒安全
            {
                randomPart = _random.Next(0, 999).ToString("D3"); // 3碼
            }
            string merchantTradeNo = $"PBL{timestamp}{orderIdPart}{randomPart}"; // 3 + 10 + 4 + 3 = 20碼




            var requestData = new Dictionary<string, string>
        {
            { "MerchantID", ecpaySettings["MerchantID"] },
            { "MerchantTradeNo", merchantTradeNo },
            { "MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") },
            { "PaymentType", "aio" },
            { "TotalAmount", order.TotalAmount.ToString("F0") },
            { "TradeDesc", "ProBookLand 電子書城" },
            { "ItemName", itemName },
            { "ReturnURL", ecpaySettings["ReturnURL"] },
            { "ChoosePayment", "Credit" }, // 先寫死信用卡，之後可以讓前端傳入
            { "EncryptType", "1" },
            // ClientBackURL 加上 OrderId，方便前端知道是哪筆訂單回來的
            { "ClientBackURL", $"{ecpaySettings["ClientBackURL"]}/{order.OrderId}" }
        };

            // 計算 CheckMacValue 並加入到字典中
            requestData["CheckMacValue"] = _ecpayService.GenerateCheckMacValue(requestData, ecpaySettings["HashKey"], ecpaySettings["HashIV"]);

            // *** 核心：回傳一個會自動提交的 Form HTML 給前端 ***
            // 前端收到這個 HTML 後，瀏覽器會自動 POST 到綠界
            var sb = new StringBuilder();
            sb.Append("<!DOCTYPE html><html><body onload='document.forms[0].submit()'>");
            sb.Append($"<form method='post' action='{ecpaySettings["ApiUrl"]}'>");
            foreach (var param in requestData)
            {
                sb.Append($"<input type='hidden' name='{param.Key}' value='{param.Value}' />");
            }
            sb.Append("</form></body></html>");

            return Content(sb.ToString(), "text/html", Encoding.UTF8);
        }

        // [AllowAnonymous] 因為這是給綠界 Server 呼叫的，不會有 JWT Token
        [HttpPost("Callback")]
        public async Task<IActionResult> Callback([FromForm] IFormCollection form)
        {
            var ecpaySettings = _configuration.GetSection("Payments:Ecpay");

            // 1. 將接收到的資料（除了 CheckMacValue）重新計算一次 CheckMacValue
            var receivedParams = form.Keys.Where(k => k != "CheckMacValue")
                                    .ToDictionary(k => k, k => form[k].ToString());
            string receivedMacValue = form["CheckMacValue"];
            string expectedMacValue = _ecpayService.GenerateCheckMacValue(receivedParams, ecpaySettings["HashKey"], ecpaySettings["HashIV"]);

            // 2. 驗證 CheckMacValue
            if (receivedMacValue != expectedMacValue)
            {
                return BadRequest("CheckMacValue 驗證失敗");
            }

            // 3. 處理後續商業邏輯
            string merchantTradeNo = receivedParams["MerchantTradeNo"];
            string rtnCode = receivedParams["RtnCode"]; // 交易狀態碼 (1=成功)

            // 從 MerchantTradeNo 解析出我們的訂單 ID
            // 這邊的解析邏輯要跟你產生 MerchantTradeNo 的邏輯一致
            if (!long.TryParse(new string(merchantTradeNo.Substring(3).TakeWhile(char.IsDigit).ToArray()), out long orderId))
            {
                return BadRequest("無法從 MerchantTradeNo 解析訂單 ID");
            }

            var order = await _context.EBookOrderMains.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null) return Content("1|OK"); // 找不到訂單，但仍需回傳成功給綠界

            // 如果訂單不是待付款，代表可能處理過了，直接回傳 OK
            if (order.OrderStatusId != 1) return Content("1|OK");

            if (rtnCode == "1") // 交易成功
            {
                // *** 這裡就是你原本 CompleteOrder 的邏輯 ***
                await CompleteOrder(order);
            }
            else
            {
                // 交易失敗，可以更新訂單狀態為「付款失敗」
                order.OrderStatusId = 4; // 假設 4 = 付款失敗
                await _context.SaveChangesAsync();
            }

            // 4. **非常重要**：回傳 "1|OK" 給綠界，否則它會一直重試，直到逾時
            return Content("1|OK");
        }

        // 將你原本 EbookOrdersController 裡的訂單完成邏輯搬過來
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
    }
}
