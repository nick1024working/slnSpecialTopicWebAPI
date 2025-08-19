using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query;
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
        public async Task<IActionResult> GetSellerBookList([FromQuery] BookListQuery query)
        {
            // HACK: 驗證政策尚未完成
            string userIdString = "22B888CB-32AB-4B07-96BF-228B60D3717A";
            Guid.TryParse(userIdString, out Guid userId);

            // 呼叫 Service Layer
            var result = await _usedBookService.GetUserBookListAsync(userId, query);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return Ok(result.Value);
        }

    }
}
