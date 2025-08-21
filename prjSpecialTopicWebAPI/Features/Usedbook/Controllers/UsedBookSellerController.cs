using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Controllers
{
    [ApiController]
    [Route("api/usedbooks/seller")]
    public class UsedbookSellerController : ControllerBase
    {
        private readonly UsedBookService _usedBookService;

        public UsedbookSellerController(
            UsedBookService usedBookService)
        {
            _usedBookService = usedBookService;
        }

        [HttpGet("books")]
        public async Task<ActionResult<IEnumerable<UserBookListItemDto>>> GetSellerBookList([FromQuery] BookListQuery query, CancellationToken ct)
        {
            // HACK: 驗證政策尚未完成
            string userIdString = "EBB03874-054F-4FEA-9AE8-02B8D05C4BB3";
            Guid.TryParse(userIdString, out Guid userId);

            // 呼叫 Service Layer
            var result = await _usedBookService.GetUserBookListAsync(userId, query, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return Ok(result.Value);
        }

    }
}
