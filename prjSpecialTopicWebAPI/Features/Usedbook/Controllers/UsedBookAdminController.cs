using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Controllers
{
    [ApiController]
    [Route("api/usedbooks/admin")]
    public class UsedbookAdminController : ControllerBase
    {
        private readonly UsedBookService _usedBookService;

        public UsedbookAdminController(
            UsedBookService usedBookService)
        {
            _usedBookService = usedBookService;
        }

        // TODO: 補呼叫鏈上 query + filter
        /// <summary>
        /// 管理員查詢所有書籍清單。
        /// </summary>
        [HttpGet("books")]
        public async Task<IActionResult> GetAdminBookList([FromQuery] BookListQuery query)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            // 呼叫 Service Layer
            var result = await _usedBookService.GetAdminBookListAsync(query);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return Ok(result.Value);
        }

    }
}
