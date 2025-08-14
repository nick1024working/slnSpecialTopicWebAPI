using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.Services
{
    public class ImageService
    {
        private readonly IWebHostEnvironment _env;

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<Result<ImageFileDto>> SaveImageAsync(IFormFile file, HttpRequest request, CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return Result<ImageFileDto>.Failure("No File", ErrorCodes.General.NotFound);

            var id = Guid.NewGuid().ToString("N");
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            var mainDir = Path.Combine(_env.WebRootPath, "uploads", "main");
            var thumbDir = Path.Combine(_env.WebRootPath, "uploads", "thumb");

            Directory.CreateDirectory(mainDir);
            Directory.CreateDirectory(thumbDir);

            var mainPath = Path.Combine(mainDir, $"{id}{ext}");
            var thumbPath = Path.Combine(thumbDir, $"{id}_thumb{ext}");

            using var sourceImage = await Image.LoadAsync(file.OpenReadStream(), ct);

            using var mainImage = sourceImage.Clone(ctx =>
                ctx.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = sourceImage.Width > sourceImage.Height
                        ? new Size(1080, 0)
                        : new Size(0, 1080)
                }));
            await mainImage.SaveAsync(mainPath, ct);

            using var thumbImage = sourceImage.Clone(ctx =>
                ctx.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Crop,
                    Size = new Size(200, 300)
                }));
            await thumbImage.SaveAsync(thumbPath, ct);

            var baseUrl = $"{request.Scheme}://{request.Host}";

            var dto = new ImageFileDto
            {
                Id = id,
                MainUrl = $"{baseUrl}/uploads/main/{id}{ext}",
                ThumbUrl = $"{baseUrl}/uploads/thumb/{id}_thumb{ext}",
                Width = sourceImage.Width,
                Height = sourceImage.Height,
            };

            return Result<ImageFileDto>.Success(dto);
        }

        public async Task<Result<IEnumerable<ImageFileDto>>> SaveImagesAsync(IEnumerable<IFormFile> files, HttpRequest request, CancellationToken ct)
        {
            List<ImageFileDto> dtoList = [];
            foreach (var file in files)
            {
                var result = await SaveImageAsync(file, request, ct);
                if (!result.IsSuccess)
                {
                    return Result<IEnumerable<ImageFileDto>>.Failure(
                        result.ErrorMessage ?? "圖片儲存失敗",
                        result.ErrorCode ?? ErrorCodes.General.Unexpected
                    );
                }
                dtoList.Add(result.Value);
            }
            return Result<IEnumerable<ImageFileDto>>.Success(dtoList);
        }

        public void DeleteImage(string id)
        {
            FileHelper.DeleteFile(id, _env.WebRootPath, "main");
            FileHelper.DeleteFile(id, _env.WebRootPath, "thumb", "_thumb");
        }

        // ========== 查詢 ==========

        public Result<IEnumerable<string>> GetImageList()
        {
            List<string> dtoList = [];
            string root = Path.Combine(_env.WebRootPath, "uploads/main");

            if (!Directory.Exists(root))
                return Result<IEnumerable<string>>.Failure("查無圖片", ErrorCodes.General.NotFound);

            foreach (var file in Directory.EnumerateFiles(root))
                dtoList.Add(Path.GetFileName(file));
            return Result<IEnumerable<string>>.Success(dtoList);
        }

        public string? GetMainAbsolutePath(string id)
            => FileHelper.GetAbsolutePathByImageId(id, _env.WebRootPath, Path.Combine("uploads", "main"));

        public string? GetMainRelativePath(string id)
            => FileHelper.GetRelativePathByImageId(id, _env.WebRootPath, Path.Combine("uploads", "main"));

        public string? GetThumbAbsolutePath(string id)
            => FileHelper.GetAbsolutePathByImageId(id, _env.WebRootPath, Path.Combine("uploads", "thumb"), "_thumb");

        public string? GetThumbRelativePath(string id)
            => FileHelper.GetRelativePathByImageId(id, _env.WebRootPath, Path.Combine("uploads", "thumb"), "_thumb");
    }
}
