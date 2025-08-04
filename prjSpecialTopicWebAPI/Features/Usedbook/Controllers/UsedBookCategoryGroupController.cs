using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Usedbook.Application.Services;

namespace prjBookAppCoreMVC.Controllers.UsedBook
{
    [ApiController]
    [Route("api/usedbooks/category-groups")]
    public class UsedBookCategoryGroupController : ControllerBase
    {
        private readonly BookCategoryGroupService _bookCategoryGroupService;
        private readonly BookCategoryService _bookCategoryService;

        public UsedBookCategoryGroupController(
            BookCategoryGroupService bookCategoryGroupService,
            BookCategoryService bookCategoryService)
        {
            _bookCategoryGroupService = bookCategoryGroupService;
            _bookCategoryService = bookCategoryService;
        }
        // ========== 新增、更新、刪除 ==========

        [HttpPost]
        public async Task<ActionResult<int>> CreateBookCategoryGroup([FromBody] CreateBookCategoryGroupRequest request, CancellationToken ct)
        {
            var result = await _bookCategoryGroupService.CreateAsync(request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return CreatedAtAction(nameof(GetBookCategoryGroup), new { id = result.Value }, result.Value);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<int>> DeleteBookCategoryGroup([FromRoute] int id, CancellationToken ct)
        {
            var result = await _bookCategoryGroupService.DeleteByIdAsync(id, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> UpdateBookCategoryGroup([FromRoute] int id, [FromBody] UpdatePartialBookCategoryGroupRequest request, CancellationToken ct)
        {
            var result = await _bookCategoryGroupService.UpdateByIdAsync(id, request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        [HttpPut("order")]
        public async Task<ActionResult> UpdateBookCategoryGroupListOrder([FromBody] IReadOnlyList<UpdateOrderByIdRequest> request, CancellationToken ct)
        {
            var result = await _bookCategoryGroupService.UpdateAllOrderAsync(request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

        // ========== 查詢 ==========

        [HttpGet]
        public async Task<ActionResult<BookCategoryGroupDto>> GetBookCategoryGroupList(CancellationToken ct)
        {
            // 呼叫 Service Layer
            var result = await _bookCategoryGroupService.GetAllAsync(ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookCategoryGroupDto>> GetBookCategoryGroup([FromRoute] int id, CancellationToken ct)
        {
            // 呼叫 Service Layer
            var result = await _bookCategoryGroupService.GetByIdAsync(id, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

        // ========== 子類別路由 ==========

        [HttpPut("{id:int}/categories/order")]
        public async Task<ActionResult> UpdateBookCategoryListOrder([FromRoute] int id, [FromBody] IReadOnlyList<UpdateOrderByIdRequest> request, CancellationToken ct)
        {
            var result = await _bookCategoryService.UpdateAllOrderByGroupIdAsync(id, request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

        [HttpGet("{id:int}/categories")]
        public async Task<ActionResult<IEnumerable<BookCategoryDto>>> GetBookCategoryList([FromRoute] int id, CancellationToken ct)
        {
            var result = await _bookCategoryService.GetAllByGroupIdAsync(id, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

    }
}
