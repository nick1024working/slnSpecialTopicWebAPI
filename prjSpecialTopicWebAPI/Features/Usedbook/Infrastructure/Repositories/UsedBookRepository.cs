using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Features.Usedbook.Enums;
using prjSpecialTopicWebAPI.Models;
using System.Linq.Expressions;

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
        /// 直接返回書本實體。
        /// </summary>
        public async Task<UsedBook?> GetEntityByIdAsync(Guid id, CancellationToken ct = default) =>
            await _db.UsedBooks.SingleOrDefaultAsync(b => b.Id == id, ct);


        // ========== 新增、更新 ==========

        public void Add(UsedBook entity) =>
            _db.UsedBooks.Add(entity);

        public async Task<bool> UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken ct = default)
        {
            var entity = await _db.UsedBooks
                .SingleOrDefaultAsync(b => b.Id == id, ct);
            if (entity == null)
                return false;

            entity.SalePrice = request.SalePrice;
            entity.Title = request.Title;
            entity.Authors = request.Authors;
            entity.ConditionRatingId = request.ConditionRatingId;
            entity.ConditionDescription = request.ConditionDescription;
            entity.Edition = request.Edition;
            entity.Publisher = request.Publisher;
            entity.PublicationDate = request.PublicationDate;
            entity.Isbn = request.Isbn;
            entity.BindingId = request.BindingId;
            entity.LanguageId = request.LanguageId;
            entity.Pages = request.Pages;
            entity.ContentRatingId = request.ContentRatingId;
            entity.IsOnShelf = request.IsOnShelf;
            entity.UpdatedAt = DateTime.UtcNow;

            return true;
        }

        public async Task<bool> UpdateActiveStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
        {
            var result = await _db.UsedBooks
                .SingleOrDefaultAsync(b => b.Id == id, ct);
            if (result == null)
                return false;
            result.IsActive = isActive;
            return true;
        }

        // ========== 查詢 ==========

        /// <summary>
        /// 根據 ID 查詢書本完整資訊 (關聯欄位已用字串顯示)。
        /// </summary>
        public async Task<UsedBookQueryResult?> GetTextByIdAsync(Guid id, CancellationToken ct = default)
        {
            var queryResult = await _db.UsedBooks
                .Where(b => b.Id == id)
                .Select(b => new UsedBookQueryResult
                {
                    Id = b.Id,
                    SellerId = b.SellerId,
                    SellerCountyName = b.SellerDistrict.County.Name,
                    SellerDistrictName = b.SellerDistrict.Name,
                    SalePrice = b.SalePrice,
                    Title = b.Title,
                    Authors = b.Authors,
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

        // TODO: 需要分頁
        /// <summary>
        /// 查詢公開書本清單 (清單項目資料，非詳細資料)。
        /// </summary>
        public async Task<IReadOnlyList<PublicBookListItemQueryResult>> GetPublicBookListAsync(
            Expression<Func<UsedBook, bool>> predicate,
            Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy,
            CancellationToken ct = default)
        {
            // 1. 建立查詢（包含關聯載入與篩選條件）
            var query = _db.UsedBooks
                .AsNoTracking()
                .Where(predicate)
                .Include(b => b.Tags)
                .Include(b => b.ContentRating);

            // 2. 排序條件
            var orderedQuery = orderBy(query);

            // 3. 分頁條件
            var pagedBooks = await orderedQuery
                //.Skip(pageIndex * pageSize)
                //.Take(pageSize)
                .ToListAsync(ct);

            // 4. 封面快取
            var bookIds = pagedBooks.Select(b => b.Id).ToList();
            var coverDict = await _db.UsedBookImages
                .AsNoTracking()
                .Where(img => bookIds.Contains(img.BookId) && img.IsCover)
                .ToDictionaryAsync(img => img.BookId, ct);

            // 5. 投影成結果
            var result = pagedBooks
                .Where(b => coverDict.ContainsKey(b.Id))
                .Select(b => new PublicBookListItemQueryResult
                {
                    CoverStorageProvider = (StorageProvider)coverDict[b.Id].StorageProvider,
                    CoverObjectKey = coverDict[b.Id].ObjectKey,

                    SaleTagList = b.Tags.Select(t => t.Name).ToList(),

                    Id = b.Id,
                    Title = b.Title,
                    SalePrice = b.SalePrice,
                    Authors = b.Authors,
                    ConditionRating = b.ConditionRating?.Name ?? "",

                    Slug = b.Slug,
                })
                .ToList();

            return result;
        }

        // TODO: 需要分頁
        /// <summary>
        /// 根據 UserId 查詢該使用者書本清單 (清單項目資料，非詳細資料)。
        /// </summary>
        public async Task<IReadOnlyList<UserBookListItemQueryResult>> GetUserBookListAsync(
            Guid userId,
            Expression<Func<UsedBook, bool>> predicate,
            Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy,
            CancellationToken ct = default)
        {
            // 1. 建立查詢（包含關聯載入與篩選條件）
            var query = _db.UsedBooks
                .AsNoTracking()
                .Where(predicate)
                //.Include(b => b.Tags)     // NOTE: 使用者書本清單不需要 Tags
                .Include(b => b.ContentRating);

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
                .AsNoTracking()
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
                    ConditionRating = b.ContentRating.Name,

                    IsOnShelf = b.IsOnShelf,
                    IsSold = b.IsSold,
                    Slug = b.Slug,

                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .ToList();

            return result;
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
                .AsNoTracking()
                .Where(predicate)
                .Include(b => b.Tags)
                .Include(b => b.ContentRating);

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
                .AsNoTracking()
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
                    ConditionRating = b.ContentRating.Name,

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
            var bookWithTagsEntity = await _db.UsedBooks
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(b => b.Id == bookId, ct);

            if (bookWithTagsEntity == null)
                return false;

            var saleTagToRemove = bookWithTagsEntity.Tags
                .SingleOrDefault(st => st.Id == tagId);
            if (saleTagToRemove == null)
                return false;

            bookWithTagsEntity.Tags.Remove(saleTagToRemove);
            return true;
        }

        // ========== 主題分類相關 ==========

        /// <summary>
        /// 為指定書籍賦予 BookCategory 主題分類
        /// </summary>

        /// <summary>
        /// 把指定書籍移除 BookCategory 主題分類
        /// </summary>
    }
}
