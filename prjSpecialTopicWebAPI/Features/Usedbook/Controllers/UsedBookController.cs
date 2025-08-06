using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Controllers
{
    [ApiController]
    [Route("api/usedbooks/books")]
    [Produces("application/json")]
    public class UsedBookController : ControllerBase
    {
        private readonly UsedBookService _bookService;
        private readonly UsedBookImageService _bookImageService;

        public UsedBookController(
            UsedBookService bookService,
            UsedBookImageService bookImageService)
        {
            _bookService = bookService;
            _bookImageService = bookImageService;
        }

        // ========== 新增、更新、刪除 ==========

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateBook([FromBody] CreateBookRequest request, CancellationToken ct)
        {
            // HACK: 驗證政策尚未完成
            string userIdString = "22B888CB-32AB-4B07-96BF-228B60D3717A";
            Guid.TryParse(userIdString, out Guid userId);

            // 嘗試取出 claims 中的 userId
            //if (AuthHelper.GetUserId(User, _logger) is not Guid userId)
            //    return ErrorCodeToHttpResponseMapper.Map(ErrorCodes.Auth.Unauthorized);

            // 呼叫 Service Layer
            var result = await _bookService.CreateAsync(userId, request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return CreatedAtAction(nameof(GetPubicDetail), new { bookId = result.Value }, result.Value);
        }

        //[HttpPut("{bookId:Guid}")]
        //public async Task<ActionResult> UpdateBook([FromRoute] Guid bookId, [FromBody] UpdateBookRequest request, CancellationToken ct)
        //{
        //    var result = await _bookService.UpdateAsync(bookId, request, ct);
        //    if (!result.IsSuccess)
        //        return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
        //    return NoContent();
        //}

        [HttpDelete("{bookId:Guid}")]
        public async Task<ActionResult> DeleteBook([FromRoute] Guid bookId, CancellationToken ct)
        {
            var result = await _bookService.UpdateActiveStatusAsync(bookId, false, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        // ========== 查詢 ==========

        [HttpGet("{bookId:Guid}")]
        public async Task<ActionResult<PublicBookDetailDto>> GetPubicDetail([FromRoute] Guid bookId, CancellationToken ct)
        {
            var result = await _bookService.GetPubicDetailAsync(bookId, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublicBookListItemDto>>> GetPublicBookList([FromQuery] BookListQuery query, CancellationToken ct)
        {
            var result = await _bookService.GetPublicListAsync(query);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return Ok(result.Value);
        }

        // ========== 子資源圖片 ==========

        // HACK: 更新
        [HttpPost("{bookId:Guid}/images")]
        public async Task<ActionResult<IEnumerable<int>>> CreateBookImages(
            [FromRoute] Guid bookId, [FromBody] List<CreateUsedBookImageRequest> requestList)
        {
            var result = await _bookImageService.CreateAsync(bookId, requestList);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return StatusCode(StatusCodes.Status201Created, result.Value);
        }

        // HACK: 更新
        [HttpPut("{bookId:Guid}/images/order")]
        public async Task<ActionResult<IEnumerable<int>>> UpdateBookImagesOrder(
            [FromRoute] Guid bookId, [FromBody] List<UpdateOrderByIdRequest> requestList, CancellationToken ct)
        {
            var result = await _bookImageService.UpdateOrderByBookIdAsync(bookId, requestList, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

        [HttpGet("{bookId:Guid}/images")]
        public async Task<ActionResult<IEnumerable<BookImageDto>>> GetBookImages([FromRoute] Guid bookId, CancellationToken ct)
        {
            var result = await _bookImageService.GetByBookIdAsync(bookId, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return Ok(result.Value);
        }

        // ========== 子資源圖片 - 封面 ==========

        [HttpGet("{bookId:Guid}/cover")]
        public async Task<ActionResult<BookImageDto>> GetBookCover([FromRoute] Guid bookId, CancellationToken ct)
        {
            var result = await _bookImageService.GetCoverByBookIdAsync(bookId, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }

        [HttpPatch("{bookId:Guid}/cover")]
        public async Task<ActionResult> SetBookCover([FromRoute] Guid bookId, [FromBody] SetBookCoverRequest request, CancellationToken ct)
        {
            var result = await _bookImageService.SetCoverAsync(bookId, request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }
    }
}
