using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Fund.Dtos;
using prjSpecialTopicWebAPI.Features.Fund.Services;

namespace prjSpecialTopicWebAPI.Features.Fund.Controllers
{
    [ApiController]
    [Route("api/fund/categories")] // ← 固定成 categories
    public class FundCategoriesController : ControllerBase
    {
        private readonly ICategoryService _svc;
        public FundCategoriesController(ICategoryService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<PagedResult<CategoryDto>>> GetPaged(
            [FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
            => Ok(await _svc.GetPagedAsync(keyword, page, pageSize));

        [HttpGet("all")]
        public async Task<ActionResult<List<CategoryDto>>> GetAll()
            => Ok(await _svc.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var dto = await _svc.GetByIdAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryCreateDto dto)
        {
            try
            {
                var created = await _svc.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.DonateCategoriesId }, created);
            }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
        {
            try
            {
                var ok = await _svc.UpdateAsync(id, dto);
                return ok ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, reason) = await _svc.DeleteAsync(id);
            if (ok) return NoContent();
            return reason switch
            {
                "NotFound" => NotFound(),
                "InUse" => Conflict(new { message = "此分類仍被專案使用，無法刪除。" }),
                _ => BadRequest()
            };
        }
    }
}