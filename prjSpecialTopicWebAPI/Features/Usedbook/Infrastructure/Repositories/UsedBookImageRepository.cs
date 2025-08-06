using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Features.Usedbook.Enums;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    public class UsedBookImageRepository
    {
        private readonly TeamAProjectContext _db;

        public UsedBookImageRepository(TeamAProjectContext context)
        {
            _db = context;
        }

        // ========== 查詢實體 ==========

        public async Task<IReadOnlyList<UsedBookImage>> GetEntitiesByBookIdAsync(Guid bookId, CancellationToken ct = default)
        {
            return await _db.UsedBookImages
                .Where(bi => bi.BookId == bookId)
                .ToListAsync(ct);
        }

        // ========== 新增、更新、刪除 ==========

        public void AddRange(IEnumerable<UsedBookImage> entities) =>
            _db.UsedBookImages.AddRange(entities);

        public void UpdateRange(IEnumerable<UsedBookImage> entities) =>
            _db.UsedBookImages.UpdateRange(entities);

        public void RemoveRange(IEnumerable<UsedBookImage> entities) =>
            _db.UsedBookImages.RemoveRange(entities);

        public async Task<bool> RemoveByImageIdAsync(int imageId, CancellationToken ct = default)
        {
            var queryResult = await _db.UsedBookImages
                .SingleOrDefaultAsync(bi => bi.Id == imageId, ct);
            if (queryResult == null)
                return false;
            _db.UsedBookImages.Remove(queryResult);
            return true;
        }

        // ========== 查詢 ==========

        public async Task<UsedBookImageQueryResult?> GetByImageIdAsync(int imageId, CancellationToken ct = default)
        {
            var queryResult = await _db.UsedBookImages
                .AsNoTracking()
                .Where(bi => bi.Id == imageId)
                .Select(bi => new UsedBookImageQueryResult
                {
                    Id = bi.Id,
                    IsCover = bi.IsCover,
                    ImageIndex = bi.ImageIndex,
                    StorageProvider = (StorageProvider)bi.StorageProvider,
                    ObjectKey = bi.ObjectKey,
                    Sha256 = bi.Sha256
                })
                .SingleOrDefaultAsync(ct);
            return queryResult;
        }

        public async Task<IReadOnlyList<UsedBookImageQueryResult>> GetByBookIdAsync(Guid bookId, CancellationToken ct = default)
        {
            var queryResult = await _db.UsedBookImages
                .AsNoTracking()
                .Where(bi => bi.BookId == bookId)
                .Select(bi => new UsedBookImageQueryResult
                {
                    Id = bi.Id,
                    IsCover = bi.IsCover,
                    ImageIndex = bi.ImageIndex,
                    StorageProvider = (StorageProvider)bi.StorageProvider,
                    ObjectKey = bi.ObjectKey,
                    Sha256 = bi.Sha256
                })
                .OrderBy(bi => bi.ImageIndex)
                .ToListAsync(ct);
            return queryResult;
        }

        /// <summary>
        /// 根據 BookId 查詢指定書封面。
        /// </summary>
        /// <remarks>雖然新增與修改時已經有檢查邏輯，但此方法容忍存在多筆封面，只取第一張返回。</remarks>
        public async Task<UsedBookImageQueryResult?> GetCoverByBookIdAsync(Guid bookId, CancellationToken ct = default)
        {
            var queryResult = await _db.UsedBookImages
                .AsNoTracking()
                .Where(bi => bi.BookId == bookId && bi.IsCover)
                .Select(bi => new UsedBookImageQueryResult
                {
                    Id = bi.Id,
                    IsCover = bi.IsCover,
                    ImageIndex = bi.ImageIndex,
                    StorageProvider = (StorageProvider)bi.StorageProvider,
                    ObjectKey = bi.ObjectKey,
                    Sha256 = bi.Sha256
                })
                .FirstOrDefaultAsync(ct);        // 多筆容忍，不會拋錯
            return queryResult;
        }
    }
}
