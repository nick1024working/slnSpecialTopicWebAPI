using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Shared.DTOs;
using prjSpecialTopicWebAPI.Features.Shared.Service;

namespace prjSpecialTopicWebAPI.Features.Shared.Controllers
{
    /// <summary>
    /// 此 controller 僅供 DEMO 用途。
    /// </summary>
    [ApiController]
    [Route("api/payments/line-pay")]
    public class PaymentController : ControllerBase
    {
        private readonly LinePayService _paymentService;

        public PaymentController(LinePayService paymentService)
        {
            _paymentService = paymentService;
        }

        // TODO: 轉由 BLL　呼叫　LinePay
        [HttpPost("request")]
        public async Task<ActionResult<LinePayRequestResponseDto>> RequestPayment([FromBody] LinePayPaymentRequestDto req, CancellationToken ct)
        {
            var result = await _paymentService.RequestLinePayPaymentAsync(req, ct);

            return Ok(result);
        }

        // TODO: 轉由 BLL　呼叫　LinePay
        [HttpGet("payments/requests/{transactionId}/check")]
        public async Task<ActionResult<LinePayRequestResponseDto>> CheckPayment([FromRoute] string transactionId, CancellationToken ct)
        {
            var result = await _paymentService.CheckLinePayPaymentAsync(transactionId, ct);

            return Ok(result);
        }

        // TODO: 轉由 BLL　呼叫　LinePay
        [HttpPost("confirm/{transactionId}")]
        public async Task<ActionResult<LinePayRequestResponseDto>> ConfirmPayment([FromRoute] string transactionId, [FromBody] LinePayPaymentConfirmDto req, CancellationToken ct)
        {
            var result = await _paymentService.ConfirmLinePayPaymentAsync(transactionId, req, ct);

            return Ok(result);
        }
    }
}
