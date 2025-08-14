using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Ebook.DTOs;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Ebook
{
    // 注意這個巢狀路由，它清楚地表示了圖片是屬於某本電子書的
    [Route("api/ebooks/{ebookId}/images")]
    [ApiController]
    public class EbookImagesController : ControllerBase
    {
        private readonly TeamAProjectContext _db;
        private readonly IWebHostEnvironment _env;

        public EbookImagesController(TeamAProjectContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        /// <summary>
        /// 取得指定書籍的所有圖片列表
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetImages(long ebookId)
        {
            var images = await _db.EBookImages
                .Where(i => i.EBookId == ebookId)
                .Select(i => new ImageDto
                {
                    ImageId = i.ImageId,
                    ImagePath = i.ImagePath,
                    IsPrimary = i.IsPrimary
                })
                .ToListAsync();
            return Ok(images);
        }

        /// <summary>
        /// 為指定的書籍上傳一張圖片
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UploadImage(long ebookId, IFormFile file)
        {
            var ebook = await _db.EBookMains.FindAsync(ebookId);
            if (ebook == null) return NotFound("找不到指定的書籍");
            if (file == null || file.Length == 0) return BadRequest("沒有上傳檔案");

            var uploadPath = Path.Combine(_env.WebRootPath, "images", "ebooks");
            Directory.CreateDirectory(uploadPath);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var newImage = new EBookImage
            {
                EBookId = ebookId,
                ImagePath = $"/images/ebooks/{uniqueFileName}",
                IsPrimary = false // 新上傳的預設不是主圖
            };

            _db.EBookImages.Add(newImage);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetImages), new { ebookId = ebookId }, newImage);
        }

        /// <summary>
        /// 將指定的圖片設為主封面圖
        /// </summary>
        [HttpPost("{imageId}/set-primary")]
        public async Task<IActionResult> SetPrimaryImage(long ebookId, long imageId)
        {
            var ebook = await _db.EBookMains.Include(e => e.EBookImages).FirstOrDefaultAsync(e => e.EbookId == ebookId);
            if (ebook == null) return NotFound("找不到書籍");

            var imageToSet = ebook.EBookImages.FirstOrDefault(i => i.ImageId == imageId);
            if (imageToSet == null) return NotFound("找不到指定的圖片");

            // 將同本書的其他圖片都設為非主圖
            foreach (var img in ebook.EBookImages)
            {
                img.IsPrimary = false;
            }

            // 將指定圖片設為主圖，並更新主表的快捷路徑
            imageToSet.IsPrimary = true;
            ebook.PrimaryCoverPath = imageToSet.ImagePath;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// 刪除指定的圖片
        /// </summary>
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage(long ebookId, long imageId)
        {
            var image = await _db.EBookImages.FirstOrDefaultAsync(i => i.EBookId == ebookId && i.ImageId == imageId);
            if (image == null) return NotFound();

            // 從硬碟刪除實體檔案
            var filePath = Path.Combine(_env.WebRootPath, image.ImagePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // 從資料庫刪除紀錄
            _db.EBookImages.Remove(image);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
