using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Features.Usedbook.Utilities;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImageController : ControllerBase
    {
        private readonly ImageService _imageService;

        public ImageController(
            ImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateImage([FromForm] UploadImageRequest request, CancellationToken ct)
        {
            var queryResult = await _imageService.SaveImageAsync(request.File, Request, ct);
            if (!queryResult.IsSuccess)
                return BadRequest(queryResult.ErrorMessage);
            return Ok(queryResult.Value);
        }

        [HttpPost("batch")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateImages([FromForm] UploadImagesRequest request, CancellationToken ct)
        {
            var result = await _imageService.SaveImagesAsync(request.Files, Request, ct);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteImage(string id)
        {
            _imageService.DeleteImage(id);
            return NoContent();
        }

        // ========== 查詢 ==========
        [HttpGet]
        public IActionResult GetImageList()
        {
            var result = _imageService.GetImageList();
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Value);
        }

        [HttpGet("folders")]
        public IActionResult GetFolderList()
        {
            var result = _imageService.GetFolderList();
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Value);
        }

        // ========== 查詢圖片 URL ==========

        [HttpGet("{id}/main-url")]
        public IActionResult GetMainUrlById(string id)
        {
            var relativePath = _imageService.GetMainRelativePath(id);
            if (string.IsNullOrEmpty(relativePath))
                return NotFound();

            var filePath = "/" + relativePath.Replace("\\", "/");
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            return Ok($"{baseUrl}{filePath}");
        }

        [HttpGet("{id}/thumb-url")]
        public IActionResult GetThumbUrlById(string id)
        {
            var relativePath = _imageService.GetThumbRelativePath(id);
            if (string.IsNullOrEmpty(relativePath))
                return NotFound();

            var filePath = "/" + relativePath.Replace("\\", "/");
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            return Ok($"{baseUrl}{filePath}");
        }

        // ========== 查詢圖片 PhysicalFile ==========

        [HttpGet("{id}/main")]
        public IActionResult GetMainFileById(string id)
        {
            var filePath = _imageService.GetMainAbsolutePath(id);
            if (filePath is null) return NotFound();

            var contentType = FileHelper.GetContentType(filePath);
            return PhysicalFile(filePath, contentType);
        }

        [HttpGet("{id}/thumb")]
        public IActionResult GetThumbFileById(string id)
        {
            var filePath = _imageService.GetThumbAbsolutePath(id);
            if (filePath is null) return NotFound();

            var contentType = FileHelper.GetContentType(filePath);
            return PhysicalFile(filePath, contentType);
        }
    }
    
}
