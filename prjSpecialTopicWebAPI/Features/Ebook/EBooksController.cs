using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Features.Ebook.DTOs;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt; // <-- [新增] 請務必加入這一行！

namespace prjSpecialTopicWebAPI.Features.Ebook
{
    [Route("api/ebooks")]
    [ApiController]
    public class EbooksController : ControllerBase
    {
        private readonly TeamAProjectContext _db;
        private readonly IWebHostEnvironment _env; // [新增]


        public EbooksController(TeamAProjectContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
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
            [FromQuery] int? categoryId, // <-- [新增] 在這裡加上 categoryId 參數
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // 1. 建立基礎查詢
            IQueryable<EBookMain> query = _db.EBookMains.AsNoTracking()
                .Where(b => b.IsAvailable)
        .Include(b => b.Category) // 載入分類
        .Include(b => b.Labels); // 載入標籤
           

            // 2. 如果 search 參數有值，就加入名稱或作者的過濾條件
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b => b.EbookName.Contains(search) || b.Author.Contains(search));
            }

            // --- [新增] 在這裡插入新的 if 區塊 ---
            // 如果 categoryId 參數有值 (且不為0)，就加入分類的過濾條件
            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(b => b.CategoryId == categoryId.Value);
            }
            // --- 新增區塊結束 ---

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
                    // [修改] 組裝成完整的 URL
                    PrimaryCoverPath = (b.PrimaryCoverPath == null) ? null : $"{Request.Scheme}://{Request.Host}/{b.PrimaryCoverPath}",
                    // [修改] 新增 IsReadable 屬性的判斷邏輯
                    // 如果 EBookPosition 不是 null 也不是空字串，就代表這本書有檔案，是可閱讀的
                    IsReadable = !string.IsNullOrEmpty(b.EBookPosition),
                    // [新增] 在此處也加入 ActualPrice
                    ActualPrice = b.ActualPrice,

                    // --- [新增] 將 CategoryName 和 Labels 加入到 DTO 中 ---
                    CategoryName = b.Category.CategoryName, // 從關聯的 Category 物件取得名稱
                    Labels = b.Labels.Select(l => l.LabelName).ToList() // 將關聯的 Labels 集合轉為字串列表
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
                // [修改] 組裝成完整的 URL
                PrimaryCoverPath = (ebookEntity.PrimaryCoverPath == null) ? null : $"{Request.Scheme}://{Request.Host}/{ebookEntity.PrimaryCoverPath}",
                ImagePaths = ebookEntity.EBookImages.Select(i => $"{Request.Scheme}://{Request.Host}/{i.ImagePath}").ToList(),
                // [修改] 映射新增的欄位
                Isbn = ebookEntity.Isbn,
                Eisbn = ebookEntity.Eisbn,
                PublishedDate = ebookEntity.PublishedDate,
                Language = ebookEntity.Language,
                Translator = ebookEntity.Translator,
                EBookDataType = ebookEntity.EBookDataType,
                TotalSales = ebookEntity.Totalsales, // [新增] 從 Entity 映射總銷量到 DTO
            };

            return Ok(ebookDetail);
        }

        // EbooksController.cs

        // ... GetEbookDetail 方法結束後 ...

        /// <summary>
        /// 根據 ID 取得電子書的 PDF 檔案內容
        /// </summary>
        /// <param name="id">電子書 ID</param>
        /// <returns>PDF 檔案</returns>
        [HttpGet("{id}/file")] // 這個路由會匹配前端的請求 GET /api/ebooks/301/file
        public async Task<IActionResult> GetEbookFile(long id)
        {
            // 1. 根據 id 從資料庫中尋找書籍
            var ebook = await _db.EBookMains.FindAsync(id);

            // 2. 檢查書籍是否存在，以及 EBookPosition 欄位是否有儲存路徑
            if (ebook == null || string.IsNullOrEmpty(ebook.EBookPosition))
            {
                return NotFound("找不到電子書或檔案路徑紀錄");
            }

            // 3. 組合出檔案在伺服器上的完整實體路徑
            //    _env.WebRootPath 會指向您的 wwwroot 資料夾
            var filePath = Path.Combine(_env.WebRootPath, ebook.EBookPosition.TrimStart('/'));

            // 4. 檢查實體檔案是否存在
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("在伺服器上找不到對應的 PDF 檔案");
            }

            // 5. 讀取檔案內容
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            // 6. 將檔案以 "application/pdf" 的形式回傳給前端
            return File(fileBytes, "application/pdf");
        }

        // ... CreateEbook 方法開始前 ...


        ///// <summary>(舊版)
        ///// 取得所有已購買的電子書列表 (目前為演示用，未根據使用者篩選)
        ///// </summary>
        //[HttpGet("purchased")] // 這個路由會匹配 GET /api/ebooks/purchased
        //public async Task<IActionResult> GetPurchasedBooks()
        //{
        //    // 根據您的資料庫結構，我們需要從 EbookPurchaseds 出發
        //    var purchasedBooks = await _db.EbookPurchaseds
        //        .AsNoTracking()
        //        .Include(p => p.EBook) // 透過導覽屬性，自動 JOIN EBookMains 資料表
        //        .Select(p => new PurchasedBookDto
        //        {
        //            EbookId = p.EBook.EbookId,
        //            EbookName = p.EBook.EbookName,
        //            Author = p.EBook.Author,
        //            PrimaryCoverPath = (p.EBook.PrimaryCoverPath == null) ? null : $"{Request.Scheme}://{Request.Host}/{p.EBook.PrimaryCoverPath}",
        //            ReadingProgress = p.ReadingProgress,
        //            IsReadable = !string.IsNullOrEmpty(p.EBook.EBookPosition)
        //        })
        //        .ToListAsync();

        //    // 移除重複的書籍 (因為同本書可能被不同使用者購買)
        //    // 待未來實作依使用者篩選時，即可移除這段
        //    var distinctBooks = purchasedBooks
        //        .GroupBy(b => b.EbookId)
        //        .Select(g => g.First())
        //        .ToList();

        //    return Ok(distinctBooks);
        //}

        /// <summary>
        /// 取得當前登入使用者已購買的電子書列表
        /// </summary>
        [HttpGet("purchased")]
        [Authorize] // <-- [重點 1] 加入 Authorize 屬性，確保只有登入的使用者才能呼叫此 API
        public async Task<IActionResult> GetPurchasedBooks()
        {
            // --- [重點 2] 從 HttpContext 的使用者宣告中，動態取得登入者的 User ID ---
            // 根據您的 UserController.cs，UID 存放在 ClaimTypes.NameIdentifier (也就是 "sub")
            //  var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // [修改] 改用 JwtRegisteredClaimNames.Sub 來匹配新的 Token 格式
            var userIdString = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;


            // 如果在 Token 中找不到使用者 ID，代表使用者未登入或 Token 無效，回傳 401 未授權
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("無法識別使用者身分，請先登入");
            }

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("無效的使用者身分識別碼");
            }
            // --- 使用者 ID 取得結束 ---

            // --- [重點 3] 修改 LINQ 查詢，加入 Where 條件 ---
            var purchasedBooks = await _db.EbookPurchaseds
                .AsNoTracking()
                .Where(p => p.Uid == userId) // <-- 只篩選出符合當前登入者 UID 的購買紀錄
                .Include(p => p.EBook)
                .Select(p => new PurchasedBookDto
                {
                    EbookId = p.EBook.EbookId,
                    EbookName = p.EBook.EbookName,
                    Author = p.EBook.Author,
                    PrimaryCoverPath = (p.EBook.PrimaryCoverPath == null) ? null : $"{Request.Scheme}://{Request.Host}/{p.EBook.PrimaryCoverPath}",
                    ReadingProgress = p.ReadingProgress,
                    IsReadable = !string.IsNullOrEmpty(p.EBook.EBookPosition)
                })
                .ToListAsync();

            // [重點 4] 因為已經針對特定使用者查詢，不再需要 GroupBy 去除重複資料，可以直接回傳結果。

            return Ok(purchasedBooks);
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


        /// <summary>
        /// 上傳或更新指定書籍的電子書檔案
        /// </summary>
        /// <param name="ebookId">要上傳檔案的書籍 ID</param>
        /// <param name="file">上傳的電子書檔案 (例如 .epub 或 .pdf)</param>
        [HttpPost("{ebookId}/file")]
        public async Task<IActionResult> UploadEbookFile(long ebookId, IFormFile file)
        {
            // 1. 檢查書籍是否存在
            var ebook = await _db.EBookMains.FindAsync(ebookId);
            if (ebook == null)
            {
                return NotFound($"找不到 ID 為 {ebookId} 的書籍");
            }

            // 2. 驗證上傳的檔案
            if (file == null || file.Length == 0)
            {
                return BadRequest("未提供上傳檔案");
            }

            // (可選) 檢查副檔名
            var allowedExtensions = new[] { ".epub", ".pdf" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("不支援的檔案格式");
            }

            // 3. 規劃儲存路徑與檔名
            var uploadPath = Path.Combine(_env.WebRootPath, "ebook-files"); // e.g., wwwroot/ebook-files
                                                                            // 建立一個較不易重複的檔名，例如用書籍ID + GUID
            var uniqueFileName = $"{ebookId}-{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            // 確保儲存的資料夾存在
            Directory.CreateDirectory(uploadPath);

            // [進階處理] 如果這本書已經有舊檔案，先將其刪除
            if (!string.IsNullOrEmpty(ebook.EBookPosition))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, ebook.EBookPosition.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // 4. 將新檔案儲存到伺服器
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. 更新資料庫中的 EBookPosition 欄位
            ebook.EBookPosition = $"/ebook-files/{uniqueFileName}"; // 存入Web可存取的相對路徑
            await _db.SaveChangesAsync();

            // 6. 回傳成功訊息，包含新的檔案路徑
            return Ok(new { filePath = ebook.EBookPosition });
        }


        // [新增] 讀取進度的 DTO
        public class ReadingProgressDto
        {
            public int CurrentPage { get; set; }
            public string? ReadingProgress { get; set; }
        }

        // 檔案: EbooksController.cs

        /// <summary>
        /// 更新指定書籍的閱讀進度
        /// </summary>
        [HttpPost("purchased/progress")]
        [Authorize] // 這個操作必須是登入狀態才能執行
        public async Task<IActionResult> UpdateReadingProgress([FromBody] UpdateProgressDto progressDto)
        {
            // 1. 從 Token 中取得當前登入者的 User ID
            // var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // [修改] 改用 JwtRegisteredClaimNames.Sub 來匹配新的 Token 格式
            var userIdString = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("無法識別使用者身分");
            }
            var userId = Guid.Parse(userIdString);

            // 2. 根據 User ID 和 EbookId 找到對應的購買紀錄
            var purchaseRecord = await _db.EbookPurchaseds
                .FirstOrDefaultAsync(p => p.Uid == userId && p.EBookId == progressDto.EbookId);

            if (purchaseRecord == null)
            {
                return NotFound("找不到對應的購買紀錄");
            }

            // 3. 計算進度百分比並更新
            if (progressDto.TotalPages > 0)
            {
                double percentage = (double)progressDto.CurrentPage / progressDto.TotalPages * 100;
                purchaseRecord.ReadingProgress = Math.Round(percentage).ToString();
                // [修改] 儲存當前頁碼
                purchaseRecord.CurrentPage = progressDto.CurrentPage;
            }

            // 4. 更新最後閱讀時間
            purchaseRecord.LastReadTime = DateTime.UtcNow;

            // 5. 儲存變更到資料庫
            await _db.SaveChangesAsync();

            // 6. 回傳成功 (NoContent 表示成功但不需要回傳任何內容)
            return NoContent();
        }

        // [新增] 讀取進度方法
        [HttpGet("purchased/{ebookId}/progress")]
        [Authorize]
        public async Task<IActionResult> GetReadingProgress(long ebookId)
        {
             //var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           // var userIdString = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            // [修改] 改用 JwtRegisteredClaimNames.Sub
            var userIdString = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("無法識別使用者身分");
            }
            var userId = Guid.Parse(userIdString);

            var purchaseRecord = await _db.EbookPurchaseds
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Uid == userId && p.EBookId == ebookId);

            if (purchaseRecord?.CurrentPage != null)
            {
                // 找到紀錄，回傳頁碼和進度
                var progressDto = new ReadingProgressDto
                {
                    CurrentPage = purchaseRecord.CurrentPage.Value,
                    ReadingProgress = purchaseRecord.ReadingProgress
                };
                return Ok(progressDto);
            }

            // 沒有紀錄或頁碼為 null，回傳 404 或預設值
            return NotFound();
        }


        ///// <summary>
        ///// 取得所有排行榜的書籍資料
        ///// </summary>
        //[HttpGet("rankings")]
        //[AllowAnonymous] // 這個 API 是公開的，不需要登入
        //public async Task<IActionResult> GetRankingBooks()
        //{
        //    var rankings = await _db.EbookRecommends
        //        .AsNoTracking()
        //        .Include(r => r.RecType)  // 載入關聯的 RecommendationType
        //        .Include(r => r.Ebook)    // 載入關聯的 EBookMain
        //        .GroupBy(r => r.RecType.TypeName) // 根據 TypeName (例如 "暢銷排行榜") 進行分組
        //        .Select(group => new
        //        {
        //            TypeName = group.Key,
        //            Books = group.Select(r => new RankingBookDto
        //            {
        //                Id = r.Ebook.EbookId,
        //                Title = r.Ebook.EbookName,
        //                Author = r.Ebook.Author,
        //                CoverImage = (r.Ebook.PrimaryCoverPath == null) ? null : $"{Request.Scheme}://{Request.Host}/{r.Ebook.PrimaryCoverPath}",
        //                Price = (int)(r.Ebook.ActualPrice ?? r.Ebook.FixedPrice)
        //            }).ToList()
        //        })
        //        .ToDictionaryAsync(k => k.TypeName, v => v.Books);

        //    return Ok(rankings);
        //}

        /// <summary>
        /// 取得所有排行榜的書籍資料 (暢銷與熱門為動態產生)
        /// </summary>
        //[HttpGet("rankings")]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetRankingBooks()
        //{
        //    // =========================================================================
        //    // 1. 取得靜態的排行榜 (編輯推薦 RecTypeID=3, 新書推薦 RecTypeID=4)
        //    // =========================================================================
        //    var staticRankings = await _db.EbookRecommends
        //        .AsNoTracking()
        //        .Where(r => r.RecTypeId == 3 || r.RecTypeId == 4) // 只選取編輯推薦和新書推薦
        //        .Include(r => r.RecType)
        //        .Include(r => r.Ebook)
        //        .GroupBy(r => r.RecType.TypeName)
        //        .Select(group => new
        //        {
        //            TypeName = group.Key,
        //            Books = group.Select(r => new RankingBookDto
        //            {
        //                Id = r.Ebook.EbookId,
        //                Title = r.Ebook.EbookName,
        //                Author = r.Ebook.Author,
        //                CoverImage = (r.Ebook.PrimaryCoverPath == null) ? null : $"{Request.Scheme}://{Request.Host}/{r.Ebook.PrimaryCoverPath}",
        //                Price = (int?)(r.Ebook.ActualPrice ?? r.Ebook.FixedPrice)
        //            }).ToList()
        //        })
        //        .ToDictionaryAsync(k => k.TypeName, v => v.Books);

        //    // =========================================================================
        //    // 2. 動態產生暢銷排行榜 (根據 TotalSales)
        //    // =========================================================================
        //    var bestsellingBooks = await _db.EBookMains
        //        .AsNoTracking()
        //        .Where(b => b.IsAvailable)
        //        .OrderByDescending(b => b.Totalsales) // 根據總銷量降冪排序
        //        .Take(5) // 取前 5 名
        //        .Select(b => new RankingBookDto
        //        {
        //            Id = b.EbookId,
        //            Title = b.EbookName,
        //            Author = b.Author,
        //            CoverImage = (b.PrimaryCoverPath == null) ? null : $"{Request.Scheme}://{Request.Host}/{b.PrimaryCoverPath}",
        //            Price = (int?)(b.ActualPrice ?? b.FixedPrice)
        //        })
        //        .ToListAsync();

        //    // =========================================================================
        //    // 3. 動態產生熱門排行榜 (根據 TotalViews)
        //    // =========================================================================
        //    var hotBooks = await _db.EBookMains
        //        .AsNoTracking()
        //        .Where(b => b.IsAvailable)
        //        .OrderByDescending(b => b.Totalviews) // 根據總觀看數降冪排序
        //        .Take(5) // 取前 5 名
        //        .Select(b => new RankingBookDto
        //        {
        //            Id = b.EbookId,
        //            Title = b.EbookName,
        //            Author = b.Author,
        //            CoverImage = (b.PrimaryCoverPath == null) ? null : $"{Request.Scheme}://{Request.Host}/{b.PrimaryCoverPath}",
        //            Price = (int?)(b.ActualPrice ?? b.FixedPrice)
        //        })
        //        .ToListAsync();

        //    // =========================================================================
        //    // 4. 將所有結果合併到一個 Dictionary 中
        //    // =========================================================================
        //    var finalRankings = staticRankings; // 從靜態排行榜開始
        //    finalRankings["暢銷排行榜"] = bestsellingBooks; // 加入動態暢銷榜
        //    finalRankings["熱門排行榜"] = hotBooks;     // 加入動態熱門榜

        //    return Ok(finalRankings);
        //}

        /// <summary>
        /// 取得所有排行榜的書籍資料 (暢銷與優惠為動態產生)
        /// </summary>
        [HttpGet("rankings")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRankingBooks()
        {
            // =========================================================================
            // 1. 取得靜態的排行榜 (編輯推薦 RecTypeID=3, 新書推薦 RecTypeID=4)
            // =========================================================================
            var staticRankings = await _db.EbookRecommends
                .AsNoTracking()
                .Where(r => r.RecTypeId == 3 || r.RecTypeId == 4)
                .Include(r => r.RecType)
                .Include(r => r.Ebook)
                .GroupBy(r => r.RecType.TypeName)
                .Select(group => new
                {
                    TypeName = group.Key,
                    Books = group.Select(r => new RankingBookDto
                    {
                        Id = r.Ebook.EbookId,
                        Title = r.Ebook.EbookName,
                        Author = r.Ebook.Author,
                        CoverImage = (r.Ebook.PrimaryCoverPath == null) ? null : $"{Request.Scheme}://{Request.Host}/{r.Ebook.PrimaryCoverPath.TrimStart('/')}",
                        Price = (int?)(r.Ebook.ActualPrice ?? r.Ebook.FixedPrice),
                        FixedPrice = (int?)r.Ebook.FixedPrice // <-- [修改] 這裡也要回傳 FixedPrice
                    }).ToList()
                })
                .ToDictionaryAsync(k => k.TypeName, v => v.Books);

            // =========================================================================
            // 2. 動態產生暢銷排行榜 (根據 TotalSales)
            // =========================================================================
            var bestsellingBooks = await _db.EBookMains
                .AsNoTracking()
                .Where(b => b.IsAvailable)
                .OrderByDescending(b => b.Totalsales)
                .Take(5)
                .Select(b => new RankingBookDto
                {
                    Id = b.EbookId,
                    Title = b.EbookName,
                    Author = b.Author,
                    CoverImage = (b.PrimaryCoverPath == null) ? null : $"{Request.Scheme}://{Request.Host}/{b.PrimaryCoverPath.TrimStart('/')}",
                    Price = (int?)(b.ActualPrice ?? b.FixedPrice),
                    //    並不需要透過 b.Ebook.FixedPrice
                    FixedPrice = (int?)b.FixedPrice // <-- 正確的寫法
                })
                .ToListAsync();

            // =========================================================================
            // 3. [修改] 動態產生超值優惠榜 (根據折扣幅度)
            // =========================================================================
            var specialOfferBooks = await _db.EBookMains
                .AsNoTracking()
                // 篩選條件：必須上架、有實際售價、售價小於定價、且定價大於 0 (避免除以零)
                .Where(b => b.IsAvailable && b.ActualPrice.HasValue && b.ActualPrice < b.FixedPrice && b.FixedPrice > 0)
                // 排序條件：折扣幅度由大到小排序。 (定價-售價)/定價 越大，代表折扣越多
                .OrderByDescending(b => (b.FixedPrice - b.ActualPrice) / b.FixedPrice)
                .Take(5) // 取折扣最多的前 5 名
                .Select(b => new RankingBookDto
                {
                    Id = b.EbookId,
                    Title = b.EbookName,
                    Author = b.Author,
                    CoverImage = (b.PrimaryCoverPath == null) ? null : $"{Request.Scheme}://{Request.Host}/{b.PrimaryCoverPath.TrimStart('/')}",
                    Price = (int?)(b.ActualPrice ?? b.FixedPrice),
                    FixedPrice = (int?)b.FixedPrice // <-- [修改] 回傳 FixedPric
                })
                .ToListAsync();

            // =========================================================================
            // 4. 將所有結果合併到一個 Dictionary 中
            // =========================================================================
            var finalRankings = staticRankings;
            finalRankings["暢銷排行榜"] = bestsellingBooks;
            finalRankings["超值優惠榜"] = specialOfferBooks; // <-- [修改] 使用新的 Key 和資料

            return Ok(finalRankings);
        }




    }
}