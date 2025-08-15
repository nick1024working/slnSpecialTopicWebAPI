using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using prjSpecialTopicWebAPI.Features.Fund.Dtos;
using prjSpecialTopicWebAPI.Features.Fund.Services;
using prjSpecialTopicWebAPI.Models;
using System.IO;

namespace prjSpecialTopicWebAPI.Features.Fund.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/fund/[controller]")] // => /api/fund/FundPlans
    public class FundPlansController : ControllerBase
    {
        private readonly IPlanService _svc;
        private readonly TeamAProjectContext _db;
        private readonly IWebHostEnvironment _env;

        public FundPlansController(IPlanService svc, TeamAProjectContext db, IWebHostEnvironment env)
        {
            _svc = svc;
            _db = db;
            _env = env;
        }

        [HttpGet("byProject/{projectId:int}")]
        public async Task<ActionResult<IEnumerable<PlanDto>>> GetByProject(int projectId)
            => Ok(await _svc.GetByProjectAsync(projectId));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PlanDto>> Get(int id)
        {
            var dto = await _svc.GetAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<PlanDto>> Create([FromBody] PlanCreateDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.DonatePlanId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PlanUpdateDto dto)
        {
            var ok = await _svc.UpdateAsync(id, dto);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _svc.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        // 方案圖片上傳：POST /api/fund/FundPlans/{id}/image
        [HttpPost("{id:int}/image")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(PlanDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadImage([FromRoute] int id, IFormFile file) // ← 移除 [FromForm]
        {
            var plan = await _db.Set<DonatePlan>().AsNoTracking()
                                .FirstOrDefaultAsync(x => x.DonatePlanId == id);
            if (plan == null) return NotFound();

            if (file == null || file.Length == 0) return BadRequest("No file.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            if (!allowed.Contains(ext)) return BadRequest("檔案類型不允許。");

            var webRoot = _env.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
            var folder = Path.Combine(webRoot, "FundImages", "Plans");
            Directory.CreateDirectory(folder);

            var fileName = $"plan_{id}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";
            var saveAbs = Path.Combine(folder, fileName);
            await using (var fs = System.IO.File.Create(saveAbs))
                await file.CopyToAsync(fs);

            var rel = Path.Combine("FundImages", "Plans", fileName).Replace("\\", "/");

            _db.Add(new DonateImage
            {
                DonateProjectId = plan.DonateProjectId,
                DonatePlanId = id,
                DonateImagePath = rel,
                IsMain = null
            });
            await _db.SaveChangesAsync();

            var dto = await _svc.GetAsync(id);
            return Ok(dto);
        }
    }
}
