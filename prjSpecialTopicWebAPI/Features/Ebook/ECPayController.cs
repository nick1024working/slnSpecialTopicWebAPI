using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;
using System.Text;
using prjSpecialTopicWebAPI.Features.Ebook.Services;
using System; // 為了使用 GUID

namespace prjSpecialTopicWebAPI.Features.Ebook
{
    [Route("api/[controller]")]
    [ApiController]
    public class ECPayController : ControllerBase
    {
        private readonly ECPayService _ecpayService;
        private readonly TeamAProjectContext _context;
        private readonly IConfiguration _configuration;

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
            var order = await _context.EBookOrderMains
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == request.OrderId);

            if (order == null) return NotFound("找不到訂單");

            string itemName = string.Join("#", order.OrderItems.Select(i => i.ItemNameSnapshot));


            var ecpaySettings = _configuration.GetSection("Payments:Ecpay");

            // 【診斷測試】暫時將商品名稱和描述寫死為最簡單的英文字串。
            // 這是為了驗證 ECPay 伺服器是否因為特定書名中的特殊字元而產生 500 錯誤。
            // 為了穩定性，我們仍然使用簡化的 ItemName 和 TradeDesc
            // string itemName = "ProBookLand E-Book Purchase";
            string tradeDesc = $"Order ID: {order.OrderId}";



            string uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 8);
            // --- 【Demo 用的臨時修改】 ---
            // 在商品描述中加入當前的時間戳記 (Ticks)，確保每次請求的內容都獨一無二
            // Ticks 是一個非常精確的時間值，可以保證每次都不同
           // string tradeDesc = $"Order ID: {order.OrderId} : {DateTime.Now.Ticks}";
            string merchantTradeNo = Guid.NewGuid().ToString("N").Substring(0, 20);
            // itemName += $"_{DateTime.Now.Ticks}";

            itemName += $"_{uniqueSuffix}";
            tradeDesc += $"_{uniqueSuffix}";

            // 【最終修正】根據你的要求，將返回商店的 URL 直接指向「我的書櫃」

            string clientBackUrl = "http://localhost:4200/ebook/library";


           // string merchantTradeNo = Guid.NewGuid().ToString("N").Substring(0, 20);
            Console.WriteLine($"--- Generated MerchantTradeNo for ECPay: {merchantTradeNo} ---");

            var parameters = new Dictionary<string, string>
            {
                { "MerchantID", ecpaySettings["MerchantID"] },
                { "MerchantTradeNo", merchantTradeNo },
                { "MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") },
                { "PaymentType", "aio" },
                { "TotalAmount", order.TotalAmount.ToString("F0") },
                //{ "TradeDesc", "ProBookLand 電子書城" },
                { "TradeDesc", tradeDesc },
                { "ItemName", itemName },
                { "ReturnURL", ecpaySettings["ReturnURL"] },
                { "ChoosePayment", "Credit" },
                { "EncryptType", "1" },
                { "ClientBackURL", clientBackUrl },
                // 【關鍵修正 1】將我們的 OrderId 放在自訂欄位1，一起送去綠界
                { "CustomField1", order.OrderId.ToString() }
            };

            string checkMacValue = _ecpayService.GenerateCheckMacValue(parameters, ecpaySettings["HashKey"], ecpaySettings["HashIV"]);
            parameters.Add("CheckMacValue", checkMacValue);

            var sb = new StringBuilder();
            sb.Append("<!DOCTYPE html><html><body onload='document.forms[0].submit()'>");
            sb.Append($"<form method='post' action='{ecpaySettings["ApiUrl"]}'>");
            foreach (var param in parameters)
            {
                sb.Append($"<input type='hidden' name='{param.Key}' value='{param.Value}' />");
            }
            sb.Append("</form></body></html>");

            return Content(sb.ToString(), "text/html", Encoding.UTF8);
        }

        [HttpPost("Callback")]
        public async Task<IActionResult> Callback([FromForm] IFormCollection form)
        {
            var ecpaySettings = _configuration.GetSection("Payments:Ecpay");

            var receivedParams = form.Keys.Where(k => k != "CheckMacValue")
                                    .ToDictionary(k => k, k => form[k].ToString());
            string receivedMacValue = form["CheckMacValue"];
            string expectedMacValue = _ecpayService.GenerateCheckMacValue(receivedParams, ecpaySettings["HashKey"], ecpaySettings["HashIV"]);

            if (receivedMacValue != expectedMacValue)
            {
                // 在真實世界中，這裡應該要記錄嚴重的安全性 Log
                return BadRequest("CheckMacValue 驗證失敗");
            }

            // 【最終修正】現在我們可以安全地從 CustomField1 取回 OrderId
            string customField1 = receivedParams.ContainsKey("CustomField1") ? receivedParams["CustomField1"] : null;
            if (string.IsNullOrEmpty(customField1) || !long.TryParse(customField1, out long orderId))
            {
                Console.WriteLine("ECPay Callback 嚴重錯誤: CustomField1 中沒有有效的 OrderId。");
                return Content("1|OK");
            }

            string rtnCode = receivedParams["RtnCode"];

            var order = await _context.EBookOrderMains.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);

            // 確保訂單存在，且處於「待付款」狀態，避免重複處理
            if (order != null && order.OrderStatusId == 1)
            {
                if (rtnCode == "1") // 交易成功
                {
                    await CompleteOrder(order);
                }
                else
                {
                    order.OrderStatusId = 4; // 更新為「付款失敗」
                    await _context.SaveChangesAsync();
                }
            }

            return Content("1|OK");
        }

        private async Task CompleteOrder(EBookOrderMain order)
        {
            order.OrderStatusId = 2; // 已付款
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

      

