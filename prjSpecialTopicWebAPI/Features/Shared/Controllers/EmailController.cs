using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Shared.DTOs;
using prjSpecialTopicWebAPI.Features.Shared.Service;

namespace prjSpecialTopicWebAPI.Features.Shared.Controllers
{
    /// <summary>
    /// 此 controller 僅供 DEMO 用途。
    /// </summary>
    [ApiController]
    [Route("api/email")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send/basic")]
        public async Task<IActionResult> SendToGmailBasic([FromBody] EmailRequest request, CancellationToken ct)
        {
            await _emailService.SendBasicEmailAsync(request.To, request.Subject, request.Arg1, request.Arg2, ct);

            return NoContent();
        }

        [HttpPost("send/order-created")]
        public async Task<IActionResult> SendToGmailOrderCreated([FromBody] EmailRequest request, CancellationToken ct)
        {
            await _emailService.SendOrderCreatedEmailAsync(request.To, request.Subject, request.Arg1, request.Arg2, ct);

            return NoContent();
        }
    }

}
