using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Fund.Dtos;
using prjSpecialTopicWebAPI.Features.Fund.Services;
using prjSpecialTopicWebAPI.Models;
using System.Security.Cryptography;

namespace prjSpecialTopicWebAPI.Features.Fund.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/fund/[controller]")] // /api/fund/FundProjects
    public class FundProjectsController : ControllerBase
    {
        private readonly IProjectService _svc;
        private readonly TeamAProjectContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IfundImageService _imageSvc;

        public FundProjectsController(
            IProjectService svc,
            TeamAProjectContext db,
            IWebHostEnvironment env,
            IfundImageService imageSvc)
        {
            _svc = svc;
            _db = db;
            _env = env;
            _imageSvc = imageSvc;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ProjectListDto>>> GetList(
            [FromQuery] string? status,
            [FromQuery] int? categoryId,
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12)
            => Ok(await _svc.GetListAsync(status, categoryId, keyword, page, pageSize));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectDetailDto>> GetById(int id)
        {
            var dto = await _svc.GetDetailAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDetailDto>> Create([FromBody] ProjectCreateDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.DonateProjectId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectUpdateDto dto)
        {
            var ok = await _svc.UpdateAsync(id, dto);
            return ok ? NoContent() : NotFound();
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusDto dto)
        {
            var ok = await _svc.ChangeStatusAsync(id, dto.Status);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var ok = await _svc.SoftDeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        // 提供另一條上傳路徑（與 FundImagesController 同邏輯、同儲存路徑）
        [HttpPost("{projectId:int}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProjectImage(int projectId, IFormFile file, [FromQuery] bool? isMain)
        {
            var exists = await _db.DonateProjects.AsNoTracking()
                .AnyAsync(p => p.DonateProjectId == projectId && !p.IsDeleted);
            if (!exists) return NotFound();

            if (file == null || file.Length == 0) return BadRequest("No file.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            if (!allowed.Contains(ext)) return BadRequest("檔案類型不允許。");

            var webRoot = _env.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
            var folder = Path.Combine(webRoot, "FundImages", "Projects", projectId.ToString());
            Directory.CreateDirectory(folder);

            // 產生 16位元十六進位檔名，總路徑 < 50 字元
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(8)).ToLower(); // 16 chars
            var fileName = $"{token}{ext}";

            var saveAbs = Path.Combine(folder, fileName);
            await using (var fs = System.IO.File.Create(saveAbs))
                await file.CopyToAsync(fs);

            var rel = Path.Combine("FundImages", "Projects", projectId.ToString(), fileName)
                        .Replace("\\", "/");

            var created = await _imageSvc.CreateUploadedAsync(projectId, rel, isMain ?? false);
            return Ok(created);
        }
    }
}
