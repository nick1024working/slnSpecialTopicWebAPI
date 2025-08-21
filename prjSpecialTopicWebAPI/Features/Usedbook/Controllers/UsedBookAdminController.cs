using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
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

        /// <summary>
        /// 管理員查詢所有書籍清單。
        /// </summary>
        [HttpGet("books")]
        public async Task<ActionResult<PagedResult<AdminBookListItemDto>>> GetAdminBookList([FromQuery] BookListQuery query)
        {
            var result = await _usedBookService.GetAdminBookListAsync(query);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return Ok(result.Value);
        }

        [HttpPut("books/sale-tags/batch")]
        public async Task<ActionResult<IEnumerable<UserBookListItemDto>>> UpdateBookSaleTagBatch([FromBody] UpdateBookSaleTagRequest request, CancellationToken ct)
        {
            var result = await _usedBookService.UpdateBookSaleTagBatchAsync(request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return NoContent();
        }

    }
}
