using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;

namespace prjBookAppCoreMVC.Controllers.UsedBook
{
    [ApiController]
    [Route("api/usedbooks/sale-tags")]
    //TODO: 等權限系統完成後，開啟以下註解
    //[Authorize(Roles = RoleNames.Admin)]
    public class UsedBookSaleTagController : ControllerBase
    {
        private readonly BookSaleTagService _bookSaleTagService;

        public UsedBookSaleTagController(BookSaleTagService saleTagService)
        {
            _bookSaleTagService = saleTagService;
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateSaleTag([FromBody] CreateSaleTagRequest request, CancellationToken ct)
        {
            var result = await _bookSaleTagService.CreateAsync(request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return CreatedAtAction(nameof(GetSaleTag), new { id = result.Value }, result.Value);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<int>> DeleteSaleTag([FromRoute] int id, CancellationToken ct)
        {
            var result = await _bookSaleTagService.DeleteByIdAsync(id, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> UpdateSaleTag([FromRoute] int id, [FromBody] UpdatePartialBookSaleTagRequest request, CancellationToken ct)
        {
            var result = await _bookSaleTagService.UpdateByIdAsync(id, request, ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return NoContent();
        }

        [HttpPut("order")]
        public async Task<ActionResult> UpdateAllSaleTagsOrder([FromBody] IReadOnlyList<UpdateOrderByIdRequest> request, CancellationToken ct)
        {
            var result = await _bookSaleTagService.UpdateAllOrderAsync(request, ct);
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookSaleTagDto>>> GetAllSaleTags(CancellationToken ct)
        {
            var result = await _bookSaleTagService.GetAllAsync(ct);
            if (!result.IsSuccess)
                return ErrorCodeToHttpResponseMapper.Map(result.ErrorCode);
            return Ok(result.Value);
        }
    }
}
