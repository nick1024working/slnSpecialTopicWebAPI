using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Features.Usedbook.Enums;
using prjSpecialTopicWebAPI.Models;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    public class UsedBookRepository
    {
        private readonly TeamAProjectContext _db;

        public UsedBookRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        // ========== 查詢實體 ==========

        /// <summary>
        /// 直接返回書本實體 (含促銷標籤)。
        /// </summary>
        public async Task<UsedBook?> GetEntityByIdWithSaleTagsAsync(Guid id, CancellationToken ct = default) =>
            await _db.UsedBooks.Include(b => b.Tags).SingleOrDefaultAsync(b => b.Id == id, ct);

        /// <summary>
        /// 直接返回書本實體 (含促銷標籤)。
        /// </summary>
        public async Task<UsedBook?> GetEntityByIdWithCountyIdAsync(Guid id, CancellationToken ct = default) =>
            await _db.UsedBooks.Include(b => b.SellerDistrict).SingleOrDefaultAsync(b => b.Id == id, ct);


        /// <summary>
        /// 直接返回書本實體。
        /// </summary>
        public async Task<UsedBook?> GetEntityByIdAsync(Guid id, CancellationToken ct = default) =>
            await _db.UsedBooks.SingleOrDefaultAsync(b => b.Id == id, ct);

        // ========== 新增、更新 ==========

        public void Add(UsedBook entity) =>
            _db.UsedBooks.Add(entity);

        public async Task<bool> UpdateOnShelfStatusAsync(Guid id, bool status, CancellationToken ct = default)
        {
            var result = await _db.UsedBooks.SingleOrDefaultAsync(b => b.Id == id, ct);
            if (result == null)
                return false;
            result.IsOnShelf = status;
            result.UpdatedAt = DateTime.UtcNow;
            return true;
        }

        public async Task<bool> UpdateSoldStatusAsync(Guid id, bool status, CancellationToken ct = default)
        {
            var result = await _db.UsedBooks.SingleOrDefaultAsync(b => b.Id == id, ct);
            if (result == null)
                return false;
            result.IsSold = status;
            result.UpdatedAt = DateTime.UtcNow;
            return true;
        }

        public async Task<bool> UpdateActiveStatusAsync(Guid id, bool status, CancellationToken ct = default)
        {
            var result = await _db.UsedBooks.SingleOrDefaultAsync(b => b.Id == id, ct);
            if (result == null)
                return false;
            result.IsActive = status;
            result.UpdatedAt = DateTime.UtcNow;
            return true;
        }

        // ========== 查詢 ==========

        /// <summary>
        /// 根據 ID 查詢書本完整資訊 (關聯欄位已用字串顯示)。
        /// </summary>
        public async Task<UsedBookDetailQueryResult?> GetDetailByIdAsync(Guid id, CancellationToken ct = default)
        {
            var queryResult = await _db.UsedBooks
                .Where(b => b.Id == id)
                .Select(b => new UsedBookDetailQueryResult
                {
                    Id = b.Id,
                    SellerId = b.SellerId,
                    SellerCountyName = b.SellerDistrict.County.Name,
                    SellerDistrictName = b.SellerDistrict.Name,
                    SalePrice = b.SalePrice,
                    Title = b.Title,
                    Authors = b.Authors,
                    CategoryName = b.Category.Name,
                    ConditionRatingName = b.ConditionRating.Name,

                    ConditionDescription = b.ConditionDescription,
                    Edition = b.Edition,
                    Publisher = b.Publisher,
                    PublicationDate = b.PublicationDate,
                    Isbn = b.Isbn,
                    BindingName = b.Binding != null ? b.Binding.Name : "",
                    LanguageName = b.Language != null ? b.Language.Name : "",
                    Pages = b.Pages,
                    ContentRatingName = b.ContentRating.Name,

                    IsOnShelf = b.IsOnShelf,
                    IsSold = b.IsSold,
                    IsActive = b.IsActive,
                    Slug = b.Slug,

                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .SingleOrDefaultAsync(ct);

            return queryResult;
        }

        /// <summary>
        /// 查詢公開書本清單 (清單項目資料，非詳細資料)。
        /// </summary>
        public async Task<PagedResult<PublicBookListItemQueryResult>> GetPublicBookListAsync(
            Expression<Func<UsedBook, bool>> predicate,
            Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy,
            PagingQuery paging,
            CancellationToken ct = default)
        {
            var pageIndex = Math.Max(0, paging.PageIndex);
            var pageSize = Math.Clamp(paging.PageSize, 1, 100);

            // 基底查詢（不 Include，先 Count）
            var baseQuery = _db.UsedBooks
                .AsNoTracking()
                .Where(b => b.IsActive && b.IsOnShelf)
                .Where(predicate)
                .Where(b => b.UsedBookImages.Any(i => i.IsCover));
            var total = await baseQuery.CountAsync(ct);

            // 排序 + 分頁 + 投影（一次把封面/分類/標籤取出）
            var items = await orderBy(baseQuery)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(b => new PublicBookListItemQueryResult
                {
                    CoverStorageProvider = (StorageProvider)b.UsedBookImages
                        .Where(i => i.IsCover)
                        .Select(i => i.StorageProvider)
                        .FirstOrDefault(),
                    CoverObjectKey = b.UsedBookImages
                        .Where(i => i.IsCover)
                        .Select(i => i.ObjectKey)
                        .FirstOrDefault() ?? "",

                    Id = b.Id,
                    Title = b.Title,
                    SalePrice = b.SalePrice,
                    Authors = b.Authors,
                    ConditionRating = b.ConditionRating.Name,

                    Category = new IdNameDto
                    {
                        Id = b.Category.Id,
                        Name = b.Category.Name
                    },
                    SaleTagList = b.Tags.Select(t => new IdNameDto
                    {
                        Id = t.Id,
                        Name = t.Name
                    }).ToList(),

                    Slug = b.Slug,

                })
                .AsSplitQuery()
                .ToListAsync(ct);

            // 組 PagedResult
            var result = new PagedResult<PublicBookListItemQueryResult>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRows = total
            };

            return result;
        }

        /// <summary>
        /// 根據 UserId 查詢該使用者書本清單 (清單項目資料，非詳細資料)。
        /// </summary>
        public async Task<IReadOnlyList<UserBookListItemQueryResult>> GetUserBookListAsync(
            Guid userId,
            Expression<Func<UsedBook, bool>> predicate,
            Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy,
            CancellationToken ct = default)
        {
            IQueryable<UsedBook> query = _db.UsedBooks
            .AsNoTracking()
            .Where(b => b.SellerId == userId && b.IsActive)
            .Where(predicate);

            query = orderBy(query);

            // 排序 + 分頁 + 投影（一次把封面/分類/標籤取出）
            var result = await query
                .AsNoTracking()
                .Select(b => new UserBookListItemQueryResult
                {
                    CoverStorageProvider = (StorageProvider)b.UsedBookImages
                        .Where(i => i.IsCover)
                        .Select(i => i.StorageProvider)
                        .FirstOrDefault(),
                    CoverObjectKey = b.UsedBookImages
                        .Where(i => i.IsCover)
                        .Select(i => i.ObjectKey)
                        .FirstOrDefault() ?? "",

                    Id = b.Id,
                    Title = b.Title,
                    SellerId = b.SellerId,
                    SalePrice = b.SalePrice,
                    ConditionRating = b.ConditionRating.Name,

                    IsOnShelf = b.IsOnShelf,
                    IsSold = b.IsSold,
                    Slug = b.Slug,

                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt

                })
                .AsSplitQuery()
                .ToListAsync(ct);

            return result;

            /*
            // 1. 建立查詢（包含關聯載入與篩選條件）
            var query = _db.UsedBooks
                .Where(predicate)
                .Where(b => b.SellerId == userId && b.IsActive)
                .Include(b => b.ConditionRating);

            // 2. 排序條件
            var orderedQuery = orderBy(query);

            // 3. 分頁條件
            var pagedBooks = await orderedQuery
                //.Skip(pageIndex * pageSize)
                //.Take(pageSize)
                .ToListAsync(ct);        // NOTE: 此處連線把 DB 端資料載入記憶體

            // 4. 封面快取
            // NOTE: 此處再次連線，優點是分開處理可讀性+維護性+SQL好寫
            var bookIds = pagedBooks.Select(b => b.Id).ToList();
            var coverDict = await _db.UsedBookImages
                .Where(img => bookIds.Contains(img.BookId) && img.IsCover)
                .ToDictionaryAsync(img => img.BookId, ct);

            // 5. 投影成結果
            var result = pagedBooks
                .Where(b => coverDict.ContainsKey(b.Id))
                .Select(b => new UserBookListItemQueryResult
                {
                    CoverStorageProvider = (StorageProvider)coverDict[b.Id].StorageProvider,
                    CoverObjectKey = coverDict[b.Id].ObjectKey,

                    Id = b.Id,
                    Title = b.Title,
                    SellerId = b.SellerId,
                    SalePrice = b.SalePrice,
                    ConditionRating = b.ConditionRating?.Name ?? "",

                    IsOnShelf = b.IsOnShelf,
                    IsSold = b.IsSold,
                    Slug = b.Slug,

                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .ToList();

            return result;
            */
        }

        // TODO: 需要分頁
        /// <summary>
        /// 管理員查詢書本清單 (清單項目資料，非詳細資料)。
        /// </summary>
        public async Task<IReadOnlyList<AdminBookListItemQueryResult>> GetAdminBookListAsync(
            Expression<Func<UsedBook, bool>> predicate,
            Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy,
            CancellationToken ct = default)
        {
            // 1. 建立查詢（包含關聯載入與篩選條件）
            var query = _db.UsedBooks
                .Where(predicate)
                .Include(b => b.Tags)
                .Include(b => b.ConditionRating);

            // 2. 排序條件
            var orderedQuery = orderBy(query);

            // 3. 分頁條件
            var pagedBooks = await orderedQuery
                //.Skip(pageIndex * pageSize)
                //.Take(pageSize)
                .ToListAsync(ct);        // NOTE: 此處連線把 DB 端資料載入記憶體

            // 4. 封面快取
            // NOTE: 此處再次連線，優點是分開處理可讀性+維護性+SQL好寫
            var bookIds = pagedBooks.Select(b => b.Id).ToList();
            var coverDict = await _db.UsedBookImages
                .Where(img => bookIds.Contains(img.BookId) && img.IsCover)
                .ToDictionaryAsync(img => img.BookId, ct);

            // 5. 投影成結果
            var result = pagedBooks
                .Where(b => coverDict.ContainsKey(b.Id))
                .Select(b => new AdminBookListItemQueryResult
                {
                    CoverStorageProvider = (StorageProvider)coverDict[b.Id].StorageProvider,
                    CoverObjectKey = coverDict[b.Id].ObjectKey,

                    SaleTagList = b.Tags.Select(t => new BookSaleTagQueryResult
                    {
                        Id = t.Id,
                        Name = t.Name,
                        IsActive = t.IsActive
                    }).ToList(),

                    Id = b.Id,
                    Title = b.Title,
                    SellerId = b.SellerId,
                    SalePrice = b.SalePrice,
                    ConditionRating = b.ConditionRating?.Name ?? "",

                    IsOnShelf = b.IsOnShelf,
                    IsActive = b.IsActive,
                    IsSold = b.IsSold,
                    Slug = b.Slug,

                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .ToList();

            return result;
        }

        // ========== 促銷標籤相關 ==========

        /// <summary>
        /// 為指定書籍賦予 SaleTag 促銷標籤
        /// </summary>
        [Obsolete("目前 service 直接使用實體")]
        public async Task<bool> AddSaleTagAsync(Guid bookId, int tagId, CancellationToken ct = default)
        {
            var book = await _db.UsedBooks
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(b => b.Id == bookId, ct);

            if (book == null || book.Tags.Any(t => t.Id == tagId))
                return false;

            var trackedTag = await _db.BookSaleTags.FirstOrDefaultAsync(t => t.Id == tagId, ct);
            if (trackedTag == null)
                return false;

            book.Tags.Add(trackedTag);
            return true;
        }

        /// <summary>
        /// 把指定書籍移除 SaleTag 促銷標籤
        /// </summary>
        public async Task<bool> RemoveBookSaleTagAsync(Guid bookId, int tagId, CancellationToken ct = default)
        {
            var affected = await _db.Set<Dictionary<string, object>>("UsedBookSaleTag")
                .Where(e => (Guid)e["BookId"] == bookId
                    && (int)e["TagId"] == tagId)
                .ExecuteDeleteAsync(ct);
            return affected > 0;
        }
    }
}
