using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Shared.Service;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;

namespace prjSpecialTopicWebAPI.Features.Shared.Controllers
{
    [ApiController]
    [Route("api/payments/line-pay")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPayment([FromBody] PaymentRequestDto req, CancellationToken ct)
        {
            // 可加 ModelState 驗證
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _paymentService.RequestLinePayPayment(req, ct);

            return StatusCode(result.StatusCode, result.RawBody);
        }
    }
}
