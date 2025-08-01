using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Authentication;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Usedbook.Application.Services;

namespace prjBookAppCoreMVC.Controllers.UsedBook
{
    [ApiController]
    [Route("api/usedbooks/sale-tags")]
    [Authorize(Roles = RoleNames.Admin)]
    public class UsedBookSaleTagController : ControllerBase
    {
        private readonly BookSaleTagService _bookSaleTagService;

        public UsedBookSaleTagController(BookSaleTagService saleTagService)
        {
            _bookSaleTagService = saleTagService;
        }

        /// <summary>
        /// 管理員新增促銷標籤。
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<int>> CreateSaleTag([FromBody] CreateSaleTagRequest request, CancellationToken ct)
        {
            var result = await _bookSaleTagService.CreateAsync(request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return CreatedAtAction(nameof(GetSaleTag), new { id = result.Value });
        }

        /// <summary>
        /// 管理員批次更新所有促銷標籤（通常用於順序調整）。
        /// </summary>
        [HttpPut]
        public async Task<ActionResult> UpdateAllSaleTags([FromBody] IReadOnlyList<UpdateBookSaleTagRequest> request, CancellationToken ct)
        {
            var result = await _bookSaleTagService.UpdateAllAsync(request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

        /// <summary>
        /// 修改指定促銷標籤的名稱，需擁有管理員權限。
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateSaleTagName([FromRoute] int id, [FromBody] UpdateBookSaleTagRequest request, CancellationToken ct)
        {
            var result = await _bookSaleTagService.UpdateSaleTagNameAsync(id, request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        /// <summary>
        /// 修改指定促銷標籤的名稱，需擁有管理員權限。
        /// </summary>
        [HttpPatch("{id:int}/active")]
        public async Task<ActionResult> UpdateSaleTagActiveStatus([FromRoute] int id, [FromBody] bool isActive, CancellationToken ct)
        {
            // 呼叫 Service Layer
            var result = await _bookSaleTagService.UpdateActiveStatusAsync(id, isActive, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookSaleTagDto>>> GetAllSaleTags(CancellationToken ct)
        {
            var result = await _bookSaleTagService.GetAllAsync(ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookSaleTagDto>> GetSaleTag([FromRoute] int id, CancellationToken ct)
        {
            // 呼叫 Service Layer
            var result = await _bookSaleTagService.GetByIdAsync(id, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }
    }
}
