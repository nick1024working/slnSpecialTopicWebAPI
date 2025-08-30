using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using System.Net;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Controllers
{
    [ApiController]
    [Route("api/usedbooks/payments")]
    public class UsedBookPaymentController : ControllerBase
    {
        private readonly UsedBookPaymentService _paymentSvc;
        private readonly IConfiguration _cfg;

        public UsedBookPaymentController(
            UsedBookPaymentService paymentSvc,
            IConfiguration cfg)
        {
            _paymentSvc = paymentSvc;
            _cfg = cfg;
        }

        [HttpGet("linepay/return")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmOrder([FromQuery] StateQuery query, CancellationToken ct)
        {
            var result = await _paymentSvc.ComfirmPaymentAsync(query, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return RedirectToResultPage("confirmed", result.Value);
        }

        [HttpGet("linepay/cancel")]
        [AllowAnonymous]
        public async Task<IActionResult> LinePayCancelOrder([FromQuery] StateQuery query, CancellationToken ct)
        {
            var result = await _paymentSvc.CancelPaymentAsync(query, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return RedirectToResultPage("canceled", result.Value);
        }

        private IActionResult RedirectToResultPage(string status, string orderNo, string? code = null)
        {
            var frontendResult = _cfg["Usedbook:CheckoutResultUrl"] ?? "http://localhost:4200/used-book/checkout-result";
            var uri = string.IsNullOrEmpty(code)
                ? $"{frontendResult}?status={WebUtility.UrlEncode(status)}&orderNo={WebUtility.UrlEncode(orderNo)}"
                : $"{frontendResult}?status={WebUtility.UrlEncode(status)}&orderNo={WebUtility.UrlEncode(orderNo)}&code={WebUtility.UrlEncode(code)}";

            return Redirect(uri);
        }
    }
}
