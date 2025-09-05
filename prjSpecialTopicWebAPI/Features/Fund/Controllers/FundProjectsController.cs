using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Fund.Dtos;
using prjSpecialTopicWebAPI.Features.Fund.Services;
using prjSpecialTopicWebAPI.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;


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
        private readonly ILogger<FundProjectsController> _logger;

        public FundProjectsController(
        IProjectService svc,
        TeamAProjectContext db,
        IWebHostEnvironment env,
        IfundImageService imageSvc,
        ILogger<FundProjectsController> logger)                 // ✅ 注入 logger
        {
            _svc = svc;
            _db = db;
            _env = env;
            _imageSvc = imageSvc;
            _logger = logger;                                       // ✅ 指派
        }
        private bool TryGetUid(out Guid uid)
        {
            uid = Guid.Empty;
            var uidStr =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ??
                User.FindFirst("uid")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(uidStr, out uid);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ProjectListDto>>> GetList(
            [FromQuery] string? status,
            [FromQuery] int? categoryId,
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12)
            => Ok(await _svc.GetListAsync(status, categoryId, keyword, page, pageSize));

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectDetailDto>> GetById(int id)
        {
            var dto = await _svc.GetDetailAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] ProjectCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var uidStr =
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
            User.FindFirst("uid")?.Value ??
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(uidStr, out var uid)) return Unauthorized();


            try
            {
                // 檢查：登入者必須存在於 Users
                var userExists = await _db.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Uid == uid);
                if (!userExists) return BadRequest(new { message = "Invalid User" });

                // 檢查：分類存在
                var catExists = await _db.DonateCategories
                    .AsNoTracking()
                    .AnyAsync(c => c.DonateCategoriesId == dto.DonateCategoriesId);
                if (!catExists) return BadRequest(new { message = "Invalid DonateCategoriesId" });

                if (dto.EndDate < dto.StartDate)
                    return BadRequest(new { message = "EndDate must be after StartDate" });

                // ✅ 傳 uid 給 Service
                var created = await _svc.CreateAsync(dto, uid);

                var id = created.DonateProjectId;
                return CreatedAtAction(nameof(GetById), new { id }, new { donateProjectId = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create project failed. DTO: {@dto}", dto);
                return Problem("Create project failed", statusCode: 500);
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectUpdateDto dto)
        {
            var ok = await _svc.UpdateAsync(id, dto);
            return ok ? NoContent() : NotFound();
        }

        [Authorize]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusDto dto)
        {
            var ok = await _svc.ChangeStatusAsync(id, dto.Status);
            return ok ? NoContent() : NotFound();
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var ok = await _svc.SoftDeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        // 提供另一條上傳路徑（與 FundImagesController 同邏輯、同儲存路徑）
        [Authorize]
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
