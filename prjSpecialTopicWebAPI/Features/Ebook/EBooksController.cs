using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Features.Ebook.DTOs;

namespace prjSpecialTopicWebAPI.Features.Ebook
{
    [Route("api/ebooks")]
    [ApiController]
    public class EbooksController : ControllerBase
    {
        private readonly TeamAProjectContext _db;

        public EbooksController(TeamAProjectContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 取得所有上架電子書的摘要列表
        /// </summary>
        /// <returns>電子書摘要列表</returns>
        //[HttpGet]
        //public async Task<IActionResult> GetEbooksList()
        //{
        //    var ebookSummaries = await _db.EBookMains
        //        .AsNoTracking()
        //        .Where(b => b.IsAvailable)
        //        .Select(b => new EBookSummaryDto
        //        {
        //            EbookId = b.EbookId,
        //            EbookName = b.EbookName,
        //            Author = b.Author,
        //            FixedPrice = b.FixedPrice,
        //            PrimaryCoverPath = b.PrimaryCoverPath
        //        })
        //        .ToListAsync();

        //    return Ok(ebookSummaries);
        //}
        // 在 EbooksController.cs 中
        //[HttpGet]
        //public async Task<IActionResult> GetEbooksList([FromQuery] string? search)
        //{
        //    var query = _db.EBookMains.AsNoTracking().Where(b => b.IsAvailable);

        //    // 如果 search 參數有值，就加入名稱或作者的過濾條件
        //    if (!string.IsNullOrWhiteSpace(search))
        //    {
        //        query = query.Where(b => b.EbookName.Contains(search) || b.Author.Contains(search));
        //    }

        //    var ebookSummaries = await query
        //        .Select(b => new EBookSummaryDto { /* ... */ })
        //        .ToListAsync();

        //    return Ok(ebookSummaries);
        //}

        /// <summary>
        /// 取得所有上架電子書的摘要列表（支援搜尋與分頁）
        /// </summary>
        /// <param name="search">搜尋關鍵字 (書名或作者)</param>
        /// <param name="pageNumber">頁碼 (預設為 1)</param>
        /// <param name="pageSize">每頁筆數 (預設為 10)</param>
        /// <returns>電子書摘要列表</returns>
        //[HttpGet]
        //public async Task<IActionResult> GetEbooksList(
        //    [FromQuery] string? search,
        //    [FromQuery] int pageNumber = 1,
        //    [FromQuery] int pageSize = 10)
        //{
        //    // 建立基礎查詢
        //    var query = _db.EBookMains.AsNoTracking().Where(b => b.IsAvailable);

        //    // 如果 search 參數有值，就加入名稱或作者的過濾條件
        //    if (!string.IsNullOrWhiteSpace(search))
        //    {
        //        query = query.Where(b => b.EbookName.Contains(search) || b.Author.Contains(search));
        //    }

        //    // [分頁邏輯]
        //    var ebookSummaries = await query
        //        .Select(b => new EBookSummaryDto
        //        {
        //            EbookId = b.EbookId,
        //            EbookName = b.EbookName,
        //            Author = b.Author,
        //            FixedPrice = b.FixedPrice,
        //            PrimaryCoverPath = b.PrimaryCoverPath
        //        })
        //        .Skip((pageNumber - 1) * pageSize) // 跳過前面頁數的資料
        //        .Take(pageSize)                   // 抓取目前頁面的資料
        //        .ToListAsync();

        //    return Ok(ebookSummaries);
        //}
        /// <summary>
        /// 取得所有上架電子書的摘要列表（支援搜尋與分頁）
        /// </summary>
        /// <param name="search">搜尋關鍵字 (書名或作者)</param>
        /// <param name="pageNumber">頁碼 (預設為 1)</param>
        /// <param name="pageSize">每頁筆數 (預設為 10)</param>
        /// <returns>包含分頁資訊的電子書摘要列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetEbooksList(
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // 1. 建立基礎查詢
            var query = _db.EBookMains.AsNoTracking().Where(b => b.IsAvailable);

            // 2. 如果 search 參數有值，就加入名稱或作者的過濾條件
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b => b.EbookName.Contains(search) || b.Author.Contains(search));
            }

            // 3. 取得符合條件的「總筆數」，這個計算必須在分頁(Skip/Take)之前
            var totalCount = await query.CountAsync();

            // 4. 套用分頁、排序(可選)與投影(Select)
            var items = await query
                .OrderByDescending(b => b.EbookId) // 預設用 ID 倒序，讓新書在前面
                .Select(b => new EBookSummaryDto
                {
                    EbookId = b.EbookId,
                    EbookName = b.EbookName,
                    Author = b.Author,
                    FixedPrice = b.FixedPrice,
                    PrimaryCoverPath = b.PrimaryCoverPath
                })
                .Skip((pageNumber - 1) * pageSize) // 跳過前面頁數的資料
                .Take(pageSize)                   // 抓取目前頁面的資料
                .ToListAsync();

