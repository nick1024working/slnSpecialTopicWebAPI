using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories
{
    /// <summary>
    /// 負責操作主題類別 (Topics) 的資料存取邏輯。
    /// </summary>
    /// <remarks>主題由管理員維護，供使用者上架圖書時選擇</remarks>
    public class BookCategoryRepository
    {
        private readonly TeamAProjectContext _db;

        public BookCategoryRepository(TeamAProjectContext context)
        {
            _db = context;
        }

        /// <summary>
        /// 新增一筆主題資料。
        /// </summary>
        public void Add(BookCategory entity)
        {
            _db.BookCategories.Add(entity);
        }

        /// <summary>
        /// 根據指定 Id 更新主題名稱。
        /// </summary>
        /// <param name="entity">包含 Id 與新名稱的主題實體。</param>
        /// <return>
        /// 傳回 <c>true</c> 表示已找到並標記更新目標。
        /// 傳回 <c>false</c> 表示找不到符合條件的紀錄。
        /// </return>
        /// <exception cref="InvalidOperationException">查詢結果超過一筆時拋出，通常代表資料違反唯一性約束。</exception>
        public async Task<bool> UpdateAsync(BookCategory entity, CancellationToken ct = default)
        {
            var queryResult = await _db.BookCategories
                .SingleOrDefaultAsync(st => st.Id == entity.Id, ct);
            if (queryResult == null)
                return false;

            queryResult.Name = entity.Name;

            return true;
        }

        /// <summary>
        /// 檢查是否存在。
        /// </summary>
        public async Task<bool> ExistsAsync(int id, CancellationToken ct) =>
             await _db.BookCategories.AsNoTracking().AnyAsync(t => t.Id == id, ct);

        /// <summary>
        /// 刪除指定主題。
        /// </summary>
        /// <return>
        /// 傳回 <c>true</c> 表示已找到並標記刪除目標（尚未呼叫 <see cref="DbContext.SaveChangesAsync()" />）。
        /// 傳回 <c>false</c> 表示找不到符合條件的紀錄。
        /// </return>
        /// <exception cref="InvalidOperationException">查詢結果超過一筆時拋出，通常代表資料違反唯一性約束。</exception>
        /// <exception cref="ArgumentNullException">查詢結果為 null，卻仍嘗試執行刪除操作時拋出。</exception>
        [Obsolete("此方法禁止使用，因書本參考主題，DeleteBehavior.Restrict。", true)]
        public async Task<bool> RemoveByIdAsync(int id, CancellationToken ct = default)
        {
            var queryResult = await _db.BookCategories
                .SingleOrDefaultAsync(st => st.Id == id, ct);
            if (queryResult == null)
                return false;
            _db.BookCategories.Remove(queryResult);
            return true;
        }

        /// <summary>
        /// 根據指定 Id 查詢主題資訊。
        /// </summary>
        /// <param name="id">主題的唯一識別碼。</param>
        /// <returns>若存在則回傳 <see cref="TopicQueryResult"/>；否則為 null。</returns>
        /// <exception cref="InvalidOperationException">查詢結果超過一筆時拋出，通常代表資料違反唯一性約束。</exception>
        public async Task<BookCategoryResult?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var queryResult = await _db.BookCategories
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new BookCategoryResult
                {
                    Id = t.Id,
                    Name = t.Name,
                })
                .SingleOrDefaultAsync(ct);
            return queryResult;
        }

        /// <summary>
        /// 查詢所有主題清單。
        /// </summary>
        /// <returns>所有主題組成的 <see cref="IReadOnlyList{TopicQueryResult}"/>。</returns>
        public async Task<IReadOnlyList<BookCategoryResult>> GetAllAsync(CancellationToken ct = default)
        {
            var queryResult = await _db.BookCategories
                .AsNoTracking()
                .Select(t => new BookCategoryResult
                {
                    Id = t.Id,
                    Name = t.Name,
                })
                .OrderBy(t => t.Id)
                .ToListAsync(ct);
            return queryResult;
        }

    }
}
