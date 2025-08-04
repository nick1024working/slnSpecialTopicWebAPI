using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Usedbook.Application.Services;

namespace prjBookAppCoreMVC.Controllers.UsedBook
{
    [ApiController]
    [Route("api/usedbooks/categories")]
    public class UsedBookCategoryController : ControllerBase
    {
        private readonly BookCategoryService _bookCategoryService;

        public UsedBookCategoryController(BookCategoryService bookCategoryService)
        {
            _bookCategoryService = bookCategoryService;
        }

        // ========== 新增、更新、刪除 ==========

        [HttpPost]
        public async Task<ActionResult<int>> CreateBookCategory([FromBody] CreateBookCategoryRequest request, CancellationToken ct)
        {
            var result = await _bookCategoryService.CreateAsync(request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return CreatedAtAction(nameof(GetBookCategory), new { id = result.Value }, result.Value);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<int>> DeleteBookCategory([FromRoute] int id, CancellationToken ct)
        {
            var result = await _bookCategoryService.DeleteByIdAsync(id, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> UpdateBookCategory([FromRoute] int id, [FromBody] UpdatePartialBookCategoryRequest request, CancellationToken ct)
        {
            var result = await _bookCategoryService.UpdateByIdAsync(id, request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        // ========== 查詢 ==========

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookCategoryDto>> GetBookCategory([FromRoute] int id, CancellationToken ct)
        {
            // 呼叫 Service Layer
            var result = await _bookCategoryService.GetByIdAsync(id, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }
    }
}
