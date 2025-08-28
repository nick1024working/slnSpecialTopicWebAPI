using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Authentication;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Controllers
{
    [ApiController]
    [Route("api/usedbooks/sellers")]
    public class UsedbookSellerController : ControllerBase
    {
        private readonly AuthHelper _authHelper;
        private readonly UsedBookService _usedBookService;

        public UsedbookSellerController(
            AuthHelper authHelper,
            UsedBookService usedBookService)
        {
            _authHelper = authHelper;
            _usedBookService = usedBookService;
        }

        [HttpGet("books")]
        public async Task<ActionResult<IEnumerable<UserBookListItemDto>>> GetSellerBookList([FromQuery] BookListQuery query, CancellationToken ct)
        {
            // HACK: 驗證政策尚未完成，若 Cookie 無 userId 則使用固定值
            Guid userId = _authHelper.GetSeller(HttpContext) ?? Guid.Parse("EBB03874-054F-4FEA-9AE8-02B8D05C4BB3");

            // 呼叫 Service Layer
            var result = await _usedBookService.GetUserBookListAsync(userId, query, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return Ok(result.Value);
        }

    }
}
