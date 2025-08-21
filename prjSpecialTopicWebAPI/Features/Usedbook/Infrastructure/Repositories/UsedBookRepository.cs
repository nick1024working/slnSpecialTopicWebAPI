using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Query;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Features.Usedbook.Enums;
using prjSpecialTopicWebAPI.Models;
using System.Collections.Generic;
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
        }

        /// <summary>
        /// 管理員查詢書本清單 (清單項目資料，非詳細資料)。
        /// </summary>
        public async Task<PagedResult<AdminBookListItemQueryResult>> GetAdminBookListAsync(
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
                .Where(predicate);
            var total = await baseQuery.CountAsync(ct);

            // 排序 + 分頁 + 投影
            var items = await orderBy(baseQuery)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(b => new AdminBookListItemQueryResult
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

                    SaleTagList = b.Tags.Select(t => new BookSaleTagQueryResult
                    {
                        Id = t.Id,
                        Name = t.Name,
                        IsActive = t.IsActive
                    }).ToList(),

                    IsOnShelf = b.IsOnShelf,
                    IsActive = b.IsActive,
                    IsSold = b.IsSold,
                    Slug = b.Slug,

                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .AsSplitQuery()
                .ToListAsync(ct);

            // 組 PagedResult
            var result = new PagedResult<AdminBookListItemQueryResult>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRows = total
            };

            return result;
        }

        // ========== 促銷標籤相關 ==========

        /// <summary>
        /// 為指定書籍賦予 SaleTag 促銷標籤
        /// </summary>
        public async Task<bool> AddSaleTagAsync(Guid bookId, int tagId, CancellationToken ct = default)
        {
            var joinSet = _db.Set<Dictionary<string, object>>("UsedBookSaleTag");

            await joinSet
                .AddAsync(new Dictionary<string, object>
                {
                    ["BookId"] = bookId,
                    ["TagId"] = tagId
                }, ct);

            return true;
        }

        /// <summary>
        /// 把指定書籍移除 SaleTag 促銷標籤
        /// </summary>
        public async Task<bool> RemoveSaleTagAsync(Guid bookId, int tagId, CancellationToken ct = default)
        {
            var joinSet = _db.Set<Dictionary<string, object>>("UsedBookSaleTag");

            await joinSet
                .Where(e => EF.Property<Guid>(e, "BookId") == bookId && EF.Property<int>(e, "TagId") == tagId)
                .ExecuteDeleteAsync(ct);

            return true;
        }


        public async Task<bool> AddSaleTagBatchAsync(IReadOnlyList<Guid> bookIds, int tagId, CancellationToken ct = default)
        {
            if (bookIds is null || bookIds.Count == 0)
                return true;

            var joinSet = _db.Set<Dictionary<string, object>>("UsedBookSaleTag");
            var ids = bookIds.Distinct().ToArray();

            var existing = await joinSet
                .Where(e => EF.Property<int>(e, "TagId") == tagId
                         && ids.Contains(EF.Property<Guid>(e, "BookId")))
                .Select(e => EF.Property<Guid>(e, "BookId"))
                .ToListAsync(ct);

            var toInsert = ids.Except(existing)
                .Select(id => new Dictionary<string, object>
                {
                    ["BookId"] = id,
                    ["TagId"] = tagId
                })
                .ToList();

            if (toInsert.Count == 0)
                return true;

            await joinSet.AddRangeAsync(toInsert, ct);
            return true;
        }

        public async Task<bool> RemoveSaleTagBatchAsync(IReadOnlyList<Guid> bookIds, int tagId, CancellationToken ct = default)
        {
            if (bookIds is null || bookIds.Count == 0)
                return true;

            var joinSet = _db.Set<Dictionary<string, object>>("UsedBookSaleTag");
            var ids = bookIds.Distinct().ToArray();

            foreach (var bookId in ids)
            {
                await joinSet
                    .Where(e => EF.Property<int>(e, "TagId") == tagId && EF.Property<Guid>(e, "BookId") == bookId)
                    .ExecuteDeleteAsync(ct);
            }

            return true;
        }
    }
}
