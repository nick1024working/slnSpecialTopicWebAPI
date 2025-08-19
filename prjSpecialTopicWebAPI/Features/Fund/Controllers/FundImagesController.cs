using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Fund.Dtos;
using prjSpecialTopicWebAPI.Features.Fund.Services;
using System.Security.Cryptography;

namespace prjSpecialTopicWebAPI.Features.Fund.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/fund/projects/{projectId:int}/images")]
    public class FundImagesController : ControllerBase
    {
        private readonly IfundImageService _svc;
        private readonly IWebHostEnvironment _env;

        public FundImagesController(IfundImageService svc, IWebHostEnvironment env)
        {
            _svc = svc;
            _env = env;
        }

        // 取得專案所有圖片
        [HttpGet]
        public async Task<ActionResult<List<ImageDto>>> GetList(int projectId)
            => Ok(await _svc.GetByProjectAsync(projectId));

        // 取得主圖
        [HttpGet("main")]
        public async Task<ActionResult<ImageDto>> GetMain(int projectId)
        {
            var dto = await _svc.GetMainAsync(projectId);
            return dto is null ? NotFound() : Ok(dto);
        }

        // 直接用路徑新增（非上傳）
        [HttpPost]
        public async Task<ActionResult<ImageDto>> CreateByPath(int projectId, [FromBody] ImageCreateDto dto)
        {
            if (dto.DonateProjectId != projectId) return BadRequest("ProjectId 不一致。");
            try
            {
                var created = await _svc.CreateAsync(dto);
                return CreatedAtAction(nameof(GetList), new { projectId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 上傳檔案
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ImageDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ImageDto>> Upload(
    int projectId,
    IFormFile file,
    [FromQuery] bool isMain = false)
        {
            if (file == null || file.Length == 0) return BadRequest("Empty file.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            if (!allowed.Contains(ext)) return BadRequest("檔案類型不允許。");

            var webRoot = _env.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
            var folder = Path.Combine(webRoot, "FundImages", "Projects", projectId.ToString());
            Directory.CreateDirectory(folder);

            // 產生 16位元十六進位檔名
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(8)).ToLower();
            var fileName = $"{token}{ext}";

            var saveAbs = Path.Combine(folder, fileName);
            await using (var fs = System.IO.File.Create(saveAbs))
                await file.CopyToAsync(fs);

            var rel = Path.Combine("FundImages", "Projects", projectId.ToString(), fileName)
                        .Replace("\\", "/");

            var created = await _svc.CreateUploadedAsync(projectId, rel, isMain);
            return CreatedAtAction(nameof(GetList), new { projectId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int projectId, int id, [FromBody] ImageUpdateDto dto)
        {
            var ok = await _svc.UpdateAsync(id, dto);
            return ok ? NoContent() : NotFound();
        }

        [HttpPatch("{id:int}/set-main")]
        public async Task<IActionResult> SetMain(int projectId, int id)
        {
            var ok = await _svc.SetMainAsync(projectId, id);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int projectId, int id)
        {
            var ok = await _svc.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
