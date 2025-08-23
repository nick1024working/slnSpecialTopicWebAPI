using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Controllers
{
    [ApiController]
    [Route("api/usedbooks/books")]
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
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Guid>> CreateBook([FromForm] CreateBookRequest request, CancellationToken ct)
        {
            // HACK: 驗證政策尚未完成
            string userIdString = "EBB03874-054F-4FEA-9AE8-02B8D05C4BB3";
            Guid.TryParse(userIdString, out Guid userId);

            // 嘗試取出 claims 中的 userId
            //if (AuthHelper.GetUserId(User, _logger) is not Guid userId)
            //    return ErrorCodeToHttpResponseMapper.Map(ErrorCodes.Auth.Unauthorized);

            // 呼叫 Service Layer
            var result = await _bookService.CreateAsync(userId, request, Request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return CreatedAtAction(nameof(GetPubicDetail), new { bookId = result.Value }, result.Value);
        }

        [HttpPut("{bookId:Guid}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UpdateBook(
            [FromRoute] Guid bookId, [FromForm] UpdateBookRequest request, CancellationToken ct)
        {
            var result = await _bookService.UpdateAsync(bookId, request, Request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        // ========== 更改狀態 ==========

        [HttpPut("{bookId:Guid}/on-shelf")]
        public async Task<ActionResult<IEnumerable<int>>> UpdateBookOnShelfStatus(
            [FromRoute] Guid bookId, [FromBody] UpdateStatusRequest status, CancellationToken ct)
        {
            var result = await _bookService.UpdateOnShelfStatusAsync(bookId, status, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        [HttpPut("{bookId:Guid}/active")]
        public async Task<ActionResult<IEnumerable<int>>> UpdateBookActiveStatus(
            [FromRoute] Guid bookId, [FromBody] UpdateStatusRequest status, CancellationToken ct)
        {
            var result = await _bookService.UpdateActiveStatusAsync(bookId, status, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        [HttpPut("{bookId:Guid}/sold")]
        public async Task<ActionResult<IEnumerable<int>>> UpdateBookSoldStatus(
            [FromRoute] Guid bookId, [FromBody] UpdateStatusRequest status, CancellationToken ct)
        {
            var result = await _bookService.UpdateSoldStatusAsync(bookId, status, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        // ========== 查詢 ==========

        [HttpGet("{bookId:Guid}")]
        public async Task<ActionResult<PublicUsedBookDetailDto>> GetPubicDetail([FromRoute] Guid bookId, CancellationToken ct)
        {
            var result = await _bookService.GetPublicDetailByIdAsync(bookId, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return Ok(result.Value);
        }

        [HttpGet("payload/{bookId:Guid}")]
        public async Task<ActionResult<UpdateBookPayloadDto>> GetUpdatePayload([FromRoute] Guid bookId, CancellationToken ct)
        {
            var result = await _bookService.GetUpdatePayloadByIdAsync(bookId, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<PublicBookListItemDto>>> GetPublicBookList([FromQuery] BookListQuery query, CancellationToken ct)
        {
            var result = await _bookService.GetPublicListAsync(query);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return Ok(result.Value);
        }

        // ========== 子資源 - 圖片 ==========

        [HttpPost("{bookId:Guid}/images")]
        public async Task<ActionResult<IEnumerable<int>>> CreateBookImages(
            [FromRoute] Guid bookId, [FromBody] List<CreateUsedBookImageRequest> requestList)
        {
            var result = await _bookImageService.CreateAsync(bookId, requestList);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return StatusCode(StatusCodes.Status201Created, result.Value);
        }

        [HttpPut("{bookId:Guid}/images/order")]
        public async Task<ActionResult<IEnumerable<int>>> UpdateBookImagesOrder(
            [FromRoute] Guid bookId, [FromBody] UpdateOrderByIdRequest request, CancellationToken ct)
        {
            var result = await _bookImageService.UpdateOrderByBookIdAsync(bookId, request, ct);
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

        // ========== 子資源 - 圖片封面 ==========

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

        // ========== 子屬性 - 標籤 ==========

        [HttpPut("{bookId:Guid}/sale-tags/{tagId:int}")]
        public async Task<IActionResult> ApplyBookSaleTag([FromRoute] Guid bookId, [FromRoute] int tagId, CancellationToken ct)
        {
            var result = await _bookService.ApplyBookSaleTagAsync(bookId, tagId, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return NoContent();
        }

        [HttpDelete("{bookId:Guid}/sale-tags/{tagId:int}")]
        public async Task<IActionResult> RemoveBookSaleTag([FromRoute] Guid bookId, [FromRoute] int tagId, CancellationToken ct)
        {
            var result = await _bookService.RemoveBookSaleTagAsync(bookId, tagId, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return NoContent();
        }

        [HttpPut("sale-tags/batch")]
        public async Task<IActionResult> UpdateBookSaleTagBatch([FromBody] UpdateBookSaleTagRequest request, CancellationToken ct)
        {
            var result = await _bookService.UpdateBookSaleTagBatchAsync(request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return NoContent();
        }

        // ========== Excel ==========
        [HttpGet("export/example")]
        public async Task<IActionResult> ExportUploadExample(CancellationToken ct)
        {
            var result = await _bookService.ExportUploadExampleAsync(ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);

            return File(result.Value,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "範例.xlsx");
        }

        [HttpPost("import")]
        public async Task<ActionResult<IEnumerable<Guid>>> ImportBooks(IFormFile file, CancellationToken ct)
        {
            // HACK: 驗證政策尚未完成
            string userIdString = "EBB03874-054F-4FEA-9AE8-02B8D05C4BB3";
            Guid.TryParse(userIdString, out Guid userId);

            if (file == null || file.Length == 0)
                return BadRequest("請上傳 Excel 檔案");

            using var stream = file.OpenReadStream();
            var result = await _bookService.ImportBooks(userId, stream, ct);
            return Ok(result);
        }

    }
}
