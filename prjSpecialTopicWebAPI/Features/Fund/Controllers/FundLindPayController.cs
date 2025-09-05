using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Fund.Services;
using prjSpecialTopicWebAPI.Features.Shared.Extensions;
using prjSpecialTopicWebAPI.Features.Shared.DTOs;
using prjSpecialTopicWebAPI.Features.Shared.Service;

namespace prjSpecialTopicWebAPI.Features.Fund.Controllers;

[ApiController]
[Route("api/fund/linepay")]
public class FundLinePayController : ControllerBase
{
    private readonly LinePayService _linePay;
    private readonly IFundOrderService _orders;
    private readonly IConfiguration _cfg;
    private const string SessionKey = "LINEPAY:FUND";

    public FundLinePayController(LinePayService linePay, IFundOrderService orders, IConfiguration cfg)
    { _linePay = linePay; _orders = orders; _cfg = cfg; }

    /// <summary>把前端 baseUrl + donePath + query 安全拼起來（可在設定檔改 donePath）</summary>
    private string Front(string pathOrQuery)
    {
        var baseUrl = _cfg["Frontend:BaseUrl"] ?? _cfg["FrontEnd:Origin"] ?? "http://localhost:4200";
        var donePath = "/fund/fund-plan-done";
        var finalPath =
            pathOrQuery.StartsWith("?")
            ? (string.IsNullOrWhiteSpace(donePath) ? "/fund/fund-plan-done" : donePath) + pathOrQuery
            : pathOrQuery;

        var baseUri = new Uri(baseUrl.TrimEnd('/') + "/");
        return new Uri(baseUri, finalPath.TrimStart('/')).ToString();
    }

    [HttpGet("confirm")]
    public async Task<IActionResult> Confirm([FromQuery] string? transactionId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
            return Redirect(Front("?code=NO_TXN"));

        var ctx = HttpContext.Session.GetObject<FundLinePaySessionCtx>(SessionKey);
        if (ctx is null)
            return Redirect(Front("?code=NOSESSION"));

        var confirmReq = new LinePayPaymentConfirmDto
        {
            Amount = ctx.Amount,
            Currency = string.IsNullOrWhiteSpace(ctx.Currency) ? "TWD" : ctx.Currency
        };

        var result = await _linePay.ConfirmLinePayPaymentAsync(transactionId, confirmReq, ct);
        if (result?.ReturnCode == "0000")
            await _orders.MarkPaidAsync(ctx.Uid, ctx.OrderId, "LINEPay");

        HttpContext.Session.RemoveObject(SessionKey);

        var code = result?.ReturnCode ?? "UNKNOWN";
        var q = $"?orderId={ctx.OrderId}&transactionId={Uri.EscapeDataString(transactionId)}&code={Uri.EscapeDataString(code)}";
        return Redirect(Front(q)); // → http(s)://localhost:4200/<donePath>?orderId=...&code=OK
    }

    [HttpGet("cancel")]
    public IActionResult Cancel()
    {
        var ctx = HttpContext.Session.GetObject<FundLinePaySessionCtx>(SessionKey);
        HttpContext.Session.RemoveObject(SessionKey);

        var q = ctx is null ? "?code=CANCEL" : $"?orderId={ctx.OrderId}&code=CANCEL";
        return Redirect(Front(q));
    }
}
