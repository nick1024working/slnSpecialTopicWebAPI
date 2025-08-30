using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Authentication;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Controllers
{
    [ApiController]
    [Route("api/usedbooks")]
    public class UsedbookAuthController : ControllerBase
    {
        private readonly AuthHelper _authHelper;
        private readonly ExternalDomainService _externalDomainService;

        public UsedbookAuthController(ExternalDomainService externalDomainService, AuthHelper authHelper)
        {
            _authHelper = authHelper;
            _externalDomainService = externalDomainService;
        }

        [HttpPut("current-seller")]
        public async Task<IActionResult> SetCurrentSeller([FromBody] SetCurrentSellerRequest req, CancellationToken ct)
        {
            var queryResult = await _externalDomainService.GetSellerListAsync(ct);
            if (!queryResult.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(queryResult.ErrorCode);

            if (!queryResult.Value.Any(res => res.Id == req.Id))
                return BadRequest();

            _authHelper.SetUser(req.Id, HttpContext);
            return NoContent();
        }

        [HttpGet("current-seller")]
        public ActionResult<Guid> GetCurrentSeller()
        {
            var result = _authHelper.GetUser(HttpContext);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpDelete("current-seller")]
        public IActionResult ClearCurrentSeller()
        {
            _authHelper.ClearUser(HttpContext);
            return NoContent();
        }

        [HttpGet("sellers")]
        public async Task<ActionResult<CurrentSellerDto>> GetSellers(CancellationToken ct)
        {
            var result = await _externalDomainService.GetSellerListAsync(ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

    }
}