            // 5. 建立包裝後的回應物件
            var response = new PaginatedResponseDto<EBookSummaryDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };

            return Ok(response);
        }

        /// <summary>
        /// 根據 ID 取得單本電子書的詳細資訊
        /// </summary>
        /// <param name="id">電子書 ID</param>
        /// <returns>單本電子書的詳細資料</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEbookDetail(long id)
        {
            // 查詢詳情時，需要關聯的資料，所以使用 Include
            var ebookEntity = await _db.EBookMains
                .AsNoTracking()
                .Include(b => b.Category)     // 載入分類
                .Include(b => b.Labels)       // 載入標籤
                .Include(b => b.EBookImages)  // [已補上] 載入這本書所有的圖片記錄
                .FirstOrDefaultAsync(b => b.EbookId == id);

            if (ebookEntity == null)
            {
                return NotFound($"找不到 ID 為 {id} 的電子書");
            }

            // 將查詢到的 Entity 手動映射到 Detail DTO
            var ebookDetail = new EBookDetailDto
            {
                EbookId = ebookEntity.EbookId,
                EbookName = ebookEntity.EbookName,
                Author = ebookEntity.Author,
                Publisher = ebookEntity.Publisher,
                BookDescription = ebookEntity.BookDescription,
                FixedPrice = ebookEntity.FixedPrice,
                ActualPrice = ebookEntity.ActualPrice,
                CategoryName = ebookEntity.Category.CategoryName,
                Labels = ebookEntity.Labels.Select(l => l.LabelName).ToList(),
                // [已補上] 加入圖片路徑的映射
                PrimaryCoverPath = ebookEntity.PrimaryCoverPath,
                ImagePaths = ebookEntity.EBookImages.Select(i => i.ImagePath).ToList()
            };

            return Ok(ebookDetail);
        }

        /// <summary>
        /// 新增一本書籍
        /// </summary>
        /// <param name="createDto">從請求 Body 傳入的電子書資料</param>
        /// <returns>新建立的書籍資料</returns>
        [HttpPost]
        public async Task<IActionResult> CreateEbook([FromBody] CreateEBookDto createDto)
        {
            var newEbook = new EBookMain
            {
                EbookName = createDto.EbookName,
                Author = createDto.Author,
                Publisher = createDto.Publisher,
                BookDescription = createDto.BookDescription,
                FixedPrice = createDto.FixedPrice,
                CategoryId = createDto.CategoryId,
                IsAvailable = false, // 新書預設為不上架
                EBookPosition = "default/path",
                EBookDataType = "EPUB",
            };

            if (createDto.LabelIds != null && createDto.LabelIds.Any())
            {
                var labels = await _db.Labels
                                      .Where(l => createDto.LabelIds.Contains(l.LabelId))
                                      .ToListAsync();
                newEbook.Labels = labels;
            }

            _db.EBookMains.Add(newEbook);
            await _db.SaveChangesAsync();

            var resultDto = new EBookSummaryDto
            {
                EbookId = newEbook.EbookId,
                EbookName = newEbook.EbookName,
                Author = newEbook.Author,
                FixedPrice = newEbook.FixedPrice,
                PrimaryCoverPath = newEbook.PrimaryCoverPath
            };

            return CreatedAtAction(nameof(GetEbookDetail), new { id = newEbook.EbookId }, resultDto);
        }

        /// <summary>
        /// 更新一本書籍的資料
        /// </summary>
        /// <param name="id">要更新的書籍 ID</param>
        /// <param name="updateDto">要更新的書籍資料</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEbook(long id, [FromBody] UpdateEBookDto updateDto)
        {
            var ebookToUpdate = await _db.EBookMains
                                         .Include(b => b.Labels)
                                         .FirstOrDefaultAsync(b => b.EbookId == id);

            if (ebookToUpdate == null)
            {
                return NotFound($"找不到 ID 為 {id} 的電子書");
            }

            ebookToUpdate.EbookName = updateDto.EbookName;
            ebookToUpdate.Author = updateDto.Author;
            ebookToUpdate.Publisher = updateDto.Publisher;
            ebookToUpdate.BookDescription = updateDto.BookDescription;
            ebookToUpdate.FixedPrice = updateDto.FixedPrice;
            ebookToUpdate.CategoryId = updateDto.CategoryId;
            ebookToUpdate.IsAvailable = updateDto.IsAvailable;

            ebookToUpdate.Labels.Clear();
            if (updateDto.LabelIds != null && updateDto.LabelIds.Any())
            {
                var labels = await _db.Labels
                                      .Where(l => updateDto.LabelIds.Contains(l.LabelId))
                                      .ToListAsync();
                ebookToUpdate.Labels = labels;
            }

            await _db.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// 刪除一本書籍
        /// </summary>
        /// <param name="id">要刪除的書籍 ID</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEbook(long id)
        {
            var ebookToDelete = await _db.EBookMains.FindAsync(id);

            if (ebookToDelete == null)
            {
                return NotFound($"找不到 ID 為 {id} 的電子書");
            }

            _db.EBookMains.Remove(ebookToDelete);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}